/*
Copyright (c) 2022, Bintang Jakasurya
All rights reserved. 

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/


using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;

using binstarjs03.AerialOBJ.Core.CoordinateSystem;
using binstarjs03.AerialOBJ.Core.MinecraftWorld;
using binstarjs03.AerialOBJ.WpfApp.Services;
using binstarjs03.AerialOBJ.WpfApp.UIElements.Components;
using binstarjs03.AerialOBJ.WpfApp.UIElements.Controls;

using Range = binstarjs03.AerialOBJ.Core.Range;
using Region = binstarjs03.AerialOBJ.Core.MinecraftWorld.Region;

namespace binstarjs03.AerialOBJ.WpfApp;

public class ChunkRegionManager
{
    // twelve should be more than enough to avoid collection resizing,
    // unless for huge monitor resolution at zoom level 0 of course
    private const int s_regionBufferSize = 15;
    private const int s_chunkBufferSize = s_regionBufferSize * Region.TotalChunkCount;

    private readonly ViewportControlVM _viewport;
    private readonly AutoResetEvent _messageEvent = new(initialState: false);

    // we communicate between chunk region manager thread and other threads using delegate message and such
    private readonly Thread _messageLoopThread;
    private readonly Queue<Action> _highPriorityMessageQueue = new(10);
    private readonly Queue<Action> _messageQueue = new(50);
    private readonly Queue<Coords2> _removeWorkedChunkQueue = new(20);
    private readonly object _messageLock = new(); // lock all messages simultaneously

    private readonly Random _rng = new();

    private readonly Dictionary<Coords2, RegionWrapper> _loadedRegions = new(s_regionBufferSize);
    private readonly Dictionary<Coords2, RegionWrapper> _loadedRegionsBuffer = new(s_regionBufferSize);
    private readonly Dictionary<Coords2, Region> _regionCache = new(s_regionBufferSize);
    private readonly List<Coords2> _pendingRegionList = new(s_regionBufferSize);
    private Task _loadRegionTask;
    private Coords2? _workedRegion = null;
    private readonly object _workedLoadedRegionLock = new(); // lock two objects simultaneously, this is to prevent concurrency problem such as deadlocks

    private readonly Dictionary<Coords2, ChunkWrapper> _loadedChunks = new(s_regionBufferSize * Region.TotalChunkCount);
    private readonly HashSet<Coords2> _pendingChunkSet = new(s_chunkBufferSize);
    private readonly List<Coords2> _pendingChunkList = new(s_chunkBufferSize); // lock pendingChunkList only and both the set and the list
                                                                               // must be in locking scope to keep both synced
    private readonly List<Task> _loadChunkTasks = new(Environment.ProcessorCount);
    private readonly List<Coords2> _workedChunks = new(Environment.ProcessorCount);

    private CoordsRange2 _visibleRegionRange;
    private CoordsRange2 _visibleChunkRange;
    private int _displayedHeightLimit;

    // public accessors
    public ViewportControlVM Viewport => _viewport;
    public CoordsRange2 VisibleRegionRange => _visibleRegionRange;
    public CoordsRange2 VisibleChunkRange => _visibleChunkRange;
    public int VisibleRegionCount => _visibleRegionRange.Sum;
    public int LoadedRegionCount => _loadedRegions.Count;
    public int PendingRegionCount => _pendingRegionList.Count;
    public string WorkedRegion => _workedRegion is null ? "None" : _workedRegion.ToString()!;

    public int VisibleChunkCount => _visibleChunkRange.Sum;
    public int LoadedChunkCount => _loadedChunks.Count;
    public int PendingChunkCount => _pendingChunkList.Count;
    public int WorkedChunkCount => _workedChunks.Count;


    public ChunkRegionManager(ViewportControlVM viewport)
    {
        _viewport = viewport;
        _messageLoopThread = new Thread(MainLoop)
        {
            Name = $"{nameof(ChunkRegionManager)} Thread",
            IsBackground = true,
        };
        _loadRegionTask = new Task(LoadRegionTaskMethod);
        App.Current.Initializing += OnAppInitializing;
    }

    private void OnAppInitializing(object sender, System.Windows.StartupEventArgs e)
    {
        _messageLoopThread.Start();
        LogService.Log($"{nameof(ChunkRegionManager)} Thread started");
    }

    /// <summary>
    /// <see cref="ChunkRegionManager"/> Thread Message Processing Loop procedure
    /// </summary>
    private void MainLoop()
    {
        TimeSpan redrawLatency = TimeSpan.FromMilliseconds(30);
        while (true)
        {
            DateTime messageLoopTimeExpiration = DateTime.Now.AddMilliseconds(redrawLatency.Milliseconds);

            // Keep spinning through message loop for 30 ms. 30 ms is roughly
            // 33 frames-per-second and that's when region image will be redrawn
            for (; DateTime.Now < messageLoopTimeExpiration;)
                processMessageLoop();
            if (getNeedRedrawRegionImageCount() > 0)
                RedrawRegionImages();
        }

        void processMessageLoop()
        {
            if (getMessageCount() == 0)
            {
                _messageEvent.WaitOne(1);
                return;
            }
            // process 3 high priority  and only one normal priority in single call
            RemoveWorkedChunk();
            ProcessMessage(3, _highPriorityMessageQueue);
            ProcessMessage(1, _messageQueue);
        }

        int getMessageCount()
        {
            lock (_messageLock)
                return _highPriorityMessageQueue.Count + _messageQueue.Count;
        }

        int getNeedRedrawRegionImageCount()
        {
            int needRedraws = 0;
            lock (_workedLoadedRegionLock)
                foreach (RegionWrapper regionWrapper in _loadedRegions.Values)
                    if (regionWrapper.NeedRedraw)
                        needRedraws++;
            return needRedraws;
        }
    }

    private void ProcessMessage(int messageCount, Queue<Action> messageQueue)
    {
        for (int i = 0; i < messageCount; i++)
        {
            Action msg;
            lock (_messageLock)
            {
                if (messageQueue.Count == 0)
                    return;
                msg = messageQueue.Dequeue();
            }
            msg();
        }
    }

    private void RemoveWorkedChunk()
    {
        lock (_messageLock)
        {
            while (_removeWorkedChunkQueue.Count > 0)
            {
                Coords2 chunkCoordsAbs = _removeWorkedChunkQueue.Dequeue();
                _workedChunks.Remove(chunkCoordsAbs);
            }
        }
        _viewport.NotifyPropertyChanged(nameof(ViewportControlVM.ChunkRegionManagerWorkedChunkCount));
    }

    public enum MessagePriority
    {
        Normal,
        High,
    }

    /// <summary>
    /// Pass a delegate message for <see cref="ChunkRegionManager"/> Thread to execute
    /// </summary>
    /// <param name="msg">The message to be executed by Chunk Region Manager Thread</param>
    public void PostMessage(Action msg, MessagePriority priority, bool noDuplicate = false)
    {
        lock (_messageLock)
        {
            switch (priority)
            {
                case MessagePriority.Normal:
                    if (noDuplicate)
                    {
                        if (!_messageQueue.Contains(msg))
                            _messageQueue.Enqueue(msg);
                    }
                    else
                        _messageQueue.Enqueue(msg);
                    break;
                case MessagePriority.High:
                    if (noDuplicate)
                    {
                        if (!_highPriorityMessageQueue.Contains(msg))
                            _highPriorityMessageQueue.Enqueue(msg);
                    }
                    else
                        _highPriorityMessageQueue.Enqueue(msg);
                    break;
                default:
                    throw new NotImplementedException();
            }
        }
        _messageEvent.Set();
    }

    private void PostRemoveWorkedChunk(Coords2 workedChunkCoordsAbs)
    {
        lock (_messageLock)
            _removeWorkedChunkQueue.Enqueue(workedChunkCoordsAbs);
    }

    public void Update()
    {
        if (App.Current.State.SavegameLoadInfo is null)
            return;
        _displayedHeightLimit = _viewport.HeightLimit;
        if (RecalculateVisibleChunkRange())
        {
            _viewport.NotifyPropertyChanged(nameof(ViewportControlVM.ChunkRegionManagerVisibleChunkCount));
            _viewport.NotifyPropertyChanged(nameof(ViewportControlVM.ChunkRegionManagerVisibleChunkRangeXStringized));
            _viewport.NotifyPropertyChanged(nameof(ViewportControlVM.ChunkRegionManagerVisibleChunkRangeZStringized));
            if (RecalculateVisibleRegionRange())
            {
                _viewport.NotifyPropertyChanged(nameof(ViewportControlVM.ChunkRegionManagerVisibleRegionCount));
                _viewport.NotifyPropertyChanged(nameof(ViewportControlVM.ChunkRegionManagerVisibleRegionRangeXStringized));
                _viewport.NotifyPropertyChanged(nameof(ViewportControlVM.ChunkRegionManagerVisibleRegionRangeZStringized));
                LoadUnloadRegions();
            }
            LoadUnloadChunks();
        }
        UpdateRegionImageTransformation();
    }

    /// <returns>true if chunk range changed</returns>
    private bool RecalculateVisibleChunkRange()
    {
        ViewportControlVM v = _viewport;
        PointF2 viewportCameraPos;
        PointF2 viewportChunkCanvasCenter;
        float viewportPixelPerChunk;

        lock (_viewport)
        {
            viewportCameraPos = v.CameraPos;
            viewportChunkCanvasCenter = v.ScreenCenter;
            viewportPixelPerChunk = v.PixelPerChunk;
        }

        // Camera chunk in here means which chunk the camera is pointing to.
        // Here we dont use int because floating point accuracy is crucial
        double xCameraChunk = viewportCameraPos.X / Section.BlockCount;
        double zCameraChunk = viewportCameraPos.Y / Section.BlockCount;

        // min/maxCanvasCenterChunk means which chunk that is visible at the edgemost of the control
        // that is measured by the length or height of the control size (which is chunk canvas)
        double minXCanvasCenterChunk = -(viewportChunkCanvasCenter.X / viewportPixelPerChunk);
        double maxXCanvasCenterChunk = viewportChunkCanvasCenter.X / viewportPixelPerChunk;
        int minX = (int)Math.Floor(xCameraChunk + minXCanvasCenterChunk);
        int maxX = (int)Math.Floor(xCameraChunk + maxXCanvasCenterChunk);
        Range visibleChunkXRange = new(minX, maxX);

        double minZCanvasCenterChunk = -(viewportChunkCanvasCenter.Y / viewportPixelPerChunk);
        double maxZCanvasCenterChunk = viewportChunkCanvasCenter.Y / viewportPixelPerChunk;
        int minZ = (int)Math.Floor(zCameraChunk + minZCanvasCenterChunk);
        int maxZ = (int)Math.Floor(zCameraChunk + maxZCanvasCenterChunk);
        Range visibleChunkZRange = new(minZ, maxZ);

        CoordsRange2 oldVisibleChunkRange = _visibleChunkRange;
        CoordsRange2 newVisibleChunkRange = new(visibleChunkXRange, visibleChunkZRange);
        if (newVisibleChunkRange == oldVisibleChunkRange)
            return false;
        _visibleChunkRange = newVisibleChunkRange;
        return true;
    }

    /// <returns>true if region range changed</returns>
    private bool RecalculateVisibleRegionRange()
    {
        CoordsRange2 vcr = _visibleChunkRange;

        // Calculating region range is easier since we only need to
        // divide the range by how many chunks in region (in single axis)

        int regionMinX = (int)MathF.Floor((float)vcr.XRange.Min / Region.ChunkCount);
        int regionMaxX = (int)MathF.Floor((float)vcr.XRange.Max / Region.ChunkCount);
        Range visibleRegionXRange = new(regionMinX, regionMaxX);

        int regionMinZ = (int)MathF.Floor((float)vcr.ZRange.Min / Region.ChunkCount);
        int regionMaxZ = (int)MathF.Floor((float)vcr.ZRange.Max / Region.ChunkCount);
        Range visibleRegionZRange = new(regionMinZ, regionMaxZ);

        CoordsRange2 oldVisibleRegionRange = _visibleRegionRange;
        CoordsRange2 newVisibleRegionRange = new(visibleRegionXRange, visibleRegionZRange);
        if (newVisibleRegionRange == oldVisibleRegionRange)
            return false;
        _visibleRegionRange = newVisibleRegionRange;
        return true;
    }

    #region Region Managing Procedure

    private void LoadUnloadRegions()
    {
        // perform boundary range checking for regions outside display frame
        // unload them if outside
        lock (_workedLoadedRegionLock)
            foreach ((Coords2 regionCoords, RegionWrapper regionWrapper) in _loadedRegions)
            {
                if (_visibleRegionRange.IsInside(regionCoords))
                    continue;
                UnloadRegion(regionWrapper);
            }

        // remove all pending region list that is no longer visible
        lock (_pendingRegionList)
            _pendingRegionList.RemoveAll(regionCoords => _visibleRegionRange.IsOutside(regionCoords));

        // perform sweep checking from min to max range for visible regions
        // add them to pending list if not loaded yet, not in the list, and is not worked yet
        for (int x = _visibleRegionRange.XRange.Min; x <= _visibleRegionRange.XRange.Max; x++)
            for (int z = _visibleRegionRange.ZRange.Min; z <= _visibleRegionRange.ZRange.Max; z++)
            {
                // TODO IOService should cache which region coords has region file,
                // so IO operation does not block execution
                Coords2 regionCoords = new(x, z);
                lock (_workedLoadedRegionLock)
                    if (_loadedRegions.ContainsKey(regionCoords)
                        || !IOService.HasRegionFile(regionCoords)
                        || _workedRegion == regionCoords)
                        continue;
                lock (_pendingRegionList)
                    if (!_pendingRegionList.Contains(regionCoords))
                        _pendingRegionList.Add(regionCoords);
            }

        // dispatch pending regions
        if (_loadRegionTask.Status != TaskStatus.Running)
            _loadRegionTask = Task.Run(LoadRegionTaskMethod);
        _viewport.NotifyPropertyChanged(nameof(ViewportControlVM.ChunkRegionManagerPendingRegionCount));
    }

    private void LoadRegionTaskMethod()
    {
        // Load all regions until pending list exhausted
        while (true)
        {
            Coords2 regionCoords;
            lock (_pendingRegionList)
            {
                if (_pendingRegionList.Count == 0)
                    return;
                int randomIndex = _rng.Next(0, _pendingRegionList.Count);
                regionCoords = _pendingRegionList[randomIndex];
                _pendingRegionList.RemoveAt(randomIndex);
                _workedRegion = regionCoords;
            }
            _viewport.NotifyPropertyChanged(nameof(ViewportControlVM.ChunkRegionManagerPendingRegionCount));
            _viewport.NotifyPropertyChanged(nameof(ViewportControlVM.ChunkRegionManagerWorkedRegion));

            Region region;

            // Try to fetch region from cache. If miss, load it from disk
            //if (_regionCache.ContainsKey(regionCoords))
            //    region = _regionCache[regionCoords];
            //else
            //{
            Region? regionFromDisk = IOService.ReadRegionFile(regionCoords, out Exception? e);
            // cancel region loading if we can't get the region at specified coords
            // (corrupted, file not exist or not generated yet etc)
            if (regionFromDisk is null)
            {
                if (e is not null)
                {
                    // TODO display messagebox only once and never again
                    LogService.LogError($"Region {regionCoords} was skipped.");
                    LogService.LogError($"Cause of exception: {e.GetType()}");
                    LogService.LogError($"Exception details: {e}", useSeparator: true);
                }
                lock (_workedLoadedRegionLock)
                    _workedRegion = null;
                _viewport.NotifyPropertyChanged(nameof(ViewportControlVM.ChunkRegionManagerWorkedRegion));
                continue;
            }
            //    _regionCache.Add(regionCoords, regionFromDisk);
            region = regionFromDisk;
            //}
            RegionWrapper regionWrapper = new(region, _viewport);
            //regionWrapper.SetRandomImage();
            regionWrapper.RedrawImage();
            regionWrapper.UpdateImageTransformation();
            LoadRegion(regionWrapper);

            lock (_workedLoadedRegionLock)
                _workedRegion = null;
            _viewport.NotifyPropertyChanged(nameof(ViewportControlVM.ChunkRegionManagerWorkedRegion));
        }
    }

    private void LoadRegion(RegionWrapper regionWrapper)
    {
        // cancel region loading if specified region is outside view screen
        lock (_workedLoadedRegionLock)
        {
            if (!_visibleRegionRange.IsInside(regionWrapper.RegionCoords)
                || _loadedRegions.ContainsKey(regionWrapper.RegionCoords)
                || App.Current.State.SavegameLoadInfo is null)
                return;
            _loadedRegions.Add(regionWrapper.RegionCoords, regionWrapper);
        }
        ShowRegionImage(regionWrapper.RegionImage);
        OnRegionLoadChanged();
    }

    private void UnloadRegion(RegionWrapper regionWrapper)
    {
        // We should not unload region unless if session is closed.
        // This is to make chunk loading faster if region is not loaded yet
        // since loading region is pretty intensive.
        // Of course we can consider if the system have little available memory,
        // maybe unload region that is not visible in that case.
        lock (_workedLoadedRegionLock)
            _loadedRegions.Remove(regionWrapper.RegionCoords);
        RemoveOrHideRegionImage(regionWrapper.RegionImage);
        OnRegionLoadChanged();
    }

    private void ShowRegionImage(RegionImage regionImage)
    {
        App.InvokeDispatcher(
            () => _viewport.Control.ViewportCanvas.Children.Add(regionImage.Image),
            DispatcherPriority.Render,
            DispatcherSynchronization.Asynchronous);
    }

    private void RemoveOrHideRegionImage(RegionImage regionImage)
    {
        App.InvokeDispatcher(
            () => _viewport.Control.ViewportCanvas.Children.Remove(regionImage.Image),
            DispatcherPriority.Render,
            DispatcherSynchronization.Asynchronous);
    }

    private void OnRegionLoadChanged()
    {
        _viewport.NotifyPropertyChanged(nameof(ViewportControlVM.ChunkRegionManagerLoadedRegionCount));
    }

    #endregion Region Managing Procedure

    #region Chunk Managing Procedure

    private void LoadUnloadChunks()
    {
        // perform boundary range checking for chunks outside display frame
        foreach ((Coords2 chunkCoordsAbs, ChunkWrapper chunkWrapper) in _loadedChunks)
        {
            if (_visibleChunkRange.IsInside(chunkCoordsAbs))
                continue;
            UnloadChunk(chunkWrapper);
        }

        // remove all pending chunk queue that is no longer visible
        _pendingChunkSet.RemoveWhere(
            chunkCoordsAbs => !_visibleChunkRange.IsInside(chunkCoordsAbs));
        _pendingChunkList.RemoveAll(
            chunkCoordsAbs => !_visibleChunkRange.IsInside(chunkCoordsAbs));

        // perform sweep-checking from min range to max range for chunks inside display frame
        for (int x = _visibleChunkRange.XRange.Min; x <= _visibleChunkRange.XRange.Max; x++)
            for (int z = _visibleChunkRange.ZRange.Min; z <= _visibleChunkRange.ZRange.Max; z++)
            {
                Coords2 chunkCoordsAbs = new(x, z);
                // set is a whole lot faster to check for item existence
                // if the content has hundreds of items, especially for
                // tight-loop like this (approx. millions of comparison performed)
                if (_loadedChunks.ContainsKey(chunkCoordsAbs)
                    || _pendingChunkSet.Contains(chunkCoordsAbs)
                    || _workedChunks.Contains(chunkCoordsAbs))
                    continue;
                _pendingChunkSet.Add(chunkCoordsAbs);
                _pendingChunkList.Add(chunkCoordsAbs);
            }
        _viewport.NotifyPropertyChanged(nameof(ViewportControlVM.ChunkRegionManagerPendingChunkCount));

        // we are executing this currently body of method from chunk region manager thread,
        // but lets take a break and queue what it needs to execute later so the thread message
        // processing routine doesnt get blocked and able to process more pending messages
        LoadChunkTaskSpawnerMethod();

    }

    private RegionWrapper? GetRegionWrapper(Coords2 chunkCoordsAbs, out bool pending)
    {
        Coords2 regionCoords = Region.GetRegionCoordsFromChunkCoordsAbs(chunkCoordsAbs);
        lock (_workedLoadedRegionLock)
        {
            if (_loadedRegions.ContainsKey(regionCoords))
            {
                pending = false;
                return _loadedRegions[regionCoords];
            }
            else if (_workedRegion == regionCoords)
            {
                pending = true;
                return null;
            }
        }
        lock (_pendingRegionList)
            if (_pendingRegionList.Contains(regionCoords))
            {
                pending = true;
                return null;
            }
        pending = false;
        return null;
    }

    private void LoadChunkTaskSpawnerMethod()
    {
        _loadChunkTasks.RemoveAll(task => task.IsCompleted);

        // TODO set max chunk worker thread count according to user setting chunk threads!
        while (_loadChunkTasks.Count < Environment.ProcessorCount)
        {
            Coords2 chunkCoordsAbs;
            if (_pendingChunkSet.Count == 0)
                return;
            int index = _rng.Next(0, _pendingChunkSet.Count);
            chunkCoordsAbs = _pendingChunkList[index];
            _pendingChunkSet.Remove(chunkCoordsAbs);
            _pendingChunkList.RemoveAt(index);
            _viewport.NotifyPropertyChanged(nameof(ViewportControlVM.ChunkRegionManagerPendingChunkCount));

            // Check if region is loaded. It can be in 3 different states followed by its respective resolutions:
            // - found and loaded -> proceed to process that chunk
            // - not found but pending,          -> put chunk coords back to pending list,
            //   being worked on it for example  -> and maybe later in next encounter, region may be already loaded
            // - not found and not pending -> forget that chunk

            Coords2 regionCoords = Region.GetRegionCoordsFromChunkCoordsAbs(chunkCoordsAbs);

            // search for the underlying loaded region this chunk is in
            // based on the absolute coordinate of it
            RegionWrapper? regionWrapper = GetRegionWrapper(chunkCoordsAbs, out bool pending);
            if (regionWrapper is null)
            {
                if (pending)
                {
                    // found regionwrapper, but is being worked on so it is pending.
                    // put chunk coords abs back to pending list
                    _pendingChunkList.Add(chunkCoordsAbs);
                    _pendingChunkSet.Add(chunkCoordsAbs);
                    _viewport.NotifyPropertyChanged(nameof(ViewportControlVM.ChunkRegionManagerPendingChunkCount));
                    continue;
                }
                else
                    // regionwrapper not found, means it may be in off screen, not generated yet etc.
                    // in this case simply just continue iteration and forget this chunk
                    continue;
            }

            // last test, check if chunk has generated, forget it if not so
            Coords2 chunkCoordsRel = Region.ConvertChunkCoordsAbsToRel(chunkCoordsAbs);
            if (!regionWrapper.HasChunkGenerated(chunkCoordsRel))
                continue;

            _workedChunks.Add(chunkCoordsAbs);
            Task task = Task.Run(() => LoadChunkTaskMethod(chunkCoordsAbs, regionWrapper));
            _loadChunkTasks.Add(task);
            _viewport.NotifyPropertyChanged(nameof(ViewportControlVM.ChunkRegionManagerWorkedChunkCount));
        }

    }

    private void LoadChunkTaskMethod(Coords2 chunkCoordsAbs, RegionWrapper regionWrapper)
    {
        // locking is unneccessary because chunk may not be visible anymore
        // at any point in time after this statement, so its irrelevant
        if (!_visibleChunkRange.IsInside(chunkCoordsAbs))
        {
            postExitMessage();
            return;
        }
        Chunk chunk = regionWrapper.GetChunk(chunkCoordsAbs, false);
        ChunkWrapper chunkWrapper = new(chunk);
        int renderedHeightLimit = _viewport.HeightLimit;
        chunk.GetHighestBlock(chunkWrapper.HighestBlocks, heightLimit: renderedHeightLimit);
        regionWrapper.BlitChunkImage(chunkWrapper.ChunkCoordsRel, chunkWrapper.HighestBlocks);

        postExitMessage();
        PostMessage(() => LoadChunk(chunkWrapper), MessagePriority.Normal);

        // if this method wants to return (or aborting), we have to remove the chunk coordinate
        // this thread is working on in the list and let background loop know that we are done
        void postExitMessage()
        {
            PostRemoveWorkedChunk(chunkCoordsAbs);
            // Let chunk region manager thread know that it has to process/check for more pending chunks.
            // Post LoadChunkTaskSpawnerMethod if it doesnt exist yet in msg queue.
            // This is to prevent spamming it to the message queue and chunkregionmanager
            // can spawn multiple load chunk task in single call, avoiding unneccessary multiple calls
            PostMessage(LoadChunkTaskSpawnerMethod, MessagePriority.High, noDuplicate: true);
        }
    }

    private void LoadChunk(ChunkWrapper chunkWrapper)
    {
        if (!_visibleChunkRange.IsInside(chunkWrapper.ChunkCoordsAbs)
            || _loadedChunks.ContainsKey(chunkWrapper.ChunkCoordsAbs)
            || App.Current.State.SavegameLoadInfo is null)
            return;
        _loadedChunks.Add(chunkWrapper.ChunkCoordsAbs, chunkWrapper);
        OnChunkLoadChanged();
    }

    public void UnloadChunk(ChunkWrapper chunkWrapper)
    {
        if (!_loadedChunks.ContainsKey(chunkWrapper.ChunkCoordsAbs))
            return;
        _loadedChunks.Remove(chunkWrapper.ChunkCoordsAbs);
        RegionWrapper? regionWrapper = GetRegionWrapper(chunkWrapper.ChunkCoordsAbs, out _);
        if (regionWrapper is not null)
            regionWrapper.EraseChunkImage(chunkWrapper.ChunkCoordsRel);
        OnChunkLoadChanged();
    }

    private void OnChunkLoadChanged()
    {
        _viewport.NotifyPropertyChanged(nameof(ViewportControlVM.ChunkRegionManagerLoadedChunkCount));
    }

    #endregion Chunk Managing Procedure

    private void RedrawRegionImages()
    {
        App.InvokeDispatcher(method, DispatcherPriority.Render, DispatcherSynchronization.Synchronous);
        void method()
        {
            lock (_workedLoadedRegionLock)
                foreach ((Coords2 regionCoords, RegionWrapper regionWrapper) in _loadedRegions)
                    if (_visibleRegionRange.IsInside(regionCoords) && regionWrapper.NeedRedraw)
                        regionWrapper.RedrawImage();
        }
    }

    private void UpdateRegionImageTransformation()
    {
        lock (_workedLoadedRegionLock)
            App.InvokeDispatcher(method, DispatcherPriority.Render, DispatcherSynchronization.Synchronous);
        void method()
        {
            foreach ((Coords2 regionCoords, RegionWrapper regionWrapper) in _loadedRegions)
                if (_visibleRegionRange.IsInside(regionCoords))
                    regionWrapper.UpdateImageTransformation();
        }
    }

    public void OnSessionClosed()
    {
        List<RegionWrapper> loadedRegions = new(_loadedRegions.Values);
        foreach (RegionWrapper region in loadedRegions)
            UnloadRegion(region);
        _loadedRegions.Clear();
        _pendingRegionList.Clear();
        _regionCache.Clear();
        _visibleRegionRange = new CoordsRange2();
        _visibleChunkRange = new CoordsRange2();
    }
}

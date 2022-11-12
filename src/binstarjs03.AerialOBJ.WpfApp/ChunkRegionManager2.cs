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

public class ChunkRegionManager2
{
    private const int s_regionBufferSize = 15;
    private const int s_chunkBufferSize = s_regionBufferSize * Region.TotalChunkCount;
    private const int s_framesPerSecond = 24;
    private const int s_redrawFrequency = 1000 / s_framesPerSecond; // unit: miliseconds

    private readonly ViewportControlVM _viewport;

    private readonly AutoResetEvent _messageEvent = new(false);
    private readonly Queue<Action> _messageQueue = new(50);
    private readonly Random _rng = new();
    private readonly Dictionary<Coords2, Region> _regionCache = new(s_regionBufferSize);

    private readonly Dictionary<Coords2, RegionWrapper> _loadedRegions = new(s_regionBufferSize);
    private readonly List<Coords2> _pendingRegionList = new(s_regionBufferSize);
    private Coords2? _workedRegion = null;
    private Task _workRegionTask;

    private readonly Dictionary<Coords2, ChunkWrapper> _loadedChunks = new(s_regionBufferSize * Region.TotalChunkCount);
    private readonly HashSet<Coords2> _pendingChunkSet = new(s_chunkBufferSize);
    private readonly List<Coords2> _pendingChunkList = new(s_chunkBufferSize);
    private readonly List<Coords2> _workedChunks = new(Environment.ProcessorCount);
    private readonly List<Task> _workedChunkTasks = new(Environment.ProcessorCount);
    private object _needRespawnChunkTasksLock = new();
    private bool _needRespawnChunkTasks;

    private CoordsRange2 _visibleRegionRange = new();
    private CoordsRange2 _visibleChunkRange = new();

    // public accessors
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

    public ChunkRegionManager2(ViewportControlVM viewport)
    {
        _viewport = viewport;
        _workRegionTask = new Task(() => { });
        new Task(MessageLoop, TaskCreationOptions.LongRunning).Start();
        new Task(RedrawLoop, TaskCreationOptions.LongRunning).Start();
    }

    private void MessageLoop()
    {
        while (true)
        {
            int messageCount;
            bool needRespawnChunkTasks;
            lock (_needRespawnChunkTasksLock)
                needRespawnChunkTasks = _needRespawnChunkTasks;
            lock (_messageQueue)
                messageCount = _messageQueue.Count;
            if (messageCount == 0)
                _messageEvent.WaitOne();
            if (needRespawnChunkTasks)
                LoadChunkTaskMethodSpawner();
            ProcessMessage();
        }
    }

    private async void RedrawLoop()
    {
        PeriodicTimer redrawTimer = new(TimeSpan.FromMilliseconds(s_redrawFrequency));
        while (await redrawTimer.WaitForNextTickAsync())
            PostMessage(RedrawRegionImages, noDuplicate: true);
    }

    private void ProcessMessage()
    {
        Action message;
        lock (_messageQueue)
            if (_messageQueue.Count > 0)
                message = _messageQueue.Dequeue();
            else
                return;
        message.Invoke();
    }

    public void PostMessage(Action message, bool noDuplicate)
    {
        lock (_messageQueue)
        {
            if (noDuplicate)
                if (!_messageQueue.Contains(message))
                    _messageQueue.Enqueue(message);
            _messageQueue.Enqueue(message);
        }
        _messageEvent.Set();
    }

    public void Update()
    {
        if (!App.Current.State.HasSavegameLoaded)
            return;
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

    private void LoadUnloadRegions()
    {
        // perform boundary range checking for regions outside display frame
        // unload them if outside
        //lock (_loadedRegions)
        //    foreach ((Coords2 regionCoords, RegionWrapper regionWrapper) in _loadedRegions)
        //        if (_visibleRegionRange.IsOutside(regionCoords))
        //            UnloadRegion(regionWrapper);

        // remove all pending region list that is no longer visible
        lock (_pendingRegionList)
            _pendingRegionList.RemoveAll(regionCoords => _visibleRegionRange.IsOutside(regionCoords));

        // perform sweep checking from min to max range for visible regions
        // add them to pending list if not loaded yet, not in the list, and is not worked yet
        for (int x = _visibleRegionRange.XRange.Min; x <= _visibleRegionRange.XRange.Max; x++)
            for (int z = _visibleRegionRange.ZRange.Min; z <= _visibleRegionRange.ZRange.Max; z++)
            {
                Coords2 regionCoords = new(x, z);
                lock (_loadedRegions)
                    lock (_pendingRegionList)
                        if (!_loadedRegions.ContainsKey(regionCoords)
                            || !_pendingRegionList.Contains(regionCoords)
                            || IOService.HasRegionFile(regionCoords)
                            || _workedRegion != regionCoords)
                            _pendingRegionList.Add(regionCoords);
            }
        if (_workRegionTask.Status != TaskStatus.Running)
            _workRegionTask = Task.Run(LoadRegionTaskMethod);
    }

    private void LoadRegionTaskMethod()
    {
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
            }
            _workedRegion = regionCoords;
            _viewport.NotifyPropertyChanged(nameof(ViewportControlVM.ChunkRegionManagerPendingRegionCount));
            _viewport.NotifyPropertyChanged(nameof(ViewportControlVM.ChunkRegionManagerWorkedRegion));

            Region region;
            // Try to fetch region from cache. If miss, load it from disk
            if (_regionCache.ContainsKey(regionCoords))
                region = _regionCache[regionCoords];
            else
            {
                Region? regionDisk = IOService.ReadRegionFile(regionCoords, out Exception? e);
                // cancel region loading if we can't get the region at specified coords
                // (corrupted, file not exist or not generated yet etc)
                if (regionDisk is null)
                {
                    if (e is not null)
                    {
                        // TODO display messagebox only once and never again
                        LogService.LogError($"Region {regionCoords} was skipped.");
                        LogService.LogError($"Cause of exception: {e.GetType()}");
                        LogService.LogError($"Exception details: {e}", useSeparator: true);
                    }
                    continue;
                }
                region = regionDisk;
                _regionCache.Add(regionCoords, region);
            }

            RegionWrapper regionWrapper = new(region, _viewport);
            regionWrapper.SetRandomImage();
            LoadRegion(regionWrapper);
            _workedRegion = null;
            _viewport.NotifyPropertyChanged(nameof(ViewportControlVM.ChunkRegionManagerWorkedRegion));
        }
    }

    // TODO showing region image should be done at redraw cycle
    private void LoadRegion(RegionWrapper regionWrapper)
    {
        lock (_loadedRegions)
        {
            // cancel region loading if specified region is outside view screen
            if (!_visibleRegionRange.IsInside(regionWrapper.RegionCoords)
                || _loadedRegions.ContainsKey(regionWrapper.RegionCoords)
                || App.Current.State.SavegameLoadInfo is null)
                return;
            _loadedRegions.Add(regionWrapper.RegionCoords, regionWrapper);
        }
        ShowRegionImage(regionWrapper.RegionImage);
        OnRegionLoadChanged();
    }

    // TODO removing region image should be done at redraw cycle
    private void UnloadRegion(RegionWrapper regionWrapper)
    {
        // We should not unload region unless if we must (e.g session is closed,
        // low memory, too far from visible range etc). This is to make chunk
        // loading faster if region is not loaded yet since loading region is
        // pretty intensive. Most region files are over 5MB.
        lock (_loadedRegions)
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

    private void RedrawRegionImages()
    {
        App.InvokeDispatcher(method, DispatcherPriority.Render, DispatcherSynchronization.Synchronous);
        void method()
        {
            lock (_loadedRegions)
                foreach ((Coords2 regionCoords, RegionWrapper regionWrapper) in _loadedRegions)
                    regionWrapper.RedrawImage();
        }
    }

    private RegionWrapper? GetRegionWrapper(Coords2 chunkCoordsAbs, out bool pending)
    {
        Coords2 regionCoords = Region.GetRegionCoordsFromChunkCoordsAbs(chunkCoordsAbs);
        lock (_loadedChunks)
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


    private void LoadUnloadChunks()
    {
        // perform boundary range checking for chunks outside display frame
        foreach ((Coords2 chunkCoordsAbs, ChunkWrapper chunkWrapper) in _loadedChunks)
            if (_visibleChunkRange.IsOutside(chunkCoordsAbs))
                UnloadChunk(chunkWrapper);

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
                lock (_workedChunks)
                    if (_loadedChunks.ContainsKey(chunkCoordsAbs)
                        || _pendingChunkSet.Contains(chunkCoordsAbs)
                        || _workedChunks.Contains(chunkCoordsAbs))
                        continue;
                _pendingChunkSet.Add(chunkCoordsAbs);
                _pendingChunkList.Add(chunkCoordsAbs);
            }
        _viewport.NotifyPropertyChanged(nameof(ViewportControlVM.ChunkRegionManagerPendingChunkCount));
        LoadChunkTaskMethodSpawner();
    }

    private void LoadChunkTaskMethodSpawner()
    {
        lock (_needRespawnChunkTasksLock)
            _needRespawnChunkTasks = false;
        _workedChunkTasks.RemoveAll(task => task.IsCompleted);
        // TODO set max chunk worker thread count according to user setting chunk threads!
        while (_workedChunkTasks.Count < Environment.ProcessorCount)
        {
            Coords2 chunkCoordsAbs;
            if (_pendingChunkSet.Count == 0)
                return;
            // get random pending chunk
            int randomCoords = _rng.Next(0, _pendingChunkSet.Count);
            chunkCoordsAbs = _pendingChunkList[randomCoords];
            _pendingChunkSet.Remove(chunkCoordsAbs);
            _pendingChunkList.RemoveAt(randomCoords);
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
            lock (_workedChunks)
                _workedChunks.Add(chunkCoordsAbs);
            Task task = Task.Run(() => LoadChunkTaskMethod(chunkCoordsAbs, regionWrapper));
            _workedChunkTasks.Add(task);
            _viewport.NotifyPropertyChanged(nameof(ViewportControlVM.ChunkRegionManagerWorkedChunkCount));
        }
    }

    private void LoadChunkTaskMethod(Coords2 chunkCoordsAbs, RegionWrapper regionWrapper)
    {
        // locking is unneccessary because chunk may not be visible anymore
        // at any point in time after this statement, so its irrelevant
        if (!_visibleChunkRange.IsInside(chunkCoordsAbs))
        {
            onExit();
            return;
        }
        Chunk chunk = regionWrapper.GetChunk(chunkCoordsAbs, false);
        ChunkWrapper chunkWrapper = new(chunk);
        int renderedHeightLimit = _viewport.HeightLimit;
        chunk.GetHighestBlock(chunkWrapper.HighestBlocks, heightLimit: renderedHeightLimit);
        regionWrapper.BlitChunkImage(chunkWrapper.ChunkCoordsRel, chunkWrapper.HighestBlocks);

        PostMessage(() => LoadChunk(chunkWrapper), false);
        onExit();

        // if this method wants to return (or aborting), we have to remove the chunk coordinate
        // this thread is working on in the list and let chunk region manager loop know that we are done.
        void onExit()
        {
            lock (_workedChunks)
                _workedChunks.Remove(chunkCoordsAbs);
            _viewport.NotifyPropertyChanged(nameof(ViewportControlVM.ChunkRegionManagerWorkedChunkCount));
            // Let chunk region manager thread know that it has to process/check for more pending chunks.
            // Post LoadChunkTaskSpawnerMethod if it doesnt exist yet in msg queue.
            // This is to prevent spamming it to the message queue and chunkregionmanager
            // can spawn multiple load chunk task in single call, avoiding redundant multiple calls
            lock (_needRespawnChunkTasksLock)
                _needRespawnChunkTasks = true;
            _messageEvent.Set();
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

    public void OnSavegameLoadClosed()
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

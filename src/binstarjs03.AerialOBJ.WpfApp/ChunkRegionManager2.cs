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
    // twelve should be more than enough to avoid collection resizing,
    // unless for huge monitor resolution at zoom level 0 of course
    private const int s_regionBufferSize = 15;
    private const int s_chunkBufferSize = s_regionBufferSize * Region.TotalChunkCount;

    private readonly ViewportControlVM _viewport;
    private readonly AutoResetEvent _messageEvent = new(initialState: false);

    // we communicate between chunk worker thread and other threads using delegate message
    private readonly Queue<Action> _messageQueue = new(30);
    private readonly Thread _mainLoopThread;

    private readonly Random _rng = new();

    private readonly Dictionary<Coords2, RegionWrapper> _loadedRegions = new(s_regionBufferSize);
    private readonly Dictionary<Coords2, RegionWrapper> _loadedRegionsBuffer = new(s_regionBufferSize);
    private readonly Dictionary<Coords2, Region> _regionCache = new(s_regionBufferSize);
    private readonly List<Coords2> _pendingRegionList = new(s_regionBufferSize);
    private readonly object _workedLoadedRegionLock = new();
    private Coords2? _workedRegion = null;
    private Task _workedRegionTask;

    private readonly Dictionary<Coords2, ChunkWrapper> _loadedChunks = new(s_regionBufferSize * Region.TotalChunkCount);
    private readonly HashSet<Coords2> _pendingChunkSet = new(s_chunkBufferSize);
    private readonly List<Coords2> _pendingChunkList = new(s_chunkBufferSize);
    private readonly List<Coords2> _workedChunks = new(Environment.ProcessorCount);
    private readonly List<Task> _workedChunkTasks = new(Environment.ProcessorCount);

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


    public ChunkRegionManager2(ViewportControlVM viewport)
    {
        _viewport = viewport;
        _mainLoopThread = new Thread(MainLoop)
        {
            Name = $"{nameof(ChunkRegionManager)} Thread",
            IsBackground = true,
        };
        _workedRegionTask = new Task(LoadRegionTaskMethod);
        _mainLoopThread.Start();
    }

    /// <summary>
    /// <see cref="ChunkRegionManager2"/> Thread Main Loop procedure
    /// </summary>
    private void MainLoop()
    {
        while (true)
        {
            _messageEvent.WaitOne();
            ProcessMessage();
        }
    }

    public void PostMessage(Action msg)
    {
        lock (_messageQueue)
            _messageQueue.Enqueue(msg);
        _messageEvent.Set();
    }

    private void ProcessMessage()
    {
        // Process all messages at once
        while (true)
        {
            Action msg;
            // lock very short in case message needs long time to process
            lock (_messageQueue)
            {
                if (_messageQueue.Count == 0)
                    return;
                msg = _messageQueue.Dequeue();
            }
            msg();
        }
    }

    public void Update()
    {
        if (App.CurrentCast.Properties.SessionInfo is null)
            return;
        _displayedHeightLimit = _viewport.ViewportHeightLimit;
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
            //LoadUnloadChunks();
        }
        UpdateRegionImageTransformation();
    }

    /// <returns>true if chunk range changed</returns>
    private bool RecalculateVisibleChunkRange()
    {
        ViewportControlVM v = _viewport;
        PointF2 viewportCameraPos;
        PointF2 viewportChunkCanvasCenter;
        int viewportPixelPerChunk;

        lock (_viewport)
        {
            viewportCameraPos = v.ViewportCameraPos;
            viewportChunkCanvasCenter = v.ViewportChunkCanvasCenter;
            viewportPixelPerChunk = v.ViewportPixelPerChunk;
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

    #region Region Load-Unload Procedure

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
        if (_workedRegionTask.Status != TaskStatus.Running)
            _workedRegionTask = Task.Run(LoadRegionTaskMethod);
        _viewport.NotifyPropertyChanged(nameof(ViewportControlVM.ChunkRegionManagerPendingRegionCount));
    }

    private void LoadRegionTaskMethod()
    {
        // Load all regions until pending list exhausted
        while (true)
        {
            Coords2 regionCoords;
            // pretty long lock, but i do hope it does not contend the main loop
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
                continue;
            }
            //    _regionCache.Add(regionCoords, regionFromDisk);
            region = regionFromDisk;
            //}
            RegionWrapper regionWrapper = new(region, _viewport);
            regionWrapper.SetRandomImage();
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
                || App.CurrentCast.Properties.SessionInfo is null)
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

    // If you ask why don't we directly reference System.Windows.Controls.Image object,
    // well that's because we are executing on worker thread, and worker thread
    // cannot access any dispatcher object (which Image object is)
    private void ShowRegionImage(RegionImage regionImage)
    {
        App.InvokeDispatcher(
            () => _viewport.Control.ChunkCanvas.Children.Add(regionImage.Image),
            DispatcherPriority.Render,
            DispatcherSynchronization.Asynchronous);
    }

    private void RemoveOrHideRegionImage(RegionImage regionImage)
    {
        App.InvokeDispatcher(
            () => _viewport.Control.ChunkCanvas.Children.Remove(regionImage.Image),
            DispatcherPriority.Render,
            DispatcherSynchronization.Asynchronous);
    }

    private void OnRegionLoadChanged()
    {
        _viewport.NotifyPropertyChanged(nameof(ViewportControlVM.ChunkRegionManagerLoadedRegionCount));
    }

    #endregion Region Load-Unload Procedure

    private void LoadUnloadChunks()
    {
        // perform boundary range checking for chunks outside display frame
        //foreach (Coords2 chunkCoordsAbs in _loadedChunks.Keys)
        //{
        //    if (_visibleChunkRange.IsInside(chunkCoordsAbs))
        //        continue;
        //    ChunkWrapper chunkWrapper = _loadedChunks[chunkCoordsAbs];
        //    UnloadChunk(chunkWrapper);
        //}

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
    }

    private void RedrawRegionImages()
    {
        lock (_workedLoadedRegionLock)
            App.InvokeDispatcher(method, DispatcherPriority.Render, DispatcherSynchronization.Synchronous);
        void method()
        {
            foreach ((Coords2 regionCoords, RegionWrapper regionWrapper) in _loadedRegions)
                if (_visibleRegionRange.IsInside(regionCoords))
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
        PostMessage(method);
        void method()
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
}

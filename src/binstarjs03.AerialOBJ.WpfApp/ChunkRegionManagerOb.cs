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
/*
public class ChunkRegionManager
{
    // twelve should be more than enough to avoid collection resizing,
    // unless for huge monitor resolution at zoom level 0 of course
    private const int s_regionBufferSize = 15;
    private const int s_chunkBufferSize = s_regionBufferSize * Region.TotalChunkCount;

    private readonly ViewportControlVM _viewport;
    private readonly AutoResetEvent _updateEvent;

    // we communicate between chunk worker thread and main loop thread using delegate queue message
    private readonly Queue<Action> _messageQueue = new(30);
    private readonly Thread _mainLoopThread;

    private readonly Dictionary<Coords2, RegionWrapper> _loadedRegions = new(s_regionBufferSize);
    private readonly Dictionary<Coords2, ChunkWrapper> _loadedChunks = new(s_regionBufferSize * Region.TotalChunkCount);

    private readonly Random _rng = new();
    private readonly HashSet<Coords2> _pendingChunkSet = new(s_chunkBufferSize);
    private readonly List<Coords2> _pendingChunkList = new(s_chunkBufferSize);
    private readonly List<Coords2> _workedChunks = new(Environment.ProcessorCount);
    private readonly List<Task> _workedChunkTasks = new(Environment.ProcessorCount);

    private CoordsRange2 _visibleRegionRange = new();
    private CoordsRange2 _visibleChunkRange = new();
    private int _displayedHeightLimit;

    public ChunkRegionManager(ViewportControlVM viewport, AutoResetEvent updateEvent)
    {
        _viewport = viewport;
        _updateEvent = updateEvent;
        _mainLoopThread = new Thread(MainLoop)
        {
            Name = $"{nameof(ChunkRegionManager)} Thread",
            IsBackground = true,
        };
        _mainLoopThread.Start();
    }

    // public accessors
    public ViewportControlVM Viewport => _viewport;
    public CoordsRange2 VisibleRegionRange => _visibleRegionRange;
    public CoordsRange2 VisibleChunkRange => _visibleChunkRange;
    public int VisibleRegionCount => _visibleRegionRange.Sum;
    public int VisibleChunkCount => _visibleChunkRange.Sum;
    public int LoadedRegionCount => _loadedRegions.Count;
    public int LoadedChunkCount => _loadedChunks.Count;
    public int PendingChunkCount => _pendingChunkList.Count;
    public int WorkedChunkCount => _workedChunks.Count;

    private void MainLoop()
    {
        TimeSpan redrawLatency = TimeSpan.FromMilliseconds(30);
        int lowLatencyMultiplier = 1000; // 300 times faster than high latency
        TimeSpan highLatencyPolling = TimeSpan.FromMilliseconds(100);
        TimeSpan lowLatencyPolling = highLatencyPolling / lowLatencyMultiplier;
        while (true)
        {
            if (_pendingChunkList.Count > 0)
            {
                // Low latency polling if there is pending chunk.
                // We want to dispatch pending chunks and process message
                // as fast as possible to keep all of our CPU cores busy
                DateTime startForLoop = DateTime.Now;
                DateTime endForLoop = startForLoop.AddMilliseconds(redrawLatency.Milliseconds);
                // keep spinning through the loop until 30 ms passed since
                // we want to redraw region image only after 30 ms
                for (; DateTime.Now < endForLoop;)
                {
                    if (_updateEvent.WaitOne(lowLatencyPolling))
                        Update();
                    processMessage();
                    DispatchPendingChunks();
                }
                App.CurrentCast.Dispatcher.BeginInvoke(RedrawRegionImages, DispatcherPriority.Render);
                _viewport.NotifyPropertyChanged(nameof(ViewportControlVM.ChunkRegionManagerPendingChunkCount));
                _viewport.NotifyPropertyChanged(nameof(ViewportControlVM.ChunkRegionManagerWorkedChunkCount));
            }

            else
            {
                // High latency polling if there is nothing to do
                if (_updateEvent.WaitOne(highLatencyPolling))
                    Update();
                processMessage();
                App.CurrentCast.Dispatcher.BeginInvoke(RedrawRegionImages, DispatcherPriority.ContextIdle);
            }
        }

        void processMessage()
        {
            // Process all messages at once
            lock (_messageQueue)
                while (_messageQueue.TryDequeue(out Action? work))
                    work();
        }
    }

    public void Update()
    {
        if (App.CurrentCast.Properties.SessionInfo is null)
            return;
        _displayedHeightLimit = _viewport.HeightLimit;
        RecalculateVisibleChunkRange(out bool chunkRangeChanged);
        if (chunkRangeChanged)
        {
            _viewport.NotifyPropertyChanged(nameof(ViewportControlVM.ChunkRegionManagerVisibleChunkCount));
            _viewport.NotifyPropertyChanged(nameof(ViewportControlVM.ChunkRegionManagerVisibleChunkRangeXStringized));
            _viewport.NotifyPropertyChanged(nameof(ViewportControlVM.ChunkRegionManagerVisibleChunkRangeZStringized));
            RecalculateVisibleRegionRange(out bool regionRangeChanged);
            if (regionRangeChanged)
            {
                _viewport.NotifyPropertyChanged(nameof(ViewportControlVM.ChunkRegionManagerVisibleRegionCount));
                _viewport.NotifyPropertyChanged(nameof(ViewportControlVM.ChunkRegionManagerVisibleRegionRangeXStringized));
                _viewport.NotifyPropertyChanged(nameof(ViewportControlVM.ChunkRegionManagerVisibleRegionRangeZStringized));
                LoadUnloadRegions();
            }
            LoadUnloadChunks();
        }
        App.CurrentCast.Dispatcher.BeginInvoke(UpdateRegionImageTransformation, DispatcherPriority.Render);
    }

    private void RecalculateVisibleChunkRange(out bool chunkRangeChanged)
    {
        ViewportControlVM v = _viewport;
        PointF2 viewportCameraPos;
        PointF2 viewportChunkCanvasCenter;
        int viewportPixelPerChunk;

        lock (_viewport)
        {
            viewportCameraPos = v.CameraPos;
            viewportChunkCanvasCenter = v.ChunkCanvasCenter;
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
            chunkRangeChanged = false;
        else
        {
            chunkRangeChanged = true;
            _visibleChunkRange = newVisibleChunkRange;
        }
    }

    private void RecalculateVisibleRegionRange(out bool regionRangeChanged)
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
            regionRangeChanged = false;
        else
        {
            regionRangeChanged = true;
            _visibleRegionRange = newVisibleRegionRange;
        }
    }

    private void LoadUnloadRegions()
    {
        // perform boundary range checking for regions outside display frame
        // unload them if outside
        foreach (Coords2 regionCoords in _loadedRegions.Keys)
        {
            if (_visibleRegionRange.IsInside(regionCoords))
                continue;
            RegionWrapper regionWrapper = _loadedRegions[regionCoords];
            UnloadRegion(regionWrapper);
        }

        // perform sweep checking from min to max range for visible regions
        // read region files in all visible range and load them if not loaded yet
        for (int x = _visibleRegionRange.XRange.Min; x <= _visibleRegionRange.XRange.Max; x++)
            for (int z = _visibleRegionRange.ZRange.Min; z <= _visibleRegionRange.ZRange.Max; z++)
            {
                Coords2 regionCoords = new(x, z);
                if (_loadedRegions.ContainsKey(regionCoords))
                    continue;
                Region? region = IOService.ReadRegionFile(regionCoords, out Exception? e);

                // cancel allocation if we can't get the region at specified coords
                // (corrupted, file not exist or not generated yet etc)
                if (region is null)
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
                RegionWrapper regionWrapper = new(region, _viewport);
                LoadRegion(regionWrapper);
            }
        _viewport.NotifyPropertyChanged(nameof(ViewportControlVM.ChunkRegionManagerPendingChunkCount));
    }

    private void LoadRegion(RegionWrapper regionWrapper)
    {
        // cancel region loading if specified region is outside view screen
        if (!_visibleRegionRange.IsInside(regionWrapper.RegionCoords)
            || _loadedRegions.ContainsKey(regionWrapper.RegionCoords)
            || App.CurrentCast.Properties.SessionInfo is null)
            return;
        lock (_loadedRegions)
            _loadedRegions.Add(regionWrapper.RegionCoords, regionWrapper);
        App.CurrentCast.Dispatcher.BeginInvoke(
            () => _viewport.Control.ChunkCanvas.Children.Add(regionWrapper.RegionImage.Image),
            DispatcherPriority.Render);
        OnRegionLoadChanged();
    }

    private void UnloadRegion(RegionWrapper regionWrapper)
    {
        lock (_loadedRegions)
            _loadedRegions.Remove(regionWrapper.RegionCoords);
        App.CurrentCast.Dispatcher.BeginInvoke(
            () => _viewport.Control.ChunkCanvas.Children.Remove(regionWrapper.RegionImage.Image),
            DispatcherPriority.Render);
        OnRegionLoadChanged();
    }

    private void OnRegionLoadChanged()
    {
        _viewport.NotifyPropertyChanged(nameof(ViewportControlVM.ChunkRegionManagerLoadedRegionCount));
    }

    private void LoadUnloadChunks()
    {
        // perform boundary range checking for chunks outside display frame
        foreach (Coords2 chunkCoordsAbs in _loadedChunks.Keys)
        {
            if (_visibleChunkRange.IsInside(chunkCoordsAbs))
                continue;
            ChunkWrapper chunkWrapper = _loadedChunks[chunkCoordsAbs];
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
    }

    private void DispatchPendingChunks()
    {
        // TODO set max chunk worker thread count according to user setting chunk threads!
        _workedChunkTasks.RemoveAll(task => task.IsCompleted);
        while (_workedChunkTasks.Count < Environment.ProcessorCount)
        {
            Coords2 chunkCoordsAbs;
            if (_pendingChunkSet.Count == 0)
                return;
            int index = _rng.Next(0, _pendingChunkSet.Count);
            chunkCoordsAbs = _pendingChunkList[index];
            _pendingChunkSet.Remove(chunkCoordsAbs);
            _pendingChunkList.RemoveAt(index);

            // Check if region is loaded. Theoretically, all pending chunks have
            // their underlying regions but this is added for extra safety.
            // Simply don't do anything if the region for particular chunk
            // is not loaded (e.g culled or whatnot)

            RegionWrapper? regionWrapper = GetRegionWrapper(chunkCoordsAbs);
            Coords2 chunkCoordsRel = Region.ConvertChunkCoordsAbsToRel(chunkCoordsAbs);
            if (regionWrapper is not null && regionWrapper.HasChunkGenerated(chunkCoordsRel))
            {
                _workedChunks.Add(chunkCoordsAbs);
                Task task = Task.Run(() => DispatchPendingChunk(chunkCoordsAbs));
                _workedChunkTasks.Add(task);
            }
        }
    }

    private void DispatchPendingChunk(Coords2 chunkCoordsAbs)
    {
        // locking is unneccessary because chunk may not be visible anymore
        // at any point in time after this statement, so its irrelevant
        if (!_visibleChunkRange.IsInside(chunkCoordsAbs))
        {
            onExit();
            return;
        }
        RegionWrapper? regionWrapper = GetRegionWrapper(chunkCoordsAbs);
        if (regionWrapper is null)
        {
            onExit();
            return;
        }
        Chunk chunk = regionWrapper.GetChunk(chunkCoordsAbs, relative: false);
        ChunkWrapper chunkWrapper = new(chunk);
        int renderedHeightLimit = _viewport.HeightLimit;
        chunk.GetHighestBlock(chunkWrapper.HighestBlocks, heightLimit: renderedHeightLimit);
        regionWrapper.BlitChunkImage(chunkWrapper.ChunkCoordsRel, chunkWrapper.HighestBlocks);

        onExit();
        lock (_messageQueue)
            _messageQueue.Enqueue(() => LoadChunk(chunkWrapper));

        // if this method wants to return (or aborting), we have to remove the chunk coordinate
        // this thread is working on in the list and let background loop know that we are done
        void onExit()
        {
            lock (_messageQueue)
                _messageQueue.Enqueue(() =>
                {
                    _workedChunks.Remove(chunkCoordsAbs);
                    _viewport.NotifyPropertyChanged(nameof(ViewportControlVM.ChunkRegionManagerWorkedChunkCount));
                });
        }
    }

    private RegionWrapper? GetRegionWrapper(Coords2 chunkCoordsAbs)
    {
        Coords2 regionCoords = Region.GetRegionCoordsFromChunkCoordsAbs(chunkCoordsAbs);
        if (_loadedRegions.ContainsKey(regionCoords))
            return _loadedRegions[regionCoords];
        return null;
    }

    private void LoadChunk(ChunkWrapper chunkWrapper)
    {
        if (!_visibleChunkRange.IsInside(chunkWrapper.ChunkCoordsAbs)
            || _loadedChunks.ContainsKey(chunkWrapper.ChunkCoordsAbs)
            || App.CurrentCast.Properties.SessionInfo is null)
            return;
        _loadedChunks.Add(chunkWrapper.ChunkCoordsAbs, chunkWrapper);
        OnChunkLoadChanged();
    }

    public void UnloadChunk(ChunkWrapper chunkWrapper)
    {
        if (!_loadedChunks.ContainsKey(chunkWrapper.ChunkCoordsAbs))
            return;
        _loadedChunks.Remove(chunkWrapper.ChunkCoordsAbs);
        RegionWrapper? regionWrapper = GetRegionWrapper(chunkWrapper.ChunkCoordsAbs);
        if (regionWrapper is not null)
            regionWrapper.EraseChunkImage(chunkWrapper.ChunkCoordsRel);
        OnChunkLoadChanged();
    }

    private void OnChunkLoadChanged()
    {
        _viewport.NotifyPropertyChanged(nameof(ViewportControlVM.ChunkRegionManagerLoadedChunkCount));
    }

    private void RedrawRegionImages()
    {
        lock (_loadedRegions)
            foreach (RegionWrapper regionWrapper in _loadedRegions.Values)
            {
                RegionImage regionImage = regionWrapper.RegionImage;
                //regionImage.Lock();
                //regionImage.AddRegionDirtyRect();
                //regionImage.Unlock();
            }
    }

    private void UpdateRegionImageTransformation()
    {
        lock (_loadedRegions)
            foreach (RegionWrapper regionWrapper in _loadedRegions.Values)
                regionWrapper.UpdateImageTransformation();
    }

    public void OnSessionClosed()
    {
        _loadedRegions.Clear();
        _visibleChunkRange = new CoordsRange2();
    }
}
*/
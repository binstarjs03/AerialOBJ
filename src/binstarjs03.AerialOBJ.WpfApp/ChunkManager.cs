using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;

using binstarjs03.AerialOBJ.Core.CoordinateSystem;
using binstarjs03.AerialOBJ.Core.WorldRegion;
using binstarjs03.AerialOBJ.WpfApp.Converters;
using binstarjs03.AerialOBJ.WpfApp.Services;
using binstarjs03.AerialOBJ.WpfApp.UIElements.Controls;

using Range = binstarjs03.AerialOBJ.Core.Range;
using Region = binstarjs03.AerialOBJ.Core.WorldRegion.Region;

namespace binstarjs03.AerialOBJ.WpfApp;

public class ChunkManager
{
    private readonly ViewportControlVM _viewport;
    private readonly RegionManager _regionManager = new();
    private readonly Dictionary<Coords2, ChunkWrapper> _renderedChunks = new();

    // we use HashSet to check for fast chunk coords existence checking.
    // it also guaranteed to not having duplicates
    private readonly HashSet<Coords2> _pendingChunkSet = new(7000);

    // we use list for fast chunk coords access.
    // it allows us to access randomly using indexing.
    private readonly List<Coords2> _pendingChunkQueue = new(7000);

    // list of chunks that is being worked on by threads
    private readonly List<Coords2> _workedChunkList = new(Environment.ProcessorCount);

    // queues for chunks that needs to be deallocated
    private readonly Queue<Coords2> _deallocatedChunkQueue = new(7000);

    // random number generator to select random pending chunk to be processed
    private readonly Random _rng = new();

    private int _runningChunkWorkerThreadCount = 0;
    private int _displayedHeightLimit;
    private CoordsRange2 _visibleChunkRange;
    private bool _needReallocate = false;

    public ChunkManager(ViewportControlVM viewport)
    {
        _viewport = viewport;
        RunPendingChunkCheckerThread();

    }

    // public accessors
    public ViewportControlVM Viewport => _viewport;
    public RegionManager RegionManager => _regionManager;
    public CoordsRange2 VisibleChunkRange => _visibleChunkRange;
    public CoordsRange2 VisibleRegionRange => GetVisibleRegionRange();
    public int VisibleChunkCount => (_visibleChunkRange.XRange.Max - _visibleChunkRange.XRange.Min + 1)
                                  * (_visibleChunkRange.ZRange.Max - _visibleChunkRange.ZRange.Min + 1);
    public int RenderedChunkCount => _viewport.Control.ChunkCanvas.Children.Count;
    public int PendingChunkCount => _pendingChunkQueue.Count;
    public int WorkedChunkCount => _workedChunkList.Count;

    private CoordsRange2 GetVisibleRegionRange()
    {
        CoordsRange2 vcr = _visibleChunkRange;

        int regionMinX = vcr.XRange.Min / Region.ChunkCount;
        int regionMaxX = vcr.XRange.Max / Region.ChunkCount;

        int regionMinZ = vcr.ZRange.Min / Region.ChunkCount;
        int regionMaxZ = vcr.ZRange.Max / Region.ChunkCount;

        CoordsRange2 visibleRegionRange = new(regionMinX, regionMaxX, regionMinZ, regionMaxZ);
        return visibleRegionRange;
    }

    public void Update()
    {
        if (SharedProperty.SessionInfo is null)
            return;
        UpdateVisibleChunkRange();
        if (_displayedHeightLimit != _viewport.ViewportHeightLimit)
        {
            foreach (Coords2 chunkCoordsAbs in _renderedChunks.Keys)
                RemoveRenderedChunk(chunkCoordsAbs);
            _renderedChunks.Clear();
            _pendingChunkSet.Clear();
            _pendingChunkQueue.Clear();
            _needReallocate = true;
        }
        ReallocateChunks();
        UpdateChunks();
        _displayedHeightLimit = _viewport.ViewportHeightLimit;
    }

    private void UpdateVisibleChunkRange()
    {
        ViewportControlVM v = _viewport;

        // camera chunk in here means which chunk the camera is pointing to
        // here we dont use int because floating point accuracy is crucial
        double xCameraChunk = v.ViewportCameraPos.X / Section.BlockCount;
        double zCameraChunk = v.ViewportCameraPos.Y / Section.BlockCount;

        // min/maxCanvasCenterChunk means which chunk that is visible at the edgemost of the control
        // that is measured by the length/height of the control size (which is chunk canvas)
        double minXCanvasCenterChunk = -(v.ViewportChunkCanvasCenter.X / v.ViewportPixelPerChunk);
        double maxXCanvasCenterChunk = v.ViewportChunkCanvasCenter.X / v.ViewportPixelPerChunk;

        int minX = (int)Math.Floor(Math.Round(xCameraChunk + minXCanvasCenterChunk, 3));
        int maxX = (int)Math.Floor(Math.Round(xCameraChunk + maxXCanvasCenterChunk, 3));
        Range visibleChunkXRange = new(minX, maxX);

        double minZCanvasCenterChunk = -(v.ViewportChunkCanvasCenter.Y / v.ViewportPixelPerChunk);
        double maxZCanvasCenterChunk = v.ViewportChunkCanvasCenter.Y / v.ViewportPixelPerChunk;

        int minZ = (int)Math.Floor(Math.Round(zCameraChunk + minZCanvasCenterChunk, 3));
        int maxZ = (int)Math.Floor(Math.Round(zCameraChunk + maxZCanvasCenterChunk, 3));
        Range visibleChunkZRange = new(minZ, maxZ);

        CoordsRange2 oldVisibleChunkRange = _visibleChunkRange;
        CoordsRange2 newVisibleChunkRange = new(visibleChunkXRange, visibleChunkZRange);
        if (newVisibleChunkRange == oldVisibleChunkRange)
            return;
        _visibleChunkRange = newVisibleChunkRange;
        _regionManager.Update(newVisibleChunkRange);
        _needReallocate = true;

        string[] propertyNames = new string[]
        {
            nameof(v.ChunkManagerVisibleChunkRangeXBinding),
            nameof(v.ChunkManagerVisibleChunkRangeZBinding),
            nameof(v.ChunkManagerVisibleRegionRangeXBinding),
            nameof(v.ChunkManagerVisibleRegionRangeZBinding),
        };
        v.NotifyPropertyChanged(propertyNames);
    }

    private void ReallocateChunks()
    {
        if (_needReallocate)
        {
            // perform boundary checking for chunks outside display frame
            foreach (Coords2 chunkCoordsAbs in _renderedChunks.Keys)
            {
                if (_visibleChunkRange.IsInside(chunkCoordsAbs))
                    continue;
                _deallocatedChunkQueue.Enqueue(chunkCoordsAbs);

            }

            // remove all pending chunk queue that is no longer visible
            _pendingChunkSet.RemoveWhere(chunkCoordsAbs => !_visibleChunkRange.IsInside(chunkCoordsAbs));
            _pendingChunkQueue.RemoveAll(chunkCoordsAbs => !_visibleChunkRange.IsInside(chunkCoordsAbs));

            // perform sweep checking for chunks inside display frame
            for (int x = _visibleChunkRange.XRange.Min; x <= _visibleChunkRange.XRange.Max; x++)
                for (int z = _visibleChunkRange.ZRange.Min; z <= _visibleChunkRange.ZRange.Max; z++)
                {
                    Coords2 chunkCoordsAbs = new(x, z);
                    if (_renderedChunks.ContainsKey(chunkCoordsAbs))
                        continue;
                    //if (_pendingChunkQueue.Contains(chunkCoordsAbs))
                    //    continue;
                    if (_pendingChunkSet.Contains(chunkCoordsAbs))
                        continue;
                    if (_workedChunkList.Contains(chunkCoordsAbs))
                        continue;
                    _pendingChunkSet.Add(chunkCoordsAbs);
                    _pendingChunkQueue.Add(chunkCoordsAbs);
                }

            // deallocate
            while (_deallocatedChunkQueue.TryDequeue(out Coords2 chunkCoordsAbs))
            {
                RemoveRenderedChunk(chunkCoordsAbs);
            }
            InitiateChunkAllocation();
            _viewport.NotifyPropertyChanged(nameof(_viewport.ChunkManagerPendingChunkCount));
            _viewport.NotifyPropertyChanged(nameof(_viewport.ChunkManagerVisibleChunkCount));
            _needReallocate = false;
        }
    }

    public void RunPendingChunkCheckerThread()
    {
        //return;
        Thread pendingChunkCheckerThread = new(() => PendingChunkCheckerLoopAsync())
        {
            Name = "PendingChunkChecker",
            IsBackground = true,
        };
        pendingChunkCheckerThread.Start();
    }

    private void PendingChunkCheckerLoopAsync()
    {
        Thread.Sleep(1000);
        // TODO this log message displayed after initialization completed. we want to send any initialization
        // log message to appear before it tells us that initialization is completed
        App.BeginInvokeDispatcher(
            () => LogService.Log($"Background pending chunk checker thread successfully started", useSeparator: true),
            DispatcherPriority.Normal);
        try
        {
            while (true)
            {
                App.BeginInvokeDispatcher(
                    () => InitiateChunkAllocation(),
                    DispatcherPriority.Send);
                Thread.Sleep(500);
            }
        }
        catch
        {
            // guard this thread against any exceptions, restart if so
            LogService.LogError($"{Thread.CurrentThread.Name} crashed! restarting...", useSeparator: true);
            RunPendingChunkCheckerThread();
            return;
        }
    }

    // run this method in UI thread, else we have to deal with lock and such
    private void InitiateChunkAllocation()
    {
        // TODO set max chunk worker thread count according to user setting chunk threads!
        while (_runningChunkWorkerThreadCount < Environment.ProcessorCount)
        {
            if (_pendingChunkSet.Count == 0)
                break;
            int index = _rng.Next(0, _pendingChunkSet.Count);
            Coords2 chunkCoordsAbs = _pendingChunkQueue[index];
            _pendingChunkSet.Remove(chunkCoordsAbs);
            _pendingChunkQueue.RemoveAt(index);
            if (_regionManager.CanGetChunk(chunkCoordsAbs))
            {
                _workedChunkList.Add(chunkCoordsAbs);
                Task.Run(() => AllocateChunkAsync(chunkCoordsAbs));
                _runningChunkWorkerThreadCount++;
            }
        }
        _viewport.NotifyPropertyChanged(nameof(_viewport.ChunkManagerPendingChunkCount));
        _viewport.NotifyPropertyChanged(nameof(_viewport.ChunkManagerWorkedChunkCount));
    }

    private void AllocateChunkAsync(Coords2 chunkCoordsAbs)
    {
        // we do not lock visible chunk range because its unneccessary, furthermore
        // it will avoid stalling the UI thread when it is trying to updating it
        if (!_visibleChunkRange.IsInside(chunkCoordsAbs))
        {
            // if this method wants to return (aborting), we have to remove the chunk coordinate
            // this thread is working on in the list, and we do it on main thread using dispatcher
            App.BeginInvokeDispatcher(
                () => OnAllocateChunkAsyncExit(chunkCoordsAbs),
                DispatcherPriority.Normal);
            return;
        }

        // since we are not locking region manager,
        // chunk may not be gettable anymore at any point of time
        Chunk? chunk = _regionManager.GetChunk(chunkCoordsAbs);
        if (chunk is null)
        {
            App.BeginInvokeDispatcher(
                () => OnAllocateChunkAsyncExit(chunkCoordsAbs),
                DispatcherPriority.Normal);
            return;
        }

        Bitmap chunkImage = new(16, 16, PixelFormat.Format32bppArgb);
        Block[,] blocks = new Block[Section.BlockCount, Section.BlockCount];

        // we also do not lock height limit, and if the height limit is changed
        // while this thread is rendering on different height limit,
        // it will not be added to viewport (see AllocateChunkSynchronized method)
        int renderedHeight = _viewport.ViewportHeightLimit;
        chunk.GetBlockTopmost(blocks, heightLimit: renderedHeight);
        for (int x = 0; x < Section.BlockCount; x++)
        {
            for (int z = 0; z < Section.BlockCount; z++)
            {
                chunkImage.SetPixel(x, z, BlockToColor2.Convert(blocks[x, z]));
            }
        }
        MemoryStream chunkImageStream = new();
        chunkImage.Save(chunkImageStream, ImageFormat.Bmp);

        App.BeginInvokeDispatcher(
            () => OnAllocateChunkAsyncExit(chunkCoordsAbs),
            DispatcherPriority.Background);

        App.BeginInvokeDispatcher(
            () => AllocateChunkSynchronized(chunkCoordsAbs, chunkImageStream, renderedHeight),
            DispatcherPriority.Background);
    }

    private void OnAllocateChunkAsyncExit(Coords2 chunkCoordsAbs)
    {
        _runningChunkWorkerThreadCount--;
        _workedChunkList.Remove(chunkCoordsAbs);
        _viewport.NotifyPropertyChanged(nameof(_viewport.ChunkManagerWorkedChunkCount));
        // rerun chunk allocation to see if there
        // is still pending chunk to be processed
        InitiateChunkAllocation();
    }

    private void AllocateChunkSynchronized(Coords2 chunkCoordsAbs, MemoryStream chunkImageStream, int renderedHeight)
    {
        // discard this chunk if it is no longer visible
        if (!_visibleChunkRange.IsInside(chunkCoordsAbs))
            return;

        // discard this chunk if it was rendered on different height limit
        // than the display is currently displaying
        if (renderedHeight != _displayedHeightLimit)
            return;
        ChunkWrapper chunkWrapper = new(chunkCoordsAbs, this, chunkImageStream);
        AddRenderedChunk(chunkWrapper);
    }

    private void AddRenderedChunk(ChunkWrapper chunkWrapper)
    {
        if (!_visibleChunkRange.IsInside(chunkWrapper.ChunkCoordsAbs))
            return;
        if (_renderedChunks.ContainsKey(chunkWrapper.ChunkCoordsAbs))
            return;
        _renderedChunks.Add(chunkWrapper.ChunkCoordsAbs, chunkWrapper);
        _viewport.Control.ChunkCanvas.Children.Add(chunkWrapper.ChunkImage);
        _viewport.NotifyPropertyChanged(nameof(_viewport.ChunkManagerRenderedChunkCount));
    }

    public void RemoveRenderedChunk(Coords2 chunkCoordsAbs)
    {
        if (!_renderedChunks.ContainsKey(chunkCoordsAbs))
            return;
        ChunkWrapper chunkWrapper = _renderedChunks[chunkCoordsAbs];
        _renderedChunks.Remove(chunkWrapper.ChunkCoordsAbs);
        _viewport.Control.ChunkCanvas.Children.Remove(chunkWrapper.ChunkImage);
        _viewport.NotifyPropertyChanged(nameof(_viewport.ChunkManagerRenderedChunkCount));
        chunkWrapper.Deallocate();
    }

    private void UpdateChunks()
    {
        foreach (ChunkWrapper chunk in _renderedChunks.Values)
            chunk.Update();
    }

    public void OnSessionClosed()
    {
        foreach (ChunkWrapper chunk in _renderedChunks.Values)
            chunk.Deallocate();
        _renderedChunks.Clear();
        // reset visible chunk range to zero, this will ensure next
        // update will set visible chunk range to different value,
        // setting needreallocate to true, in turns allowing chunk reallocation
        _visibleChunkRange = new CoordsRange2();
        _regionManager.OnSessionClosed();
    }
}
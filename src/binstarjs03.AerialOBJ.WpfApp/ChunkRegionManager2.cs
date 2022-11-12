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
    private const int s_framesPerSecond = 60;
    private const int s_redrawFrequency = 1000 / s_framesPerSecond; // unit: miliseconds

    private readonly ViewportControlVM _viewport;

    private readonly AutoResetEvent _messageEvent = new(false);
    private readonly Queue<Action> _messageQueue = new(50);

    private readonly Dictionary<Coords2, RegionWrapper> _loadedRegions = new(s_regionBufferSize);
    private readonly List<Coords2> _pendingRegionList = new(s_regionBufferSize);
    private Coords2? _workedRegion = null;

    private readonly Dictionary<Coords2, ChunkWrapper> _loadedChunks = new(s_regionBufferSize * Region.TotalChunkCount);
    private readonly HashSet<Coords2> _pendingChunkSet = new(s_chunkBufferSize);
    private readonly List<Coords2> _pendingChunkList = new(s_chunkBufferSize);
    private readonly object _pendingChunkLock = new();
    private Task _loadRegionTask;

    private CoordsRange2 _visibleRegionRange = new();
    private CoordsRange2 _visibleChunkRange = new();
    private readonly Random _rng = new();

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
    public int WorkedChunkCount => 0;

    public ChunkRegionManager2(ViewportControlVM viewport)
    {
        _viewport = viewport;
        _loadRegionTask = new Task(() => { });
        new Task(MessageLoop, TaskCreationOptions.LongRunning).Start();
        new Task(RedrawLoop, TaskCreationOptions.LongRunning).Start();
    }

    private void MessageLoop()
    {
        while (true)
        {
            int messageCount;
            lock (_messageQueue)
                messageCount = _messageQueue.Count;
            if (messageCount == 0)
                _messageEvent.WaitOne();
            ProcessMessage();
        }
    }

    private async void RedrawLoop()
    {
        PeriodicTimer redrawTimer = new(TimeSpan.FromMilliseconds(s_redrawFrequency));
        while (await redrawTimer.WaitForNextTickAsync())
            PostMessage(RedrawRegionImages);
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

    public void PostMessage(Action message)
    {
        lock (_messageQueue)
            _messageQueue.Enqueue(message);
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
            //LoadUnloadChunks();
        }
        //UpdateRegionImageTransformation();
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
        _pendingRegionList.RemoveAll(regionCoords => _visibleRegionRange.IsOutside(regionCoords));

        // perform sweep checking from min to max range for visible regions
        // add them to pending list if not loaded yet, not in the list, and is not worked yet
        for (int x = _visibleRegionRange.XRange.Min; x <= _visibleRegionRange.XRange.Max; x++)
            for (int z = _visibleRegionRange.ZRange.Min; z <= _visibleRegionRange.ZRange.Max; z++)
            {
                // TODO IOService should cache which region coords has region file,
                // so IO operation does not block execution
                Coords2 regionCoords = new(x, z);
                lock (_loadedRegions)
                    if (_loadedRegions.ContainsKey(regionCoords)
                        || _pendingRegionList.Contains(regionCoords)
                        || !IOService.HasRegionFile(regionCoords)
                        || _workedRegion == regionCoords)
                        continue;
                _pendingRegionList.Add(regionCoords);
            }
        LoadRegionAsync();
    }

    private async void LoadRegionAsync()
    {
        while (_pendingRegionList.Count > 0)
        {
            int randomIndex = _rng.Next(0, _pendingRegionList.Count);
            Coords2 regionCoords = _pendingRegionList[randomIndex];
            _pendingRegionList.RemoveAt(randomIndex);

            _workedRegion = regionCoords;
            _viewport.NotifyPropertyChanged(nameof(ViewportControlVM.ChunkRegionManagerWorkedRegion));
            Task<RegionWrapper?> loadRegionTask = Task.Run(() => LoadRegionTaskMethod(regionCoords));
            await loadRegionTask;
            _workedRegion = null;
            _viewport.NotifyPropertyChanged(nameof(ViewportControlVM.ChunkRegionManagerWorkedRegion));

            RegionWrapper? regionWrapper = loadRegionTask.Result;
            if (regionWrapper is null)
                continue;
            LoadRegion(regionWrapper);
        }
    }

    private RegionWrapper? LoadRegionTaskMethod(Coords2 regionCoords)
    {
        Region? region = IOService.ReadRegionFile(regionCoords, out Exception? e);
        if (region is null)
        {
            if (e is not null)
            {
                // TODO display messagebox only once and never again
                LogService.LogError($"Region {regionCoords} was skipped.");
                LogService.LogError($"Cause of exception: {e.GetType()}");
                LogService.LogError($"Exception details: {e}", useSeparator: true);
            }
            return null;
        }
        RegionWrapper regionWrapper = new(region, _viewport);
        regionWrapper.SetRandomImage();
        regionWrapper.RedrawImage();
        return regionWrapper;
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
                    //if (_visibleRegionRange.IsInside(regionCoords) && regionWrapper.NeedRedraw)
                    regionWrapper.RedrawImage();
        }
    }


    public void OnSavegameLoadClosed()
    {
        List<RegionWrapper> loadedRegions = new(_loadedRegions.Values);
        foreach (RegionWrapper region in loadedRegions)
            UnloadRegion(region);

        _loadedRegions.Clear();
        _pendingRegionList.Clear();
        _visibleRegionRange = new CoordsRange2();
        _visibleChunkRange = new CoordsRange2();
    }
}

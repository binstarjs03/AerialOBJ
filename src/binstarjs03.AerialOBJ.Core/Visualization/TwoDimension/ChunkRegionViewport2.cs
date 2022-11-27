using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

using binstarjs03.AerialOBJ.Core.MinecraftWorld;
using binstarjs03.AerialOBJ.Core.Primitives;
using binstarjs03.AerialOBJ.Core.Threading.MessageDispatching;

namespace binstarjs03.AerialOBJ.Core.Visualization.TwoDimension;

public delegate Region? ReadRegionFileHandler(Point2Z<int> regionCoords, out Exception? e);
public delegate IRegionModel RegionModelFactoryHandler(Region region);
public class ChunkRegionViewport2 : IChunkRegionViewport2, ILogging, IDispatcherObject, INotifyPropertyChanged
{
    private const int s_regionBufferSize = 20;
    private const int s_chunkBufferSize = s_regionBufferSize * Region.TotalChunkCount;
    private const int s_framesPerSecond = 60;
    private const int s_redrawFrequency = 1000 / s_framesPerSecond; // unit: miliseconds

    // threading fields
    private readonly AutoResetEvent _redrawCompletedEvent = new(false);
    private CancellationTokenSource _cts = new();

    // viewport fields
    private readonly Random _rng = new();
    private Point2Z<float> _cameraPos = Point2Z<float>.Zero;
    private float _zoomLevel = 1f;
    private Size<int> _screenSize;
    private int _heightLimit;

    // region fields
    private Point2ZRange<int> _visibleRegionRange;
    private readonly Dictionary<Point2Z<int>, IRegionModel> _loadedRegions = new(s_regionBufferSize);
    private readonly List<Point2Z<int>> _pendingRegionList = new(s_regionBufferSize);
    private Point2Z<int>? _workedRegion = null;
    private readonly object _workedRegionLock = new();
    private Task _workedRegionTask = new(() => { });
    private Dictionary<Point2Z<int>, Region> _regionCache = new(s_regionBufferSize);

    // chunk fields
    private Point2ZRange<int> _visibleChunkRange;
    private readonly Dictionary<Point2Z<int>, IChunkModel> _loadedChunks = new(s_chunkBufferSize);
    private readonly List<Point2Z<int>> _pendingChunkList = new(s_chunkBufferSize);
    private readonly HashSet<Point2Z<int>> _pendingChunkSet = new(s_chunkBufferSize);

    #region Properties
    public Point2Z<float> CameraPos
    {
        get => _cameraPos;
        set
        {
            if (value == _cameraPos)
                return;
            _cameraPos = value;
            Update();
            OnPropertyChanged();
        }
    }
    public float ZoomLevel
    {
        get => _zoomLevel;
        set
        {
            if (value == _zoomLevel)
                return;
            if (value == 0)
                throw new ArgumentException("Zoom level cannot be zero", nameof(ZoomLevel));
            _zoomLevel = value;
            Update();
            OnPropertyChanged();
            OnPropertyChanged(nameof(PixelPerBlock));
            OnPropertyChanged(nameof(PixelPerChunk));
            OnPropertyChanged(nameof(PixelPerRegion));
        }
    }
    public Size<int> ScreenSize
    {
        get => _screenSize;
        set
        {
            if (value != _screenSize)
            {
                _screenSize = value;
                Update();
                OnPropertyChanged();
                OnPropertyChanged(nameof(ScreenCenter));
            }
        }
    }
    public int HeightLimit
    {
        get => _heightLimit;
        set => throw new NotImplementedException();
    }
    public Point2<int> ScreenCenter => ScreenSize.GetMidPoint();

    public float PixelPerBlock => ZoomLevel;
    public float PixelPerChunk => PixelPerBlock * Section.BlockCount;
    public float PixelPerRegion => PixelPerChunk * Region.ChunkCount;


    public Point2ZRange<int> VisibleRegionRange => _visibleRegionRange;
    public int LoadedRegionCount => _loadedRegions.Count;
    public int PendingRegionCount => _pendingRegionList.Count;
    public Point2Z<int>? WorkedRegion => _workedRegion;

    public Point2ZRange<int> VisibleChunkRange => _visibleChunkRange;
    public int LoadedChunkCount => _loadedChunks.Count;
    public int PendingChunkCount => _pendingChunkList.Count;
    #endregion Properties

    #region Dependency Injection Properties
    public required IMessageDispatcher Dispatcher { get; init; }
    public required Predicate<Point2Z<int>> HasRegionFileHandler { get; init; }
    public required ReadRegionFileHandler ReadRegionFileHandler { get; init; }
    public required RegionModelFactoryHandler CreateRegionModelHandler { get; init; }
    public required Func<bool> IsSavegameLoadedHandler { get; init; }
    public required Action<IImage> LoadRegionImageHandler { get; init; }
    public required Action<IImage> UnloadRegionImageHandler { get; init; }
    public required Action<Action> InvokeUIThreadAsynchronousHandler { get; init; }
    public required Action<string> LogHandler { get; init; }
    #endregion Dependency Injection Properties

    public event Action<string>? PropertyChanged;

    public ChunkRegionViewport2(IMessageDispatcher dispatcher)
    {
        Dispatcher = dispatcher;
        dispatcher.Started += OnDispatcherStarted;
        dispatcher.Stopping += OnDispatcherStopping;
        dispatcher.Stopped += OnDispatcherStopped;
        dispatcher.Reinitialized += OnReinitializing;
    }

    private void OnDispatcherStarted(CancellationToken ct)
    {
        Task.Run(() => RedrawLoop(ct), ct);
        LogHandler("Started CRV dispatcher and redrawer loop thread");
    }

    private void OnDispatcherStopping()
    {
        _redrawCompletedEvent.Set();
    }

    private void OnDispatcherStopped()
    {
        LogHandler("Stopped CRV dispatcher and redrawer loop thread");
        OnReinitializing();
    }

    private void OnReinitializing()
    {
        CameraPos = Point2Z<float>.Zero;
        ZoomLevel = 1;
        ScreenSize = new Size<int>(0, 0);

        _visibleRegionRange = new Point2ZRange<int>(0, 0, 0, 0);
        foreach (IRegionModel regionModel in _loadedRegions.Values)
            UnloadRegionModel(regionModel);
        _pendingRegionList.Clear();
        OnPropertyChanged(nameof(VisibleRegionRange));
        OnPropertyChanged(nameof(LoadedRegionCount));
        OnPropertyChanged(nameof(PendingRegionCount));

        _visibleChunkRange = new Point2ZRange<int>(0, 0, 0, 0);
        //foreach (ChunkModel chunkModel in _loadedChunks)
        //    UnloadChunkModel(chunkModel);
        _pendingChunkList.Clear();
        _pendingChunkSet.Clear();
        OnPropertyChanged(nameof(VisibleChunkRange));
        OnPropertyChanged(nameof(LoadedChunkCount));
        OnPropertyChanged(nameof(PendingChunkCount));

        LogHandler("Successfully reinitialized all CRV states");
    }

    private void OnPropertyChanged([CallerMemberName] string propertyName = "")
    {
        PropertyChanged?.Invoke(propertyName);
    }

    public void Update()
    {
        if (RecalculateVisibleChunkRange() && RecalculateVisibleRegionRange())
            LoadUnloadRegions();
    }

    private async void RedrawLoop(CancellationToken ct)
    {
        using PeriodicTimer redrawTimer = new(TimeSpan.FromMilliseconds(s_redrawFrequency));
        while (await redrawTimer.WaitForNextTickAsync(CancellationToken.None))
            if (ct.IsCancellationRequested)
                return;
            else
                Dispatcher.InvokeAsynchronousNoDuplicate(RedrawRegionImages);
    }

    private bool RecalculateVisibleChunkRange()
    {
        // Camera chunk in here means which chunk the camera is pointing to.
        // Here we dont use int because floating point accuracy is crucial
        double xCameraChunk = CameraPos.X / Section.BlockCount;
        double zCameraChunk = CameraPos.Z / Section.BlockCount;

        // min/maxCanvasCenterChunk means which chunk that is visible at the edgemost of the control
        // that is measured by the length or height of the control size (which is chunk canvas)
        double minXCanvasCenterChunk = -(ScreenCenter.X / PixelPerChunk);
        double maxXCanvasCenterChunk = ScreenCenter.X / PixelPerChunk;
        int minX = MathUtils.Floor(xCameraChunk + minXCanvasCenterChunk);
        int maxX = MathUtils.Floor(xCameraChunk + maxXCanvasCenterChunk);
        Rangeof<int> visibleChunkXRange = new(minX, maxX);

        double minZCanvasCenterChunk = -(ScreenCenter.Y / PixelPerChunk);
        double maxZCanvasCenterChunk = ScreenCenter.Y / PixelPerChunk;
        int minZ = MathUtils.Floor(zCameraChunk + minZCanvasCenterChunk);
        int maxZ = MathUtils.Floor(zCameraChunk + maxZCanvasCenterChunk);
        Rangeof<int> visibleChunkZRange = new(minZ, maxZ);

        Point2ZRange<int> oldVisibleChunkRange = _visibleChunkRange;
        Point2ZRange<int> newVisibleChunkRange = new(visibleChunkXRange, visibleChunkZRange);
        if (newVisibleChunkRange == oldVisibleChunkRange)
            return false;
        _visibleChunkRange = newVisibleChunkRange;
        OnPropertyChanged(nameof(VisibleChunkRange));
        return true;
    }

    private bool RecalculateVisibleRegionRange()
    {
        Point2ZRange<int> vcr = _visibleChunkRange;

        // Calculating region range is easier since we only need to
        // divide the range by how many chunks in region (in single axis)

        int regionMinX = MathUtils.DivFloor(vcr.XRange.Min, Region.ChunkCount);
        int regionMaxX = MathUtils.DivFloor(vcr.XRange.Max, Region.ChunkCount);
        Rangeof<int> visibleRegionXRange = new(regionMinX, regionMaxX);

        int regionMinZ = MathUtils.DivFloor(vcr.ZRange.Min, Region.ChunkCount);
        int regionMaxZ = MathUtils.DivFloor(vcr.ZRange.Max, Region.ChunkCount);
        Rangeof<int> visibleRegionZRange = new(regionMinZ, regionMaxZ);

        Point2ZRange<int> oldVisibleRegionRange = _visibleRegionRange;
        Point2ZRange<int> newVisibleRegionRange = new(visibleRegionXRange, visibleRegionZRange);
        if (newVisibleRegionRange == oldVisibleRegionRange)
            return false;
        _visibleRegionRange = newVisibleRegionRange;
        OnPropertyChanged(nameof(VisibleRegionRange));
        return true;
    }

    private void LoadUnloadRegions()
    {
        // perform boundary range checking for regions outside display frame
        // unload them if outside
        lock (_loadedRegions)
            foreach ((Point2Z<int> regionCoords, IRegionModel regionModel) in _loadedRegions)
                if (_visibleRegionRange.IsOutside(regionCoords))
                    UnloadRegionModel(regionModel);
        OnPropertyChanged(nameof(LoadedRegionCount));

        // remove all pending region that is no longer visible
        lock (_pendingRegionList)
            _pendingRegionList.RemoveAll(_visibleRegionRange.IsOutside);

        // perform sweep checking from min to max range for visible regions
        // add them to pending list if not loaded yet, not in the list, and is not worked yet
        for (int x = _visibleRegionRange.XRange.Min; x <= _visibleRegionRange.XRange.Max; x++)
            for (int z = _visibleRegionRange.ZRange.Min; z <= _visibleRegionRange.ZRange.Max; z++)
            {
                Point2Z<int> regionCoords = new(x, z);
                lock (_loadedRegions)
                    lock (_pendingRegionList)
                        lock (_workedRegionLock)
                        {
                            if (_loadedRegions.ContainsKey(regionCoords)
                                || _pendingRegionList.Contains(regionCoords)
                                || !HasRegionFileHandler(regionCoords)
                                || _workedRegion == regionCoords)
                                continue;
                            _pendingRegionList.Add(regionCoords);
                        }
            }
        OnPropertyChanged(nameof(PendingRegionCount));
        if (_workedRegionTask.Status != TaskStatus.Running)
            _workedRegionTask = Task.Run(LoadRegionTask, _cts.Token);
    }

    private void LoadRegionTask()
    {
        while (!_cts.IsCancellationRequested)
        {
            Point2Z<int> regionCoords;
            lock (_pendingRegionList)
            {
                if (_pendingRegionList.Count == 0)
                    break;
                int randomIndex = _rng.Next(0, _pendingRegionList.Count);
                regionCoords = _pendingRegionList[randomIndex];
                _pendingRegionList.RemoveAt(randomIndex);
            }
            lock (_workedRegionLock)
                _workedRegion = regionCoords;
            OnPropertyChanged(nameof(PendingRegionCount));
            OnPropertyChanged(nameof(WorkedRegion));
            Region region;
            // Try to fetch region from cache. If miss, load it from disk
            if (_regionCache.ContainsKey(regionCoords))
                region = _regionCache[regionCoords];
            else
            {
                // Region ReadRegionFileHandler(Point2Z<int> regionCoords, out Exception? e);
                Region? diskRegion = ReadRegionFileHandler(regionCoords, out Exception? e);
                if (diskRegion is null)
                {
                    if (e is not null)
                    {
                        // TODO display messagebox only once and never again
                        string msg = $"Region {regionCoords} was skipped.\n"
                                   + $"Cause of exception: {e.GetType()}\n"
                                   + $"Exception details: {e}";
                        LogHandler(msg);
                    }
                    continue;
                }
                region = diskRegion;
                _regionCache.Add(regionCoords, region);
            }
            IRegionModel regionModel = CreateRegionModelHandler(region);
            regionModel.SetRandomImage();
            LoadRegionModel(regionModel);
        }
        lock (_workedRegionLock)
            _workedRegion = null;
        OnPropertyChanged(nameof(WorkedRegion));
    }

    private void LoadRegionModel(IRegionModel regionModel)
    {
        lock (_loadedRegions)
        {
            if (_visibleRegionRange.IsOutside(regionModel.RegionCoords)
                || _loadedRegions.ContainsKey(regionModel.RegionCoords)
                || !IsSavegameLoadedHandler())
                return;
            _loadedRegions.Add(regionModel.RegionCoords, regionModel);
        }
        InvokeUIThreadAsynchronousHandler(() => LoadRegionImageHandler(regionModel.Image));
        OnPropertyChanged(nameof(LoadedRegionCount));
    }

    private void UnloadRegionModel(IRegionModel regionModel)
    {
        lock (_loadedRegions)
            _loadedRegions.Remove(regionModel.RegionCoords);
        InvokeUIThreadAsynchronousHandler(() => UnloadRegionImageHandler(regionModel.Image));
        OnPropertyChanged(nameof(LoadedRegionCount));
    }

    private void RedrawRegionImages()
    {
        InvokeUIThreadAsynchronousHandler(RedrawRegionImagesUIThread);
        _redrawCompletedEvent.WaitOne();
    }

    private void RedrawRegionImagesUIThread()
    {
        lock (_loadedRegions)
            foreach (IRegionModel regionModel in _loadedRegions.Values)
            {
                Point2<int> imageScreenPos = CalculateRegionImageScreenPosition(regionModel.RegionCoords);
                regionModel.RedrawImage(imageScreenPos, PixelPerRegion);
            }
        _redrawCompletedEvent.Set();
    }

    private Point2<int> CalculateRegionImageScreenPosition(Point2Z<int> regionCoords)
    {
        int xWorldPos = regionCoords.X * Region.BlockCount;
        float xScaledWorldPos = xWorldPos * ZoomLevel;
        float xScaledCameraPos = -(CameraPos.X * ZoomLevel);
        int xScreenPos = MathUtils.Floor(xScaledCameraPos + xScaledWorldPos) + ScreenCenter.X;

        int zWorldPos = regionCoords.Z * Region.BlockCount;
        float zScaledWorldPos = zWorldPos * ZoomLevel;
        float zScaledCameraPos = -(CameraPos.Z * ZoomLevel);
        int yScreenPos = MathUtils.Floor(zScaledCameraPos + zScaledWorldPos) + ScreenCenter.Y;

        return new Point2<int>(xScreenPos, yScreenPos);
    }
}

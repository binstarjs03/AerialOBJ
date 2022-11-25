using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Threading;

using binstarjs03.AerialOBJ.Core;
using binstarjs03.AerialOBJ.Core.MinecraftWorld;
using binstarjs03.AerialOBJ.Core.Primitives;
using binstarjs03.AerialOBJ.WpfAppNew.Components.Interfaces;
using binstarjs03.AerialOBJ.WpfAppNew.Models;
using binstarjs03.AerialOBJ.WpfAppNew.Services;

namespace binstarjs03.AerialOBJ.WpfAppNew.Components;

public class ChunkRegionViewport : IThreadMessageReceiver
{
    public event Action<string>? PropertyChanged;
    public event RegionImageEventHandler? RegionImageLoaded;
    public event RegionImageEventHandler? RegionImageUnloaded;
    public delegate void RegionImageEventHandler(Image regionImage);

    private const int s_regionBufferSize = 20;
    private const int s_chunkBufferSize = s_regionBufferSize * Region.TotalChunkCount;
    private const int s_framesPerSecond = 60;
    private const int s_redrawFrequency = 1000 / s_framesPerSecond; // unit: miliseconds

    // thread messaging fields
    private readonly AutoResetEvent _messageEvent = new(false);
    private readonly Queue<Action> _messageQueue = new(50);

    // viewport fields
    private Point2Z<float> _cameraPos = Point2Z<float>.Zero;
    private float _zoomLevel = 1f;
    private Size<int> _screenSize;
    private readonly Random _rng = new();

    private Point2ZRange<int> _visibleRegionRange;
    private readonly Dictionary<Point2Z<int>, RegionModel> _loadedRegions = new(s_regionBufferSize);
    private readonly List<Point2Z<int>> _pendingRegionList = new(s_regionBufferSize);
    private Point2Z<int>? _workedRegion = null;
    private readonly object _workedRegionLock = new();
    private Task _workedRegionTask = new(() => { });

    private Point2ZRange<int> _visibleChunkRange;
    private readonly Dictionary<Point2Z<int>, ChunkModel> _loadedChunks = new(s_chunkBufferSize);
    private readonly List<Point2Z<int>> _pendingChunkList = new(s_chunkBufferSize);
    private readonly HashSet<Point2Z<int>> _pendingChunkSet = new(s_chunkBufferSize);

    #region Properties
    // Public readonly accessors for internal use and for UI update.
    // For consistency:
    // - Use property for CameraPos, ZoomLevel, and ScreenSize
    // - Other than those 3 main properties, use field if available,
    //   else use the properties
    public Point2Z<float> CameraPos
    {
        get => _cameraPos;
        set
        {
            if (value != _cameraPos)
            {
                _cameraPos = value;
                Update();
                OnPropertyChanged();
            }
        }
    }
    public float ZoomLevel
    {
        get => _zoomLevel;
        set
        {
            if (value != _zoomLevel)
            {
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
    }
    public float PixelPerBlock => ZoomLevel;
    public float PixelPerChunk => PixelPerBlock * Section.BlockCount;
    public float PixelPerRegion => PixelPerChunk * Region.ChunkCount;
    public Size<int> ScreenSize
    {
        get => _screenSize;
        set
        {
            if (value != _screenSize)
            {
                if (value.Width < 0 || value.Height < 0)
                    throw new ArgumentException("Screen size cannot be negative", nameof(ScreenSize));
                _screenSize = value;
                Update();
                OnPropertyChanged();
                OnPropertyChanged(nameof(ScreenCenter));
            }
        }
    }
    public Point2<int> ScreenCenter => ScreenSize.GetMidPoint();
    public int HeightLimit
    {
        get => throw new NotImplementedException();
        set => throw new NotImplementedException();
    }

    public Point2ZRange<int> VisibleRegionRange => _visibleRegionRange;
    public int LoadedRegionCount => _loadedRegions.Count;
    public int PendingRegionCount => _pendingRegionList.Count;
    public Point2Z<int>? WorkedRegion => _workedRegion;

    public Point2ZRange<int> VisibleChunkRange => _visibleChunkRange;
    public int LoadedChunkCount => _loadedChunks.Count;
    public int PendingChunkCount => _pendingChunkList.Count;
    #endregion Properties

    public ChunkRegionViewport()
    {
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
            PostMessage(RedrawRegionImages, MessageOption.NoDuplicate);
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

    public void PostMessage(Action message, MessageOption messageOption)
    {
        lock (_messageQueue)
        {
            if (messageOption == MessageOption.NoDuplicate)
            {
                if (!_messageQueue.Contains(message))
                    _messageQueue.Enqueue(message);
            }
            else
                _messageQueue.Enqueue(message);
        }
        _messageEvent.Set();
    }

    public void Update(bool forced = false)
    {
        if (RecalculateVisibleChunkRange() || forced)
        {
            if (RecalculateVisibleRegionRange() || forced)
                LoadUnloadRegions();
        }
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
            foreach ((Point2Z<int> regionCoords, RegionModel regionModel) in _loadedRegions)
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
                    lock (_workedRegionLock)
                        if (_loadedRegions.ContainsKey(regionCoords)
                            || _pendingRegionList.Contains(regionCoords)
                            || !IOService.HasRegionFile(regionCoords)
                            || _workedRegion == regionCoords)
                            continue;
                _pendingRegionList.Add(regionCoords);
            }
        OnPropertyChanged(nameof(PendingRegionCount));
        if (_workedRegionTask.Status != TaskStatus.Running)
            _workedRegionTask = Task.Run(LoadRegionTask);
    }

    private void LoadRegionTask()
    {
        while (true)
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
            if (RegionCacheService.HasRegion(regionCoords))
                region = RegionCacheService.Get(regionCoords);
            else
            {
                Region? diskRegion = IOService.ReadRegionFile(regionCoords, out Exception? e);
                if (diskRegion is null)
                {
                    if (e is not null)
                    {
                        // TODO display messagebox only once and never again
                        LogService.LogEmphasis($"Region {regionCoords} was skipped.",
                                               LogService.Emphasis.Error);
                        LogService.Log($"Cause of exception: {e.GetType()}");
                        LogService.Log($"Exception details: {e}", useSeparator: true);
                    }
                    continue;
                }
                region = diskRegion;
                RegionCacheService.Store(region);
            }
            RegionModel regionModel = App.InvokeDispatcherSynchronous(
                () => new RegionModel(region), DispatcherPriority.Background);
            regionModel.SetRandomImage();
            LoadRegionModel(regionModel);
        }
        lock (_workedRegionLock)
            _workedRegion = null;
        OnPropertyChanged(nameof(WorkedRegion));
    }

    private void LoadRegionModel(RegionModel regionModel)
    {
        lock (_loadedRegions)
        {
            if (_visibleRegionRange.IsOutside(regionModel.RegionCoords)
                || _loadedRegions.ContainsKey(regionModel.RegionCoords)
                || SharedStateService.SavegameLoadInfo is null)
                return;
            _loadedRegions.Add(regionModel.RegionCoords, regionModel);
        }
        App.InvokeDispatcher(
            method,
            DispatcherPriority.Render,
            DispatcherSynchronization.Asynchronous);
        OnPropertyChanged(nameof(LoadedRegionCount));
        void method() => RegionImageLoaded?.Invoke(regionModel.Image);
    }

    private void UnloadRegionModel(RegionModel regionModel)
    {
        lock (_loadedRegions)
            _loadedRegions.Remove(regionModel.RegionCoords);
        App.InvokeDispatcher(() => RegionImageUnloaded?.Invoke(regionModel.Image),
                             DispatcherPriority.Render,
                             DispatcherSynchronization.Asynchronous);
        OnPropertyChanged(nameof(LoadedRegionCount));
    }

    private void RedrawRegionImages()
    {
        App.InvokeDispatcher(method,
                             DispatcherPriority.Render,
                             DispatcherSynchronization.Synchronous);
        void method()
        {
            lock (_loadedRegions)
                foreach (RegionModel regionModel in _loadedRegions.Values)
                {
                    Point2<int> imageScreenPos = CalculateRegionImageScreenPosition(regionModel.RegionCoords);
                    regionModel.RedrawImage(imageScreenPos, PixelPerRegion);
                }
        }
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

    private void OnPropertyChanged([CallerMemberName] string propertyName = "")
    {
        App.InvokeDispatcher(() => PropertyChanged?.Invoke(propertyName),
                             DispatcherPriority.Normal,
                             DispatcherSynchronization.Asynchronous);
    }

    public void Reinitialize()
    {
        lock (_pendingRegionList)
            _pendingRegionList.Clear();
        _pendingChunkList.Clear();

        lock (_loadedRegions)
            foreach (RegionModel regionModel in _loadedRegions.Values)
                UnloadRegionModel(regionModel);

        //foreach (ChunkModel chunkModel in _loadedChunks)
        //{

        //}

        _cameraPos = Point2Z<float>.Zero;
        _zoomLevel = 1f;
        _visibleRegionRange = new Point2ZRange<int>();
        _visibleChunkRange = new Point2ZRange<int>();

    }
}

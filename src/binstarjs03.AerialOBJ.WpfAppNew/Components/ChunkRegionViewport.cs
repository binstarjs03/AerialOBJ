using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

using binstarjs03.AerialOBJ.Core;
using binstarjs03.AerialOBJ.Core.MinecraftWorld;
using binstarjs03.AerialOBJ.Core.Primitives;
using binstarjs03.AerialOBJ.WpfAppNew.Components.Interfaces;
using binstarjs03.AerialOBJ.WpfAppNew.Models;

namespace binstarjs03.AerialOBJ.WpfAppNew.Components;

public class ChunkRegionViewport : IThreadMessageReceiver
{
    public event Action<string>? PropertyChanged;
    public event RegionImageEventHandler? RegionImageLoaded;
    public event RegionImageEventHandler? RegionImageUnloaded;
    public delegate void RegionImageEventHandler(RegionImage regionImage);

    private const int s_regionBufferSize = 20;
    private const int s_chunkBufferSize = s_regionBufferSize * Region.TotalChunkCount;

    // thread messaging fields
    private readonly AutoResetEvent _messageEvent = new(false);
    private readonly Queue<Action> _messageQueue = new(50);

    // viewport fields
    private Point2Z<float> _cameraPos = Point2Z<float>.Zero;
    private float _zoomLevel = 1f;
    private Size<int> _screenSize;

    private Point2ZRange<int> _visibleRegionRange;
    protected readonly Dictionary<Point2Z<int>, RegionModel> _loadedRegions = new(s_regionBufferSize);
    protected readonly List<Point2Z<int>> _pendingRegionList = new(s_regionBufferSize);

    private Point2ZRange<int> _visibleChunkRange;
    protected readonly Dictionary<Point2Z<int>, ChunkModel> _loadedChunks = new(s_chunkBufferSize);
    protected readonly List<Point2Z<int>> _pendingChunkList = new(s_chunkBufferSize);
    protected readonly HashSet<Point2Z<int>> _pendingChunkSet = new(s_chunkBufferSize);

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

    public Point2ZRange<int> VisibleChunkRange => _visibleChunkRange;
    public int LoadedChunkCount => _loadedChunks.Count;
    public int PendingChunkCount => _pendingChunkList.Count;
    #endregion Properties

    public ChunkRegionViewport()
    {
        new Task(MessageLoop, TaskCreationOptions.LongRunning).Start();
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

    protected void Update()
    {
        if (RecalculateVisibleChunkRange())
        {
            if (RecalculateVisibleRegionRange())
                LoadUnloadRegions();
        }
    }

    protected bool RecalculateVisibleChunkRange()
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

    protected bool RecalculateVisibleRegionRange()
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
        foreach ((Point2Z<int> regionCoords, RegionModel regionModel) in _loadedRegions)
            if (_visibleRegionRange.IsOutside(regionCoords))
                UnloadRegionModel(regionModel);
        OnPropertyChanged(nameof(LoadedRegionCount));

        // remove all pending region that is no longer visible
        _pendingRegionList.RemoveAll(_visibleRegionRange.IsOutside);

        // perform sweep checking from min to max range for visible regions
        // add them to pending list if not loaded yet, not in the list, and is not worked yet
        for (int x = _visibleRegionRange.XRange.Min; x <= _visibleRegionRange.XRange.Max; x++)
            for (int z = _visibleRegionRange.ZRange.Min; z <= _visibleRegionRange.ZRange.Max; z++)
            {
                Point2Z<int> regionCoords = new(x, z);
                if (!_pendingRegionList.Contains(regionCoords))
                    _pendingRegionList.Add(regionCoords);
            }
        OnPropertyChanged(nameof(PendingRegionCount));
        // base implementation goes here, may call LoadRegion(), must call base implementation()
    }

    private void LoadRegionModel(RegionModel regionModel)
    {
        if (_visibleRegionRange.IsOutside(regionModel.RegionCoords)
            || _loadedRegions.ContainsKey(regionModel.RegionCoords))
            return;
        _loadedRegions.Add(regionModel.RegionCoords, regionModel);
        RegionImageLoaded?.Invoke(regionModel.RegionImage);
    }

    private void UnloadRegionModel(RegionModel regionModel)
    {
        _loadedRegions.Remove(regionModel.RegionCoords);
        RegionImageUnloaded?.Invoke(regionModel.RegionImage);
    }

    private void OnPropertyChanged([CallerMemberName] string propertyName = "")
    {
        PropertyChanged?.Invoke(propertyName);
    }
}

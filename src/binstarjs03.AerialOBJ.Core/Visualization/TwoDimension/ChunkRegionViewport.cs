using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using binstarjs03.AerialOBJ.Core.MinecraftWorld;
using binstarjs03.AerialOBJ.Core.Primitives;
using binstarjs03.AerialOBJ.Core.Threading.MessageDispatching;

namespace binstarjs03.AerialOBJ.Core.Visualization.TwoDimension;

public class ChunkRegionViewport : IChunkRegionViewport, ILogging, IDispatcherObject
{
    private const int s_regionBufferSize = 20;
    private const int s_chunkBufferSize = s_regionBufferSize * Region.TotalChunkCount;
    private const int s_framesPerSecond = 60;
    private const int s_redrawFrequency = 1000 / s_framesPerSecond; // unit: miliseconds

    // thread messaging fields
    private readonly IMessageDispatcher _dispatcher;
    private readonly AutoResetEvent _redrawCompletedEvent = new(false);

    // viewport fields
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

    // chunk fields
    private Point2ZRange<int> _visibleChunkRange;
    private readonly Dictionary<Point2Z<int>, IChunkModel> _loadedChunks = new(s_chunkBufferSize);
    private readonly List<Point2Z<int>> _pendingChunkList = new(s_chunkBufferSize);
    private readonly HashSet<Point2Z<int>> _pendingChunkSet = new(s_chunkBufferSize);

    public Point2Z<float> CameraPos { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public float ZoomLevel { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public Size<int> ScreenSize { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public int HeightLimit { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public Point2<int> ScreenCenter => throw new NotImplementedException();

    public float PixelPerBlock => throw new NotImplementedException();
    public float PixelPerChunk => throw new NotImplementedException();
    public float PixelPerRegion => throw new NotImplementedException();


    public Point2ZRange<int> VisibleRegionRange => throw new NotImplementedException();
    public int LoadedRegionCount => throw new NotImplementedException();
    public int PendingRegionCount => throw new NotImplementedException();

    public Point2ZRange<int> VisibleChunkRange => throw new NotImplementedException();
    public int LoadedChunkCount => throw new NotImplementedException();
    public int PendingChunkCount => throw new NotImplementedException();

    public IMessageDispatcher Dispatcher => _dispatcher;

    public event Action<IImage>? RegionImageLoaded;
    public event Action<IImage>? RegionImageUnloaded;
    public event Action<string>? Logging;

    public ChunkRegionViewport(IMessageDispatcher dispatcher)
    {
        _dispatcher = dispatcher;
    }

    public void Update()
    {
        throw new NotImplementedException();
    }
}

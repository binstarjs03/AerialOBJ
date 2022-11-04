using System;
using System.ComponentModel;
using System.Threading;
using System.Windows.Input;

using binstarjs03.AerialOBJ.Core.CoordinateSystem;
using binstarjs03.AerialOBJ.Core.MinecraftWorld;
using binstarjs03.AerialOBJ.WpfApp.UIElements.Components;

namespace binstarjs03.AerialOBJ.WpfApp.UIElements.Controls;

public class ViewportControlVM : ViewModelBase<ViewportControlVM, ViewportControl>
{
    // for updating value, dont modify field directly! use the updaters
    #region States - Fields and Properties

    // zoom table, using fibonacci sequence to produce natural zoom behaviour
    private static readonly int[] s_blockPixelCount = new int[] {
        1, 2, 3, 5, 8, 13, 21, 34
    };

    private readonly ChunkRegionManager _chunkRegionManager;
    private readonly AutoResetEvent _chunkManagerUpdateEvent = new(initialState: false);

    private PointF2 _viewportCameraPos = PointF2.Zero;
    private int _viewportZoomLevel = 1;
    private int _viewportHeightLimit = 255;

    private Coords3 _exportArea1 = Coords3.Zero;
    private Coords3 _exportArea2 = Coords3.Zero;

    private PointInt2 _mousePos = PointInt2.Zero;
    private PointInt2 _mousePosDelta = PointInt2.Zero;
    private bool _mouseClickHolding = false;
    private bool _mouseInitClickDrag = true;
    private bool _mouseIsOutside = true;

    private bool _uiSidePanelVisible = false;
    private bool _uiViewportDebugInfoVisible = false;

    // public accessor
    public PointF2 ViewportCameraPos => _viewportCameraPos;
    public double ViewportCameraPosX
    {
        get => _viewportCameraPos.X;
        set => UpdateViewportCameraPos(new PointF2(value, _viewportCameraPos.Y));
    }
    public double ViewportCameraPosZ
    {
        get => _viewportCameraPos.Y;
        set => UpdateViewportCameraPos(new PointF2(_viewportCameraPos.X, value));
    }
    public int ViewportZoomLevel
    {
        get => _viewportZoomLevel;
        set => UpdateZoomLevel(value);
    }
    public int ViewportMaximumZoomLevel => s_blockPixelCount.Length - 1;

    public int ViewportHeightLimit
    {
        get => _viewportHeightLimit;
        set => UpdateViewportHeightLimit(value);
    }

    public PointF2 ViewportChunkCanvasCenter => new(Control.ChunkCanvas.ActualWidth / 2,
                                                    Control.ChunkCanvas.ActualHeight / 2);

    public int ViewportPixelPerBlock => s_blockPixelCount[_viewportZoomLevel];
    public int ViewportPixelPerChunk => ViewportPixelPerBlock * Section.BlockCount;
    public int ViewportPixelPerRegion => ViewportPixelPerChunk * Region.ChunkCount;

    public PointF2 ChunkPosOffset => _viewportCameraPos * ViewportPixelPerBlock;
    public PointF2 RegionPosOffset => _viewportCameraPos * ViewportPixelPerBlock;
    public string ChunkPosOffsetStringized => ChunkPosOffset.ToStringAnotherFormat();
    public string RegionPosOffsetStringized => RegionPosOffset.ToStringAnotherFormat();


    public string ChunkRegionManagerVisibleChunkRangeXStringized => _chunkRegionManager.VisibleChunkRange.XRange.ToString();
    public string ChunkRegionManagerVisibleChunkRangeZStringized => _chunkRegionManager.VisibleChunkRange.ZRange.ToString();
    public string ChunkRegionManagerVisibleRegionRangeXStringized => _chunkRegionManager.VisibleRegionRange.XRange.ToString();
    public string ChunkRegionManagerVisibleRegionRangeZStringized => _chunkRegionManager.VisibleRegionRange.ZRange.ToString();
    public int ChunkRegionManagerVisibleChunkCount => _chunkRegionManager.VisibleChunkCount;
    public int ChunkRegionManagerVisibleRegionCount => _chunkRegionManager.VisibleRegionCount;
    public int ChunkRegionManagerLoadedChunkCount => _chunkRegionManager.LoadedChunkCount;
    public int ChunkRegionManagerLoadedRegionCount => _chunkRegionManager.LoadedRegionCount;
    public int ChunkRegionManagerPendingChunkCount => _chunkRegionManager.PendingChunkCount;
    public int ChunkRegionManagerWorkedChunkCount => _chunkRegionManager.WorkedChunkCount;

    public bool UISidePanelVisible
    {
        get => _uiSidePanelVisible;
        set => SetAndNotifyPropertyChanged(value, ref _uiSidePanelVisible);
    }
    public bool UIViewportDebugInfoVisible
    {
        get => _uiViewportDebugInfoVisible;
        set => SetAndNotifyPropertyChanged(value, ref _uiViewportDebugInfoVisible);
    }

    public int ExportArea1X
    {
        get => _exportArea1.X;
        set => UpdateExportArea1(new Coords3(value, _exportArea1.Y, _exportArea1.Z));
    }
    public int ExportArea1Y
    {
        get => _exportArea1.Y;
        set => UpdateExportArea1(new Coords3(_exportArea1.X, value, _exportArea1.Z));

    }
    public int ExportArea1Z
    {
        get => _exportArea1.Z;
        set => UpdateExportArea1(new Coords3(_exportArea1.X, _exportArea1.Y, value));
    }

    public int ExportArea2X
    {
        get => _exportArea2.X;
        set => UpdateExportArea2(new Coords3(value, _exportArea2.Y, _exportArea2.Z));
    }
    public int ExportArea2Y
    {
        get => _exportArea2.Y;
        set => UpdateExportArea1(new Coords3(_exportArea2.X, value, _exportArea2.Z));
    }
    public int ExportArea2Z
    {
        get => _exportArea2.Z;
        set => UpdateExportArea1(new Coords3(_exportArea2.X, _exportArea2.Y, value));
    }

    public string MousePosStringized => _mousePos.ToString();
    public string MousePosDeltaStringized => _mousePosDelta.ToString();
    public bool MouseClickHolding => _mouseClickHolding;
    public bool MouseIsOutside => _mouseIsOutside;

    #endregion

    public ViewportControlVM(ViewportControl control) : base(control)
    {
        App.CurrentCast.Properties.PropertyChanged += OnSharedPropertyChanged;

        _chunkRegionManager = new ChunkRegionManager(this, _chunkManagerUpdateEvent);

        // set commands to its corresponding implementations
        SizeChangedCommand = new RelayCommand(OnSizeChanged);
        MouseWheelCommand = new RelayCommand(OnMouseWheel);
        MouseMoveCommand = new RelayCommand(OnMouseMove);
        MouseUpCommand = new RelayCommand(OnMouseUp);
        MouseDownCommand = new RelayCommand(OnMouseDown);
        MouseLeaveCommand = new RelayCommand(OnMouseLeave);
        MouseEnterCommand = new RelayCommand(OnMouseEnter);
        KeyUpCommand = new RelayCommand(OnKeyUp);
    }

    #region Commands

    public ICommand SizeChangedCommand { get; }
    public ICommand MouseWheelCommand { get; }
    public ICommand MouseMoveCommand { get; }
    public ICommand MouseUpCommand { get; }
    public ICommand MouseDownCommand { get; }
    public ICommand MouseLeaveCommand { get; }
    public ICommand MouseEnterCommand { get; }
    public ICommand KeyUpCommand { get; }

    private void OnSizeChanged(object? arg)
    {
        UpdateChunkRegionManager();
    }

    private void OnMouseWheel(object? arg)
    {
        MouseWheelEventArgs e = (MouseWheelEventArgs)arg!;
        int newZoomLevel = _viewportZoomLevel;
        if (e.Delta > 0)
            newZoomLevel++;
        else
            newZoomLevel--;
        newZoomLevel = Math.Clamp(newZoomLevel, 0, ViewportMaximumZoomLevel);
        UpdateZoomLevel(newZoomLevel);
    }

    private void OnMouseMove(object? arg)
    {
        MouseEventArgs e = (MouseEventArgs)arg!;
        PointInt2 oldMousePos = _mousePos;
        PointInt2 newMousePos = new((int)e.GetPosition(Control).X,
                                    (int)e.GetPosition(Control).Y);
        UpdateMousePos(newMousePos);

        // Set delta to 0 if this call is initial click and dragging.
        // This is to avoid jumps when clicking menu bar
        // then clicking and dragging on the viewer again.
        PointInt2 newMousePosDelta = PointInt2.Zero;
        newMousePosDelta.X = _mouseInitClickDrag && _mouseClickHolding ? 0 : newMousePos.X - oldMousePos.X;
        newMousePosDelta.Y = _mouseInitClickDrag && _mouseClickHolding ? 0 : newMousePos.Y - oldMousePos.Y;
        UpdateMousePosDelta(newMousePosDelta);

        if (MouseClickHolding)
        {
            // increase division resolution or precision from int to double
            PointF2 newCameraPos = _viewportCameraPos;
            newCameraPos -= ((PointF2)_mousePosDelta) / ViewportPixelPerBlock;
            UpdateViewportCameraPos(newCameraPos);
            UpdateMouseInitClickDrag(false);
        }
    }

    private void OnMouseUp(object? arg)
    {
        MouseButtonEventArgs e = (MouseButtonEventArgs)arg!;
        if (e.LeftButton == MouseButtonState.Released)
        {
            UpdateMouseClickHolding(false);
            UpdateMouseInitClickDrag(true);
        }
    }

    private void OnMouseDown(object? arg)
    {
        MouseButtonEventArgs e = (MouseButtonEventArgs)arg!;
        ClearFocus(); // on mouse down any mouse button, yes
        if (e.LeftButton == MouseButtonState.Pressed)
        {
            UpdateMouseClickHolding(true);
        }
    }

    private void OnMouseLeave(object? arg)
    {
        UpdateMouseIsOutside(true);
        UpdateMouseClickHolding(false);
    }

    private void OnMouseEnter(object? arg)
    {
        UpdateMouseIsOutside(false);
    }

    private void OnKeyUp(object? arg)
    {
        if (arg is null)
            throw new ArgumentNullException(nameof(arg));
        KeyEventArgs e = (KeyEventArgs)arg;
        object sender = e.Source;
        if (e.Key == Key.Return && sender is INumericBox)
        {
            ClearFocus();
            e.Handled = true;
            return;
        }
    }

    #endregion

    #region Event Handlers

    protected override void OnSharedPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        base.OnSharedPropertyChanged(sender, e);
        string propName = e.PropertyName!;
        if (propName == nameof(App.CurrentCast.Properties.SessionInfo))
        {
            ReinitializeStates();
            if (App.CurrentCast.Properties.SessionInfo is null)
                _chunkRegionManager.OnSessionClosed();
        }
    }

    #endregion

    #region Updaters

    private void UpdateChunkRegionManager()
    {
        _chunkManagerUpdateEvent.Set();
    }

    private void UpdateViewportCameraPos(PointF2 cameraPos)
    {
        _viewportCameraPos = cameraPos;
        NotifyPropertyChanged(new string[]
        {
            nameof(ViewportCameraPosX),
            nameof(ViewportCameraPosZ),
            nameof(ChunkPosOffsetStringized),
            nameof(RegionPosOffsetStringized),
        });
        UpdateChunkRegionManager();
    }

    private void UpdateViewportHeightLimit(int viewportHeightLimit)
    {
        SetAndNotifyPropertyChanged(viewportHeightLimit, ref _viewportHeightLimit, nameof(ViewportHeightLimit));
        UpdateChunkRegionManager();
    }

    private void UpdateExportArea1(Coords3 exportArea1)
    {
        _exportArea1 = exportArea1;
        NotifyPropertyChanged(new string[]
        {
            nameof(ViewportZoomLevel),
            nameof(ExportArea1X),
            nameof(ExportArea1Y),
            nameof(ExportArea1Z),
        });
    }

    private void UpdateZoomLevel(int zoomLevel)
    {
        SetAndNotifyPropertyChanged(zoomLevel, ref _viewportZoomLevel, nameof(ViewportZoomLevel));
        NotifyPropertyChanged(new string[]
        {
            nameof(ViewportPixelPerBlock),
            nameof(ViewportPixelPerChunk),
            nameof(ViewportPixelPerRegion),
            nameof(ChunkPosOffsetStringized),
            nameof(RegionPosOffsetStringized),
        });
        UpdateChunkRegionManager();
    }

    private void UpdateExportArea2(Coords3 exportArea2)
    {
        _exportArea2 = exportArea2;
        NotifyPropertyChanged(new string[]
        {
            nameof(ExportArea2X),
            nameof(ExportArea2Y),
            nameof(ExportArea2Z),
        });
    }

    private void UpdateMousePos(PointInt2 mousePos)
    {
        SetAndNotifyPropertyChanged(mousePos, ref _mousePos, nameof(MousePosStringized));
    }

    private void UpdateMousePosDelta(PointInt2 mousePosDelta)
    {
        SetAndNotifyPropertyChanged(mousePosDelta, ref _mousePosDelta, nameof(MousePosDeltaStringized));
    }

    private void UpdateMouseClickHolding(bool mouseClickHolding)
    {
        SetAndNotifyPropertyChanged(mouseClickHolding, ref _mouseClickHolding, nameof(MouseClickHolding));
    }

    private void UpdateMouseIsOutside(bool mouseIsOutside)
    {
        SetAndNotifyPropertyChanged(mouseIsOutside, ref _mouseIsOutside, nameof(MouseIsOutside));
    }

    private void UpdateMouseInitClickDrag(bool mouseInitClickDrag)
    {
        _mouseInitClickDrag = mouseInitClickDrag;
    }

    #endregion

    #region Methods

    private void ClearFocus()
    {
        Control.HeightSlider.Focus();
        Keyboard.ClearFocus();
    }

    private void ReinitializeStates()
    {
        ClearFocus();
        UpdateViewportCameraPos(PointF2.Zero);
        UpdateZoomLevel(1);
        UpdateViewportHeightLimit(255);

        UpdateExportArea1(Coords3.Zero);
        UpdateExportArea2(Coords3.Zero);

        UpdateMousePos(PointInt2.Zero);
        UpdateMousePosDelta(PointInt2.Zero);
        UpdateMouseClickHolding(false);
        UpdateMouseInitClickDrag(true);
        UpdateMouseIsOutside(true);
    }

    #endregion
}

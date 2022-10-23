using System;
using System.ComponentModel;
using System.Windows.Input;

using binstarjs03.AerialOBJ.Core.CoordinateSystem;
using binstarjs03.AerialOBJ.Core.WorldRegion;
using binstarjs03.AerialOBJ.WpfApp.UIElements.Components;

namespace binstarjs03.AerialOBJ.WpfApp.UIElements.Controls;

public class ViewportControlVM : ViewModelBase<ViewportControlVM, ViewportControl>
{
    public ViewportControlVM(ViewportControl control) : base(control)
    {
        SharedProperty.PropertyChanged += OnSharedPropertyChanged;

        _chunkManager = new(this);

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

    // for updating value, dont modify field directly! use the updaters
    #region States - Fields and Properties

    // zoom table, using fibonacci sequence to produce natural zoom behaviour
    private static readonly int[] s_blockPixelCount = new int[] {
        1, 2, 3, 5, 8, 13, 21, 34
    };

    private PointF2 _viewportCameraPos = PointF2.Zero;
    private int _viewportZoomLevel = 1;
    private int _viewportLimitHeight = 255;

    private readonly ChunkManager _chunkManager;

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
    public PointF2 ViewportChunkCanvasCenter => new(Control.ChunkCanvas.ActualWidth / 2,
                                                    Control.ChunkCanvas.ActualHeight / 2);
    public static int ViewportMaximumZoomLevel => s_blockPixelCount.Length - 1;
    public int ViewportPixelPerBlock => s_blockPixelCount[_viewportZoomLevel];
    public int ViewportPixelPerChunk => ViewportPixelPerBlock * Section.BlockCount;
    public PointF2 ChunkPosOffset => _viewportCameraPos * ViewportPixelPerBlock;

    #endregion

    // do not use data binders (even just dereferencing it),
    // use the non-data binders version instead!
    #region Data Binders

    public bool UISidePanelVisibleBinding
    {
        get => _uiSidePanelVisible;
        set => SetAndNotifyPropertyChanged(value, ref _uiSidePanelVisible);
    }

    public bool UIViewportDebugInfoVisibleBinding
    {
        get => _uiViewportDebugInfoVisible;
        set => SetAndNotifyPropertyChanged(value, ref _uiViewportDebugInfoVisible);
    }

    public double ViewportCameraPosXBinding
    {
        get => _viewportCameraPos.X;
        set => UpdateViewportCameraPos(new PointF2(value, _viewportCameraPos.Y));
    }

    public double ViewportCameraPosZBinding
    {
        get => _viewportCameraPos.Y;
        set => UpdateViewportCameraPos(new PointF2(_viewportCameraPos.X, value));
    }

    public int ViewportZoomLevelBinding
    {
        get => _viewportZoomLevel;
        set => UpdateZoomLevel(value);
    }

    public int ViewportMaximumZoomLevelBinding => ViewportMaximumZoomLevel;
    public int ViewportPixelPerBlockBinding => ViewportPixelPerBlock;
    public int ViewportPixelPerChunkBinding => ViewportPixelPerChunk;
    public int ViewportLimitHeightBinding
    {
        get => _viewportLimitHeight;
        set => UpdateViewportLimitHeight(value);
    }

    public string ChunkPosOffsetBinding => ChunkPosOffset.ToStringAnotherFormat();

    public string ChunkManagerVisibleChunkRangeXBinding => _chunkManager.VisibleChunkRange.XRange.ToString();
    public string ChunkManagerVisibleChunkRangeZBinding => _chunkManager.VisibleChunkRange.ZRange.ToString();
    public string ChunkManagerVisibleRegionRangeXBinding => _chunkManager.VisibleRegionRange.XRange.ToString();
    public string ChunkManagerVisibleRegionRangeZBinding => _chunkManager.VisibleRegionRange.ZRange.ToString();
    public int ChunkManagerVisibleChunkCount => _chunkManager.VisibleChunkCount;
    public int ChunkManagerLoadedChunkCount => _chunkManager.LoadedChunkCount;
    public int ChunkManagerPendingChunkCount => ChunkManagerVisibleChunkCount - ChunkManagerLoadedChunkCount;

    public int ExportArea1XBinding
    {
        get => _exportArea1.X;
        set => UpdateExportArea1(new Coords3(value, _exportArea1.Y, _exportArea1.Z));
    }

    public int ExportArea1YBinding
    {
        get => _exportArea1.Y;
        set => UpdateExportArea1(new Coords3(_exportArea1.X, value, _exportArea1.Z));

    }

    public int ExportArea1ZBinding
    {
        get => _exportArea1.Z;
        set => UpdateExportArea1(new Coords3(_exportArea1.X, _exportArea1.Y, value));
    }

    public int ExportArea2XBinding
    {
        get => _exportArea2.X;
        set => UpdateExportArea2(new Coords3(value, _exportArea2.Y, _exportArea2.Z));
    }

    public int ExportArea2YBinding
    {
        get => _exportArea2.Y;
        set => UpdateExportArea1(new Coords3(_exportArea2.X, value, _exportArea2.Z));
    }

    public int ExportArea2ZBinding
    {
        get => _exportArea2.Z;
        set => UpdateExportArea1(new Coords3(_exportArea2.X, _exportArea2.Y, value));
    }

    public string MousePosBinding => _mousePos.ToStringAnotherFormat();
    public string MousePosDeltaBinding => _mousePosDelta.ToStringAnotherFormat();
    public bool MouseClickHoldingBinding => _mouseClickHolding;
    public bool MouseIsOutsideBinding => _mouseIsOutside;

    #endregion

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
        _chunkManager.Update(_viewportLimitHeight);
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

        if (MouseClickHoldingBinding)
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
        if (propName == nameof(SharedProperty.SessionInfo))
        {
            ReinitializeStates();
            if (SharedProperty.SessionInfo is null)
                _chunkManager.OnSessionClosed();
        }
    }

    #endregion

    #region Updaters

    public void UpdateUISidePanelVisible(bool value)
    {
        SetAndNotifyPropertyChanged(value, ref _uiSidePanelVisible, nameof(UISidePanelVisibleBinding));
    }

    private void UpdateViewportCameraPos(PointF2 cameraPos)
    {
        _viewportCameraPos = cameraPos;
        NotifyPropertyChanged(new string[]
        {
            nameof(ViewportCameraPosXBinding),
            nameof(ViewportCameraPosZBinding),
            nameof(ChunkPosOffsetBinding),
        });
        _chunkManager.Update(_viewportLimitHeight);
    }

    private void UpdateViewportLimitHeight(int viewportLimitHeight)
    {
        SetAndNotifyPropertyChanged(viewportLimitHeight, ref _viewportLimitHeight, nameof(ViewportLimitHeightBinding));
    }

    private void UpdateExportArea1(Coords3 exportArea1)
    {
        _exportArea1 = exportArea1;
        NotifyPropertyChanged(new string[]
        {
            nameof(ViewportZoomLevelBinding),
            nameof(ExportArea1XBinding),
            nameof(ExportArea1YBinding),
            nameof(ExportArea1ZBinding),
        });
    }

    private void UpdateZoomLevel(int zoomLevel)
    {
        SetAndNotifyPropertyChanged(zoomLevel, ref _viewportZoomLevel, nameof(ViewportZoomLevelBinding));
        NotifyPropertyChanged(new string[]
        {
            nameof(ChunkPosOffsetBinding),
            nameof(ViewportPixelPerBlockBinding),
            nameof(ViewportPixelPerChunkBinding),
        });
        _chunkManager.Update(_viewportLimitHeight);
    }

    private void UpdateExportArea2(Coords3 exportArea2)
    {
        _exportArea2 = exportArea2;
        NotifyPropertyChanged(new string[]
        {
            nameof(ExportArea2XBinding),
            nameof(ExportArea2YBinding),
            nameof(ExportArea2ZBinding),
        });
    }

    private void UpdateMousePos(PointInt2 mousePos)
    {
        SetAndNotifyPropertyChanged(mousePos, ref _mousePos, nameof(MousePosBinding));
    }

    private void UpdateMousePosDelta(PointInt2 mousePosDelta)
    {
        SetAndNotifyPropertyChanged(mousePosDelta, ref _mousePosDelta, nameof(MousePosDeltaBinding));
    }

    private void UpdateMouseClickHolding(bool mouseClickHolding)
    {
        SetAndNotifyPropertyChanged(mouseClickHolding, ref _mouseClickHolding, nameof(MouseClickHoldingBinding));
    }

    private void UpdateMouseIsOutside(bool mouseIsOutside)
    {
        SetAndNotifyPropertyChanged(mouseIsOutside, ref _mouseIsOutside, nameof(MouseIsOutsideBinding));
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
        UpdateViewportLimitHeight(255);

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

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

using binstarjs03.CubeOBJ.Core.CoordinateSystem;
using binstarjs03.CubeOBJ.Core.WorldRegion;

using Range = binstarjs03.CubeOBJ.Core.Range;

namespace binstarjs03.CubeOBJ.WpfApp.UIElements.Controls;

public class ViewportControlVM : ViewModelBase<ViewportControlVM, ViewportControl>
{
    public ViewportControlVM(ViewportControl control) : base(control)
    {
        SharedProperty.PropertyChanged += OnSharedPropertyChanged;

        // set commands to its corresponding implementations
        SizeChangedCommand = new RelayCommand(OnSizeChanged);
        MouseWheelCommand = new RelayCommand(OnMouseWheel);
        MouseMoveCommand = new RelayCommand(OnMouseMove);
        MouseUpCommand = new RelayCommand(OnMouseUp);
        MouseDownCommand = new RelayCommand(OnMouseDown);
        MouseLeaveCommand = new RelayCommand(OnMouseLeave);
        MouseEnterCommand = new RelayCommand(OnMouseEnter);
    }

    // for updating value, dont modify field directly! use the updaters
    #region States - Fields and Properties

    private static readonly int[] s_blockPixelCount = new int[] {
        1, 2, 3, 5, 8, 13, 21, 34
    };

    private PointF2 _viewportCameraPos = PointF2.Zero;
    private int _viewportZoomLevel = 2;
    private int _viewportLimitHeight = 255;
    private Coords3 _exportArea1 = Coords3.Zero;
    private Coords3 _exportArea2 = Coords3.Zero;

    private PointInt2 _mousePos = PointInt2.Zero;
    private PointInt2 _mousePosDelta = PointInt2.Zero;
    private bool _mouseClickHolding = false;
    private bool _mouseInitClickDrag = true;
    private bool _mouseIsOutside = true;

    private Size ViewportChunkCanvasSize => new(Control.ChunkCanvas.Width,
                                                Control.ChunkCanvas.Height);
    private PointF2 ViewportChunkCanvasCenter => new(Control.ChunkCanvas.ActualWidth / 2,
                                                     Control.ChunkCanvas.ActualHeight / 2);
    private PointF2 ChunkPosOffset => _viewportCameraPos * ViewportPixelPerBlock;
    private int ViewportMaximumZoomLevel => s_blockPixelCount.Length - 1;
    private int ViewportPixelPerBlock => s_blockPixelCount[_viewportZoomLevel];
    private int ViewportPixelPerChunk => ViewportPixelPerBlock * Section.BlockCount;

    #endregion

    // do not use data binders (even just dereferencing it),
    // use the non-data binders version instead!
    #region Data Binders

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


    public bool UISidePanelVisibleBinding
    {
        get => SharedProperty.IsSidePanelVisible;
        set => SharedProperty.IsSidePanelVisibleUpdater(value);
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

    private void OnSizeChanged(object? arg) { }
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

    #endregion

    #region Event Handlers

    protected override void OnSharedPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        base.OnSharedPropertyChanged(sender, e);
        string propName = e.PropertyName!;
        if (propName == nameof(SharedProperty.SessionInfo))
        {
            ReinitializeStates();
        }
        else if (propName == nameof(SharedProperty.IsSidePanelVisible))
        {
            NotifyPropertyChanged(nameof(UISidePanelVisibleBinding));
        }
    }

    #endregion

    #region Updaters

    private void UpdateViewportCameraPos(PointF2 cameraPos)
    {
        _viewportCameraPos = cameraPos;
        NotifyPropertyChanged(new string[]
        {
            nameof(ViewportCameraPosXBinding),
            nameof(ViewportCameraPosZBinding),
            nameof(ChunkPosOffsetBinding),
        });
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
        //Keyboard.ClearFocus();
        // ^ even focus is cleared, DoubleBox is still displaying the
        // non-formatted string, which only does that when IsFocused is false.
        // To fix this, we set focus to any element other that DoubleBox,
        // may be inappropriate but it works
        Control.HeightSlider.Focus();
    }

    private void ReinitializeStates()
    {
        ClearFocus();
        UpdateViewportCameraPos(PointF2.Zero);
        UpdateZoomLevel(2);
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
        ExportArea1 = Coords3.Zero;
        ExportArea2 = Coords3.Zero;

        }
    }
}

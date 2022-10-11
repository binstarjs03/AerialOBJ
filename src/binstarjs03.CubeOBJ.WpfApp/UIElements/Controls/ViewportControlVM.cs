using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;

using binstarjs03.CubeOBJ.Core.CoordinateSystem;
using binstarjs03.CubeOBJ.Core.WorldRegion;

namespace binstarjs03.CubeOBJ.WpfApp.UIElements.Controls;

// TODO consider refactor restructure code
public class ViewportControlVM : ViewModelBase<ViewportControlVM, ViewportControl>
{
    public ViewportControlVM(ViewportControl control) : base(control)
    {
        SharedProperty.PropertyChanged += OnSharedPropertyChanged;

        SizeChangedCommand = new RelayCommand(OnSizeChanged);
        MouseWheelCommand = new RelayCommand(OnMouseWheel);
        MouseMoveCommand = new RelayCommand(OnMouseMove);
        MouseUpCommand = new RelayCommand(OnMouseUp);
        MouseDownCommand = new RelayCommand(OnMouseDown);
        MouseLeaveCommand = new RelayCommand(OnMouseLeave);
        MouseEnterCommand = new RelayCommand(OnMouseEnter);
    }

    #region States

    private static readonly int[] s_zoomBlockPixelCount = new int[] {
        1, 2, 3, 5, 8, 13, 21, 34
    };

    private PointF2 _cameraPos = PointF2.Zero;
    private int _zoomLevel = 2;
    private int _height = 255;
    private Coords3 _exportArea1 = Coords3.Zero;
    private Coords3 _exportArea2 = Coords3.Zero;

    private PointInt2 _mousePos = PointInt2.Zero;
    private PointInt2 _mousePosDelta = PointInt2.Zero;
    private bool _mouseClickHolding = false;
    private bool _mouseInitClickDrag = true;
    private bool _mouseIsOutside = true;

    private PointF2 CameraPos
    {
        get => _cameraPos;
        set
        {
            _cameraPos = value;
            NotifyPropertyChanged(nameof(CameraPosX));
            NotifyPropertyChanged(nameof(CameraPosZ));
        }
    }

    private Coords3 ExportArea1
    {
        get => _exportArea1;
        set
        {
            _exportArea1 = value;
            NotifyPropertyChanged(nameof(ExportArea1X));
            NotifyPropertyChanged(nameof(ExportArea1Y));
            NotifyPropertyChanged(nameof(ExportArea1Z));
        }
    }

    private Coords3 ExportArea2
    {
        get => _exportArea2;
        set
        {
            _exportArea2 = value;
            NotifyPropertyChanged(nameof(ExportArea2X));
            NotifyPropertyChanged(nameof(ExportArea2Y));
            NotifyPropertyChanged(nameof(ExportArea2Z));
        }
    }

    public Size ChunkCanvasSize => new(Control.ChunkCanvas.Width,
                                       Control.ChunkCanvas.Height);

    public PointF2 ChunkCanvasCenter => new(Control.ChunkCanvas.ActualWidth / 2,
                                            Control.ChunkCanvas.ActualHeight / 2);

    public PointF2 ChunkPosOffset => _cameraPos * PixelPerBlock;

    public int PixelPerBlock => s_zoomBlockPixelCount[_zoomLevel];

    public int PixelPerChunk => PixelPerBlock * Section.BlockCount;

    #endregion

    #region Data Binders

    public int MaximumZoomLevel => s_zoomBlockPixelCount.Length - 1;

    public double CameraPosX
    {
        get => _cameraPos.X;
        set => SetAndNotifyPropertyChanged(value, ref _cameraPos.X);
    }
    public double CameraPosZ
    {
        get => _cameraPos.Y;
        set => SetAndNotifyPropertyChanged(value, ref _cameraPos.Y);
    }


    public int ZoomLevel
    {
        get => _zoomLevel;
        set => SetAndNotifyPropertyChanged(value, ref _zoomLevel);
    }


    public int Height
    {
        get => _height;
        set => SetAndNotifyPropertyChanged(value, ref _height);
    }


    public int ExportArea1X
    {
        get => _exportArea1.X;
        set => SetAndNotifyPropertyChanged(value, ref _exportArea1.X);
    }
    public int ExportArea1Y
    {
        get => _exportArea1.Y;
        set => SetAndNotifyPropertyChanged(value, ref _exportArea1.Y);
    }
    public int ExportArea1Z
    {
        get => _exportArea1.Z;
        set => SetAndNotifyPropertyChanged(value, ref _exportArea1.Z);
    }


    public int ExportArea2X
    {
        get => _exportArea2.X;
        set => SetAndNotifyPropertyChanged(value, ref _exportArea2.X);
    }
    public int ExportArea2Y
    {
        get => _exportArea2.Y;
        set => SetAndNotifyPropertyChanged(value, ref _exportArea2.Y);
    }
    public int ExportArea2Z
    {
        get => _exportArea2.Z;
        set => SetAndNotifyPropertyChanged(value, ref _exportArea2.Z);
    }



    public PointInt2 MousePos
    {
        get => _mousePos;
        set => SetAndNotifyPropertyChanged(value, ref _mousePos);
    }
    public PointInt2 MousePosDelta
    {
        get => _mousePosDelta;
        set => SetAndNotifyPropertyChanged(value, ref _mousePosDelta);
    }
    public bool MouseClickHolding
    {
        get => _mouseClickHolding;
        set => SetAndNotifyPropertyChanged(value, ref _mouseClickHolding);
    }
    public bool MouseIsOutside
    {
        get => _mouseIsOutside;
        set => SetAndNotifyPropertyChanged(value, ref _mouseIsOutside);
    }

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
        if (e.Delta > 0)
            ZoomLevel++;
        else
            ZoomLevel--;
        ZoomLevel = Math.Clamp(ZoomLevel, 0, MaximumZoomLevel);

    }

    private void OnMouseMove(object? arg)
    {
        MouseEventArgs e = (MouseEventArgs)arg!;
        PointInt2 oldMousePos = _mousePos;
        PointInt2 newMousePos = new((int)e.GetPosition(Control).X,
                                    (int)e.GetPosition(Control).Y);
        MousePos = newMousePos;

        // Set delta to 0 if this call is initial click and dragging.
        // This is to avoid jumps when clicking menu bar
        // then clicking and dragging on the viewer again.
        PointInt2 newMousePosDelta = PointInt2.Zero;
        newMousePosDelta.X = _mouseInitClickDrag && _mouseClickHolding ? 0 : newMousePos.X - oldMousePos.X;
        newMousePosDelta.Y = _mouseInitClickDrag && _mouseClickHolding ? 0 : newMousePos.Y - oldMousePos.Y;
        MousePosDelta = newMousePosDelta;

        if (MouseClickHolding)
        {
            // increase division precision from int to float (double)
            CameraPos -= ((PointF2)_mousePosDelta) / PixelPerBlock;
            _mouseInitClickDrag = false;
        }
    }

    private void OnMouseUp(object? arg)
    {
        MouseButtonEventArgs e = (MouseButtonEventArgs)arg!;
        if (e.LeftButton == MouseButtonState.Released)
        {
            MouseClickHolding = false;
            _mouseInitClickDrag = true;
        }
    }

    private void OnMouseDown(object? arg)
    {
        MouseButtonEventArgs e = (MouseButtonEventArgs)arg!;

        if (e.LeftButton == MouseButtonState.Pressed)
        {
            MouseClickHolding = true;
        }
    }

    private void OnMouseLeave(object? arg)
    {
        MouseIsOutside = true;
        MouseClickHolding = false;
    }

    private void OnMouseEnter(object? arg)
    {
        MouseIsOutside = false;
    }

    #endregion

    #region Event Handlers

    protected override void OnSharedPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        base.OnSharedPropertyChanged(sender, e);
        if (e.PropertyName! == nameof(SharedProperty.SessionInfo))
        {
            ReinitializeStates();
        }
    }

    #endregion

    private void ReinitializeStates()
    {
        Control.ViewportCanvas.Focus();
        CameraPos = PointF2.Zero;
        ZoomLevel = 2;
        Height = 255;
        
        ExportArea1 = Coords3.Zero;
        ExportArea2 = Coords3.Zero;

        MousePos = PointInt2.Zero;
        MousePosDelta = PointInt2.Zero;
        MouseClickHolding = false;
        _mouseInitClickDrag = true;
        MouseIsOutside = true;
    }
}

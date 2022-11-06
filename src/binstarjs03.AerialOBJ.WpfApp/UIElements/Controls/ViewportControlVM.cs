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
    #region States - Fields and Properties
    // Because our viewport VM has complex interaction with the view, for updating value,
    // DON'T modify field directly! use the updaters or set the properties instead

    // Zoom table, array index represent how many pixels one block is.
    // We use fibonacci sequence to produce natural zoom behaviour
    private static readonly int[] s_blockPixelCount = new int[] {
        1, 2, 3, 5, 8, 13, 21, 34
    };

    private readonly ChunkRegionManager _chunkRegionManager;

    private PointF2 _cameraPos = PointF2.Zero;
    private int _zoomLevel = 1;
    private int _heightLimit = 255;

    private Coords3 _exportArea1 = Coords3.Zero;
    private Coords3 _exportArea2 = Coords3.Zero;

    private PointInt2 _mousePos = PointInt2.Zero;
    private PointInt2 _mousePosDelta = PointInt2.Zero;
    private bool _mouseClickHolding = false;
    private bool _mouseInitClickDrag = true;
    private bool _mouseIsOutside = true;

    private bool _sidePanelVisible = false;
    private bool _sidePanelDebugInfoVisible = false;

    // Public accessor.
    // Yes, below is a very intricate setup of sea of properties

    // Camera Group \/
    public PointF2 CameraPos
    {
        get => _cameraPos;
        set => UpdateCameraPos(value);
    }
    public double CameraPosX
    {
        get => _cameraPos.X;
        set => UpdateCameraPos(new PointF2(value, _cameraPos.Y));
    }
    public double CameraPosZ
    {
        get => _cameraPos.Y;
        set => UpdateCameraPos(new PointF2(_cameraPos.X, value));
    }
    // Camera Group /\

    // Zooming group \/
    public int ZoomLevel
    {
        get => _zoomLevel;
        set
        {
            string[] propNames = new string[]
            {
                nameof(PixelPerBlock),
                nameof(PixelPerChunk),
                nameof(PixelPerRegion),
                nameof(ViewportItemOffsetStringized),
            };
            SetAndNotifyPropertyChanged(value, ref _zoomLevel, propNames, UpdateChunkRegionManager);
        }
    }
    public int MaximumZoomLevel => s_blockPixelCount.Length - 1;
    // Zooming group /\

    public int HeightLimit
    {
        get => _heightLimit;
        set => SetAndNotifyPropertyChanged(value, ref _heightLimit, UpdateChunkRegionManager);
    }

    public PointF2 ChunkCanvasCenter => new(Control.ViewportPanel.ActualWidth / 2,
                                            Control.ViewportPanel.ActualHeight / 2);

    // How many pixels per group \/
    public int PixelPerBlock => s_blockPixelCount[_zoomLevel];
    public int PixelPerChunk => PixelPerBlock * Section.BlockCount;
    public int PixelPerRegion => PixelPerChunk * Region.ChunkCount;
    // How many pixels per group /\

    // Offsets group \/
    public PointF2 ViewportItemOffset => _cameraPos * PixelPerBlock;
    public string ViewportItemOffsetStringized => ViewportItemOffset.ToStringRounded();
    // Offsets group /\

    // ChunkRegionManager group \/
    public string ChunkRegionManagerVisibleChunkRangeXStringized => _chunkRegionManager.VisibleChunkRange.XRange.ToString();
    public string ChunkRegionManagerVisibleChunkRangeZStringized => _chunkRegionManager.VisibleChunkRange.ZRange.ToString();
    public int ChunkRegionManagerVisibleChunkCount => _chunkRegionManager.VisibleChunkCount;
    public int ChunkRegionManagerLoadedChunkCount => _chunkRegionManager.LoadedChunkCount;
    public int ChunkRegionManagerPendingChunkCount => _chunkRegionManager.PendingChunkCount;
    public int ChunkRegionManagerWorkedChunkCount => _chunkRegionManager.WorkedChunkCount;

    public string ChunkRegionManagerVisibleRegionRangeXStringized => _chunkRegionManager.VisibleRegionRange.XRange.ToString();
    public string ChunkRegionManagerVisibleRegionRangeZStringized => _chunkRegionManager.VisibleRegionRange.ZRange.ToString();
    public int ChunkRegionManagerVisibleRegionCount => _chunkRegionManager.VisibleRegionCount;
    public int ChunkRegionManagerLoadedRegionCount => _chunkRegionManager.LoadedRegionCount;
    public int ChunkRegionManagerPendingRegionCount => _chunkRegionManager.PendingRegionCount;
    public string ChunkRegionManagerWorkedRegion => _chunkRegionManager.WorkedRegion;
    // ChunkRegionManager group /\

    // Export Area 1 Group \/
    public Coords3 ExportArea1
    {
        get => _exportArea1;
        set => UpdateExportArea1(value);
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
    // Export Area 1 Group /\

    // Export Area 2 Group \/
    public Coords3 ExportArea2
    {
        get => _exportArea2;
        set => UpdateExportArea2(value);
    }
    public int ExportArea2X
    {
        get => _exportArea2.X;
        set => UpdateExportArea2(new Coords3(value, _exportArea2.Y, _exportArea2.Z));
    }
    public int ExportArea2Y
    {
        get => _exportArea2.Y;
        set => UpdateExportArea2(new Coords3(_exportArea2.X, value, _exportArea2.Z));
    }
    public int ExportArea2Z
    {
        get => _exportArea2.Z;
        set => UpdateExportArea2(new Coords3(_exportArea2.X, _exportArea2.Y, value));
    }
    // Export Area 2 Group /\

    // Mouse Group \/
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
    public bool MouseInitClickDrag
    {
        get => _mouseInitClickDrag;
        set => _mouseInitClickDrag = value;
    }
    public bool MouseIsOutside
    {
        get => _mouseIsOutside;
        set => SetAndNotifyPropertyChanged(value, ref _mouseIsOutside);
    }
    // Mouse Group /\

    // Side Panel Group \/
    public bool SidePanelVisible
    {
        get => _sidePanelVisible;
        set => SetAndNotifyPropertyChanged(value, ref _sidePanelVisible);
    }
    public bool SidePanelDebugInfoVisible
    {
        get => _sidePanelDebugInfoVisible;
        set => SetAndNotifyPropertyChanged(value, ref _sidePanelDebugInfoVisible);
    }
    // Side Panel Group /\

    #endregion States - Fields and Properties

    #region Property Updater Helper

    private void UpdateChunkRegionManager()
    {
        _chunkRegionManager.PostMessage(_chunkRegionManager.Update, ChunkRegionManager.MessagePriority.Normal);
    }

    private void UpdateCameraPos(PointF2 cameraPos)
    {
        SetAndNotifyPropertyChanged(cameraPos, ref _cameraPos, new string[]
        {
            nameof(CameraPosX),
            nameof(CameraPosZ),
            nameof(ViewportItemOffsetStringized),
        });
        UpdateChunkRegionManager();
    }

    private void UpdateExportArea1(Coords3 exportArea1)
    {
        SetAndNotifyPropertyChanged(exportArea1, ref _exportArea1, new string[]
        {
            nameof(ExportArea1X),
            nameof(ExportArea1Y),
            nameof(ExportArea1Z),
        });
    }

    private void UpdateExportArea2(Coords3 exportArea2)
    {
        SetAndNotifyPropertyChanged(exportArea2, ref _exportArea2, new string[]
        {
            nameof(ExportArea2X),
            nameof(ExportArea2Y),
            nameof(ExportArea2Z),
        });
    }

    #endregion Property Updater Helper

    public ViewportControlVM(ViewportControl control) : base(control)
    {
        App.Current.State.SavegameLoadChanged += OnSavegameLoadChanged;

        _chunkRegionManager = new ChunkRegionManager(this);

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
        MouseWheelEventArgs e = (arg as MouseWheelEventArgs)!;
        int newZoomLevel = ZoomLevel;
        if (e.Delta > 0)
            newZoomLevel++;
        else
            newZoomLevel--;
        newZoomLevel = Math.Clamp(newZoomLevel, 0, MaximumZoomLevel);
        ZoomLevel = newZoomLevel;
    }

    private void OnMouseMove(object? arg)
    {
        MouseEventArgs e = (arg as MouseEventArgs)!;
        PointInt2 oldMousePos = MousePos;
        PointInt2 newMousePos = new((int)e.GetPosition(Control).X,
                                    (int)e.GetPosition(Control).Y);
        MousePos = newMousePos;

        // Set delta to 0 if this call is initial click and dragging.
        // This is to avoid jumps when clicking menu bar
        // then clicking and dragging on the viewer again.
        PointInt2 newMousePosDelta = PointInt2.Zero;
        newMousePosDelta.X = MouseInitClickDrag && MouseClickHolding ? 0 : newMousePos.X - oldMousePos.X;
        newMousePosDelta.Y = MouseInitClickDrag && MouseClickHolding ? 0 : newMousePos.Y - oldMousePos.Y;
        MousePosDelta = newMousePosDelta;

        if (MouseClickHolding)
        {
            // increase division resolution or precision from int to double
            PointF2 newCameraPos = CameraPos;
            newCameraPos -= ((PointF2)MousePosDelta) / PixelPerBlock;
            CameraPos = newCameraPos;
            MouseInitClickDrag = false;
        }
    }

    private void OnMouseUp(object? arg)
    {
        MouseButtonEventArgs e = (MouseButtonEventArgs)arg!;
        if (e.LeftButton == MouseButtonState.Released)
        {
            MouseClickHolding = false;
            MouseInitClickDrag = false;
        }
    }

    private void OnMouseDown(object? arg)
    {
        MouseButtonEventArgs e = (MouseButtonEventArgs)arg!;
        ClearFocus(); // on mouse down any mouse button, yes
        if (e.LeftButton == MouseButtonState.Pressed)
            MouseClickHolding = true;
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

    private void OnKeyUp(object? arg)
    {
        KeyEventArgs e = (arg as KeyEventArgs)!;
        object sender = e.Source;
        if (e.Key == Key.Return && sender is INumericBox)
        {
            ClearFocus();
            e.Handled = true;
        }
    }

    #endregion Commands

    #region Methods

    private void ClearFocus()
    {
        Control.HeightSlider.Focus();
        Keyboard.ClearFocus();
    }

    private void ReinitializeStates()
    {
        ClearFocus();
        CameraPos = PointF2.Zero;
        ZoomLevel = 1;
        HeightLimit = 255;

        ExportArea1 = Coords3.Zero;
        ExportArea2 = Coords3.Zero;

        MousePos = PointInt2.Zero;
        MousePosDelta = PointInt2.Zero;
        MouseClickHolding = false;
        MouseInitClickDrag = true;
        MouseIsOutside = true;

        SidePanelVisible = false;
        SidePanelDebugInfoVisible = false;
    }

    #endregion Methods

    #region Event Handlers

    private void OnSavegameLoadChanged(SavegameLoadState state)
    {
        if (state == SavegameLoadState.Closed)
            _chunkRegionManager.PostMessage(_chunkRegionManager.OnSessionClosed, ChunkRegionManager.MessagePriority.High);
        ReinitializeStates();
    }

    #endregion Event Handlers
}

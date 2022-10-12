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

        _chunkManager = new(this);

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

    private readonly ChunkManager _chunkManager;
    private CoordsRange2 _chunkManagerVisibleChunkRange;

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

    public string ChunkManagerVisibleChunkRangeXBinding => _chunkManager.VisibleChunkRange.XRange.ToString();
    public string ChunkManagerVisibleChunkRangeZBinding => _chunkManager.VisibleChunkRange.ZRange.ToString();
    public int ChunkManagerVisibleChunkCount => _chunkManager.VisibleChunkCount;

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

    private void OnSizeChanged(object? arg)
    {
        _chunkManager.Update();
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
        _chunkManager.Update();
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
        _chunkManager.Update();
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

    public class ChunkManager
    {
        private readonly ViewportControlVM _viewport;
        private bool _needReallocate = false;
        private CoordsRange2 _visibleChunkRange;
        private readonly Dictionary<Coords2, ChunkControl> _buffer = new();

        public ChunkManager(ViewportControlVM viewport)
        {
            _viewport = viewport;
            // below is a dummy chunk for development purpose.
            // delete below lines once chunk reallocation is stable
            Add(new ChunkControl(new PointInt2(0, 0)), new Coords2(0, 0));
            Add(new ChunkControl(new PointInt2(-1, 0)), new Coords2(-1, 0));
            Add(new ChunkControl(new PointInt2(0, -1)), new Coords2(0, -1));
            Add(new ChunkControl(new PointInt2(-1, -1)), new Coords2(-1, -1));
            Update();

        }

        public CoordsRange2 VisibleChunkRange  => _visibleChunkRange;
        public int VisibleChunkCount => _buffer.Count;

        // Add(Chunk chunk)
        private void Add(ChunkControl chunk, Coords2 coords)
        {
            _buffer.Add(coords, chunk);
            chunk.SetRandomImage();
            _viewport.Control.ChunkCanvas.Children.Add(chunk);
        }

        public void Update()
        {
            UpdateVisibleChunkRange();
            UpdateBuffer();
        }

        private void UpdateBuffer()
        {
            if (_needReallocate)
            {
                DeallocateChunk();
                AllocateChunk();
                _needReallocate = false;
                _viewport.NotifyPropertyChanged(nameof(_viewport.ChunkManagerVisibleChunkCount));
            }
            foreach (ChunkControl chunk in _buffer.Values)
            {
                UpdateChunkPosition(chunk);
                UpdateChunkSize(chunk);
            }
        }

        private void UpdateChunkPosition(ChunkControl chunk)
        {
            // we floor all the floating-point number here
            // so it snaps perfectly to the pixel and it removes
            // "Jaggy-Moving" illusion.
            // Try to not floor it and see yourself the illusion

            PointF2 chunkPosOffset = _viewport.ChunkPosOffset.Floor;

            // scaled unit is offset amount required to align the
            // coordinate to zoomed coordinate measured from world origin.
            // Here we are scaling the cartesian coordinate unit by zoom amount
            // (which is pixel-per-chunk)

            PointInt2 scaledUnit = chunk.CanvasPos * _viewport.ViewportPixelPerChunk;

            // Push toward center is offset amount required to align the coordinate
            // relative to the chunk canvas center,
            // so it creates "zoom toward center" effect

            PointF2 pushTowardCenter = _viewport.ViewportChunkCanvasCenter.Floor;

            // Origin offset is offset amount requred to align the coordinate
            // to keep it stays aligned with moved world origin
            // when view is dragged around.
            // The offset itself is from camera position.
            // It is inverted because obviously, if camera is 1 meter to the right
            // of origin, then everything else the camera sees must be 1 meter
            // shifted to the left of the camera

            PointF2 originOffset = -chunkPosOffset;

            PointF2 finalPos
                = (originOffset + scaledUnit + pushTowardCenter).Floor;
            Canvas.SetLeft(chunk, finalPos.X);
            Canvas.SetTop(chunk, finalPos.Y);
        }

        private void UpdateChunkSize(ChunkControl chunk)
        {
            chunk.Width = _viewport.ViewportPixelPerChunk;
        }

        //private void UpdateVisibleChunkRange()
        //{
        //    ViewportControlVM v = _viewport;

        //    // camera chunk in here means which chunk the camera is pointing to
        //    int xCameraChunk = (int)(v._viewportCameraPos.X / Section.BlockCount);
        //    int yCameraChunk = (int)(v._viewportCameraPos.Y / Section.BlockCount);

        //    // min/maxCanvasCenterChunk means which chunk that is visible at the edgemost of the control
        //    // that is measured by the length/height of the control size (which is chunk canvas)
        //    int minXCanvasCenterChunk = (int)(-v.ViewportChunkCanvasCenter.X / v.ViewportPixelPerChunk);
        //    int maxXCanvasCenterChunk = (int)( v.ViewportChunkCanvasCenter.X / v.ViewportPixelPerChunk);

        //    int minX = xCameraChunk + minXCanvasCenterChunk;
        //    int maxX = xCameraChunk + maxXCanvasCenterChunk;
        //    Range visibleChunkXRange = new(minX, maxX);

        //    int minYCanvasCenterChunk = (int)(-v.ViewportChunkCanvasCenter.Y / v.ViewportPixelPerChunk);
        //    int maxYCanvasCenterChunk = (int)( v.ViewportChunkCanvasCenter.Y / v.ViewportPixelPerChunk);

        //    int minZ = yCameraChunk + minYCanvasCenterChunk;
        //    int maxZ = yCameraChunk + maxYCanvasCenterChunk;
        //    Range visibleChunkZRange = new(minZ, maxZ);

        //    _visibleChunkRange = new CoordsRange2(visibleChunkXRange, visibleChunkZRange);
        //}
        private void UpdateVisibleChunkRange()
        {
            ViewportControlVM v = _viewport;

            // camera chunk in here means which chunk the camera is pointing to
            double xCameraChunk = v._viewportCameraPos.X / Section.BlockCount;
            double yCameraChunk = v._viewportCameraPos.Y / Section.BlockCount;

            // min/maxCanvasCenterChunk means which chunk that is visible at the edgemost of the control
            // that is measured by the length/height of the control size (which is chunk canvas)
            double minXCanvasCenterChunk = -(v.ViewportChunkCanvasCenter.X / v.ViewportPixelPerChunk);
            double maxXCanvasCenterChunk = v.ViewportChunkCanvasCenter.X / v.ViewportPixelPerChunk;

            int minX = (int)Math.Floor(Math.Round(xCameraChunk + minXCanvasCenterChunk, 3));
            int maxX = (int)Math.Floor(Math.Round(xCameraChunk + maxXCanvasCenterChunk, 3));
            Range visibleChunkXRange = new(minX, maxX);

            double minYCanvasCenterChunk = -(v.ViewportChunkCanvasCenter.Y / v.ViewportPixelPerChunk);
            double maxYCanvasCenterChunk = v.ViewportChunkCanvasCenter.Y / v.ViewportPixelPerChunk;

            int minZ = (int)Math.Floor(Math.Round(yCameraChunk + minYCanvasCenterChunk, 3));
            int maxZ = (int)Math.Floor(Math.Round(yCameraChunk + maxYCanvasCenterChunk, 3));
            Range visibleChunkZRange = new(minZ, maxZ);

            CoordsRange2 oldVisibleChunkRange = _visibleChunkRange;
            CoordsRange2 newVisibleChunkRange = new (visibleChunkXRange, visibleChunkZRange);
            if (newVisibleChunkRange == oldVisibleChunkRange)
                return;
            _visibleChunkRange = newVisibleChunkRange;
            _needReallocate = true;
            v.NotifyPropertyChanged(nameof(v.ChunkManagerVisibleChunkRangeXBinding));
            v.NotifyPropertyChanged(nameof(v.ChunkManagerVisibleChunkRangeZBinding));
        }

        // TODO CRITICAL deallocation still have memory leaks.
        // We want to completely deallocate everything inside the ChunkControl
        // and make it leave no trace of memory leftover
        private void DeallocateChunk()
        {
            // collect non-visible chunks
            List<Coords2> deallocatedChunks = new();
            foreach (Coords2 chunkCoords in _buffer.Keys)
            {
                if (!_visibleChunkRange.IsInside(chunkCoords))
                    deallocatedChunks.Add(chunkCoords);
            }

            // delete non-visible chunks from buffer and canvas, then dispose it
            foreach (Coords2 deallocatedChunk in deallocatedChunks)
            {
                ChunkControl chunk = _buffer[deallocatedChunk];
                _buffer.Remove(deallocatedChunk);
                _viewport.Control.ChunkCanvas.Children.Remove(chunk);
            }
        }

        private void AllocateChunk()
        {
            // collect newly visible chunks
            List<Coords2> allocatedChunks = new();
            for (int x = _visibleChunkRange.XRange.Min; x <= _visibleChunkRange.XRange.Max; x++)
            {
                for (int z = _visibleChunkRange.ZRange.Min; z <= _visibleChunkRange.ZRange.Max; z++)
                {
                    // if chunk does not exist in buffer, it means
                    // it is newly visible and must be allocated
                    Coords2 chunkCoords = new(x, z);
                    if (!_buffer.ContainsKey(chunkCoords))
                        allocatedChunks.Add(chunkCoords);
                }
            }

            // allocate newly visible chunks
            foreach (Coords2 allocatedChunk in allocatedChunks)
            {
                ChunkControl chunk = new((PointInt2)allocatedChunk);
                chunk.SetRandomImage();
                _buffer.Add(allocatedChunk, chunk);
                _viewport.Control.ChunkCanvas.Children.Add(chunk);
            }
        }
    }
}

/*
Copyright (c) 2022, Bintang Jakasurya
All rights reserved. 

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/

using System;
using System.Threading.Tasks;
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

    // Unit multiplier sequence, index represent how big the multiplier at that zoom level.
    // We use fibonacci sequence to produce natural zoom behaviour
    private static readonly float[] s_unitSequence = new float[] {
        1f, 2f, 3f, 5f, 8f, 13f, 21f, 34f
    };

    private readonly ChunkRegionManager2 _chunkRegionManager;

    private PointF2 _cameraPos = PointF2.Zero;
    private int _zoomLevel = 2;
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
    private int _debugMainThreadResposeTestRandomNumber;

    // Public accessor.
    // Yes, below is a very intricate setup of sea of properties

    // Camera Group \/
    /// <summary>
    /// Camera position is where the camera is currently positioned in world coordinate
    /// </summary>
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

    // Miscellaneous of Viewport Variable Group \/
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
                nameof(UnitOffsetScaledStringized),
            };
            SetAndNotifyPropertyChanged(value, ref _zoomLevel, propNames, UpdateChunkRegionManager);
        }
    }
    public int MaximumZoomLevel => s_unitSequence.Length - 1;
    public int HeightLimit
    {
        get => _heightLimit;
        set => SetAndNotifyPropertyChanged(value, ref _heightLimit, UpdateChunkRegionManager);
    }

    /// <summary>
    /// Screen center is offset amount required to keep a point aligned to the screen center
    /// (given example point is at (0,0)), so it creates "zoom toward center" effect
    /// </summary>
    public PointF2 ScreenCenter => new(Control.ViewportPanel.ActualWidth / 2,
                                       Control.ViewportPanel.ActualHeight / 2);

    /// <summary>
    /// Unit scale is multiplier of the unit depending on zoom level. When zoom level says unit
    /// scale is two, then every point is twice further each other. Example given a point from
    /// world coordinate is at (1,2), when zoom level says unit scale is 3, then the point when being plotted
    /// in screen coordinate is scaled to (3,6)
    /// </summary>
    public float UnitScale => s_unitSequence[ZoomLevel];

    /// <summary>
    /// Unit offset scaled is offset amount requred to align a given point from world coordinate 
    /// when being plotted in screen coordinate to keep it stays aligned with the cartesian coordinate
    /// being constantly transformed by dragged around the view and zoomed in and out.
    /// The offset amout depends on where the camera is positioned in the world coordinate while the scaling
    /// depends on zoom level. It is inverted because obviously, if camera is 1 meter to the right 
    /// of the origin, then everything else the camera sees must be 1 meter shifted  to the left of the camera
    /// </summary>
    public PointF2 UnitOffsetScaled => -(CameraPos * UnitScale);
    public string UnitOffsetScaledStringized => UnitOffsetScaled.ToStringRounded();
    // Miscellaneous of Viewport Variable Group /\

    // How many pixels per group \/
    public float PixelPerBlock => UnitScale; // one block represent one meter/one unit.
                                             // At zoom level zero, one pixel represent one block
    public float PixelPerChunk => PixelPerBlock * Section.BlockCount;
    public float PixelPerRegion => PixelPerChunk * Region.ChunkCount;
    // How many pixels per group /\

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

    public int DebugMainThreadResposeTestRandomNumber => _debugMainThreadResposeTestRandomNumber;

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
        _chunkRegionManager.PostMessage(_chunkRegionManager.Update);
    }

    private void UpdateCameraPos(PointF2 cameraPos)
    {
        SetAndNotifyPropertyChanged(cameraPos, ref _cameraPos, new string[]
        {
            nameof(CameraPosX),
            nameof(CameraPosZ),
            nameof(UnitOffsetScaledStringized),
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

        _chunkRegionManager = new ChunkRegionManager2(this);

        // set commands to its corresponding implementations
        SizeChangedCommand = new RelayCommand(OnSizeChanged);
        MouseWheelCommand = new RelayCommand(OnMouseWheel);
        MouseMoveCommand = new RelayCommand(OnMouseMove);
        MouseUpCommand = new RelayCommand(OnMouseUp);
        MouseDownCommand = new RelayCommand(OnMouseDown);
        MouseLeaveCommand = new RelayCommand(OnMouseLeave);
        MouseEnterCommand = new RelayCommand(OnMouseEnter);
        KeyUpCommand = new RelayCommand(OnKeyUp);

        DebugMainThreadResposeTestGenerateRandomNumber();
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
            MouseInitClickDrag = true;
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

    public PointF2 ConvertWorldPositionToScreenPosition(PointF2 worldPosition)
    {
        // we floor the final position here so it snaps perfectly to the pixel
        // (or the unit) and it removes "Jaggy-Moving" illusion.
        // Try to remove the floor and see yourself the illusion

        // if world pos is 3 at unit scale 1, then it must be 12 at unit scale 4
        PointF2 scaledPos = worldPosition * UnitScale;
        PointF2 finalPos = (UnitOffsetScaled + ScreenCenter + scaledPos).Floor;
        return finalPos;
    }

    private void ClearFocus()
    {
        Control.HeightSlider.Focus();
        Keyboard.ClearFocus();
    }

    private void ReinitializeStates()
    {
        ClearFocus();
        CameraPos = PointF2.Zero;
        ZoomLevel = 0;
        HeightLimit = 255;

        ExportArea1 = Coords3.Zero;
        ExportArea2 = Coords3.Zero;

        MousePos = PointInt2.Zero;
        MousePosDelta = PointInt2.Zero;
        MouseClickHolding = false;
        MouseInitClickDrag = true;
        MouseIsOutside = true;

        SidePanelVisible = true;
        SidePanelDebugInfoVisible = true;
    }

    private async void DebugMainThreadResposeTestGenerateRandomNumber()
    {
        _debugMainThreadResposeTestRandomNumber = 0;

        while (true)
        {
            _debugMainThreadResposeTestRandomNumber++;
            if (_debugMainThreadResposeTestRandomNumber > 1000)
                _debugMainThreadResposeTestRandomNumber = 0;
            NotifyPropertyChanged(nameof(DebugMainThreadResposeTestRandomNumber));
            await Task.Delay(16);
        }
    }

    #endregion Methods

    #region Event Handlers

    private void OnSavegameLoadChanged(SavegameLoadState state)
    {
        if (state == SavegameLoadState.Closed)
            _chunkRegionManager.PostMessage(_chunkRegionManager.OnSavegameLoadClosed);
        ReinitializeStates();
    }

    #endregion Event Handlers
}

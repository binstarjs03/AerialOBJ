using System;
using System.Windows;

namespace binstarjs03.AerialOBJ.WpfAppNew.Components;

public abstract class Viewport
{
    public event Action? CameraPosChanged;
    public event Action? ZoomLevelChanged;
    public event Action? ScreenSizeChanged;

    private Point _cameraPos = new(0, 0);
    private double _zoomLevel = 1;
    private Size _screenSize = new(0, 0);

    public Point CameraPos
    {
        get => _cameraPos;
        set
        {
            if (value != _cameraPos)
            {
                _cameraPos = value;
                Update();
                CameraPosChanged?.Invoke();
            }
        }
    }
    public double ZoomLevel
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
                ZoomLevelChanged?.Invoke();
            }
        }
    }
    public Size ScreenSize
    {
        get => _screenSize;
        set
        {
            if (value != _screenSize)
            {
                if (value.Width == 0 || value.Height == 0)
                    throw new ArgumentException("Screen size cannot be zero", nameof(ScreenSize));
                _screenSize = value;
                Update();
                ScreenSizeChanged?.Invoke();
            }
        }
    }

    public Viewport() { }

    protected abstract void Update();
}

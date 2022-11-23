using System.Drawing;
using System;

using binstarjs03.AerialOBJ.Core.Primitives;

namespace binstarjs03.AerialOBJ.Core.Visualization.TwoDimension;

public abstract class Viewport2
{
    protected event Action? Update;

    private Point2Z<float> _cameraPos = Point2Z<float>.Zero;
    private float _zoomLevel = 1f;
    private Size<int> _screenSize;

    public Point2Z<float> CameraPos
    {
        get => _cameraPos;
        set
        {
            if (value != _cameraPos)
            {
                _cameraPos = value;
                Update?.Invoke();
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
                Update?.Invoke();
            }
        }
    }
    public Size<int> ScreenSize
    {
        get => _screenSize;
        set
        {
            if (value != _screenSize)
            {
                if (value.Width <= 0 || value.Height <= 0)
                    throw new ArgumentException("Screen size cannot be zero or less than", nameof(ScreenSize));
                _screenSize = value;
                Update?.Invoke();
            }
        }
    }

    public Viewport2(Size<int> screenSize)
    {
        Update += OnUpdate;
        ScreenSize = screenSize;
    }

    protected abstract void OnUpdate();
}

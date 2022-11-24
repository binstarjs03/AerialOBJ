using System;

using binstarjs03.AerialOBJ.Core.Primitives;
using System.Runtime.CompilerServices;

namespace binstarjs03.AerialOBJ.Core.Visualization.TwoDimension;

public abstract class Viewport2
{
    public event Action<string>? PropertyChanged;
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
                OnPropertyChanged();
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
                OnPropertyChanged();
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
                OnPropertyChanged();
            }
        }
    }

    public Viewport2(Size<int> screenSize)
    {
        Update += OnUpdate;
        ScreenSize = screenSize;
    }

    protected abstract void OnUpdate();
    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName ="")
    {
        PropertyChanged?.Invoke(propertyName);
    }
}

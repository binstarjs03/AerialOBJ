using System.ComponentModel;

namespace binstarjs03.MineSharpOBJ.WpfApp.UIElements.Controls;

public class ViewportControlVM : ViewModelBase<ViewportControlVM, ViewportControl>
{
    public ViewportControlVM(ViewportControl control) : base(control)
    {
        // listen to shared property change
        SharedProperty.PropertyChanged += OnSharedPropertyChanged;
    }

    // States -----------------------------------------------------------------

    public bool IsViewportCameraPositionGuideVisible
    {
        get => SharedProperty.IsViewportCameraPositionGuideVisible;
        set => SetSharedPropertyChanged
        (
            value,
            SharedProperty.IsViewportCameraPositionGuideVisibleUpdater
        );
    }

    // Event Handlers ---------------------------------------------------------

    protected override void OnSharedPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        string name = e.PropertyName!;
        if (name == nameof(IsViewportCameraPositionGuideVisible))
            NotifyPropertyChanged(nameof(IsViewportCameraPositionGuideVisible));
    }
}

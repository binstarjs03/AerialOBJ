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

    public bool IsViewportDebugInfoVisible
    {
        get => SharedProperty.IsViewportDebugInfoVisible;
        set => SetSharedPropertyChanged
        (
            value, 
            SharedProperty.IsViewportDebugInfoVisibleUpdater
        );
    }

    // Event Handlers ---------------------------------------------------------

    //protected override void OnSharedPropertyChanged(object? sender, PropertyChangedEventArgs e)
    //{
    //    NotifyPropertyChanged(e.PropertyName!);
    //}
}

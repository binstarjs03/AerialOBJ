using binstarjs03.AerialOBJ.Core.Primitives;

using CommunityToolkit.Mvvm.ComponentModel;

namespace binstarjs03.AerialOBJ.WpfApp;
[ObservableObject]
public partial class WindowState
{
    [ObservableProperty][NotifyPropertyChangedFor(nameof(DebugLogWindowPosition))] private Point2<int> _mainWindowPosition;
    [ObservableProperty][NotifyPropertyChangedFor(nameof(DebugLogWindowPosition))] private Size<int> _mainWindowSize;
    [ObservableProperty] private bool _isDebugLogViewVisible = false;

    public Point2<int> DebugLogWindowPosition => new(MainWindowPosition.X + MainWindowSize.Width, MainWindowPosition.Y);
}

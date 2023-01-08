using CommunityToolkit.Mvvm.ComponentModel;

namespace binstarjs03.AerialOBJ.WpfApp;

[ObservableObject]
public partial class ViewState
{
    [ObservableProperty] private bool _isDebugLogViewVisible = false;
}

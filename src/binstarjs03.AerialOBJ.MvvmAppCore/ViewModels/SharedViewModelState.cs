using CommunityToolkit.Mvvm.ComponentModel;

namespace binstarjs03.AerialOBJ.MvvmAppCore.ViewModels;
public partial class SharedViewModelState : ObservableObject
{
    [ObservableProperty] private bool _isDebugLogViewVisible = false;
}

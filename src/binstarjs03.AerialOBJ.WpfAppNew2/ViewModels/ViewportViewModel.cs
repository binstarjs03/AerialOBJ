using binstarjs03.AerialOBJ.WpfAppNew2.Components;

using CommunityToolkit.Mvvm.ComponentModel;

namespace binstarjs03.AerialOBJ.WpfAppNew2.ViewModels;
[ObservableObject]
public partial class ViewportViewModel
{
    private readonly GlobalState _globalState;

    public ViewportViewModel(GlobalState globalState)
    {
        _globalState = globalState;

        _globalState.ViewportDebugPanelVisibilityChanged += GlobalState_ViewportDebugPanelVisibilityChanged;
    }

    public bool IsDebugPanelVisible
    {
        get => _globalState.IsViewportDebugPanelVisible;
        set => _globalState.IsViewportDebugPanelVisible = value;
    }

    private void GlobalState_ViewportDebugPanelVisibilityChanged(bool visible)
    {
        OnPropertyChanged(nameof(IsDebugPanelVisible));
    }
}

using System;

using binstarjs03.AerialOBJ.WpfAppNew.Components;
using binstarjs03.AerialOBJ.WpfAppNew.Services;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace binstarjs03.AerialOBJ.WpfAppNew.ViewModel;

#pragma warning disable CA1822 // Mark members as static
public partial class MainViewModel : BaseViewModel
{
    [ObservableProperty]
    private string _title = SharedStateService.AppName;

    public bool HasSavegameLoaded => SharedStateService.HasSavegameLoaded;

    public bool IsDebugLogWindowVisible
    {
        get => SharedStateService.IsDebugLogWindowVisible;
        set => SharedStateService.IsDebugLogWindowVisible = value;
    }
    public bool IsViewportSidePanelVisible
    {
        get => SharedStateService.IsViewportSidePanelVisible;
        set => SharedStateService.IsViewportSidePanelVisible = value;
    }
    public bool IsViewportSidePanelDebugInfoVisible
    {
        get => SharedStateService.IsViewportSidePanelDebugInfoVisible;
        set => SharedStateService.IsViewportSidePanelDebugInfoVisible = value;
    }

    public MainViewModel()
    {
        SharedStateService.SavegameLoadChanged += OnSavegameLoadChanged;
        SharedStateService.ViewportSidePanelVisibilityChanged += OnViewportSidePanelVisibilityChanged;
        SharedStateService.ViewportSidePanelDebugInfoVisibilityChanged += OnViewportSidePanelDebugInfoVisibilityChanged;
        SharedStateService.DebugLogWindowVisibilityChanged += OnDebugLogWindowVisibilityChanged;
    }

    private void OnDebugLogWindowVisibilityChanged(bool obj) => OnPropertyChanged(nameof(IsDebugLogWindowVisible));
    private void OnViewportSidePanelVisibilityChanged(bool obj) => OnPropertyChanged(nameof(IsViewportSidePanelVisible));
    private void OnViewportSidePanelDebugInfoVisibilityChanged(bool obj) => OnPropertyChanged(nameof(IsViewportSidePanelDebugInfoVisible));

    [RelayCommand]
    private void OnShowAboutModal() => ModalService.ShowAboutModal();

    private void OnSavegameLoadChanged(SavegameLoadState state)
    {
        Title = state switch
        {
            SavegameLoadState.Opened => $"{SharedStateService.AppName} - {SharedStateService.SavegameLoadInfo!.WorldName}",
            SavegameLoadState.Closed => SharedStateService.AppName,
            _ => throw new NotImplementedException()
        };
        OnPropertyChanged(nameof(HasSavegameLoaded));
    }
}
#pragma warning restore CA1822 // Mark members as static

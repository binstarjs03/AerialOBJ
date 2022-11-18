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
    private string _title = StateService.AppName;

    public bool HasSavegameLoaded => StateService.HasSavegameLoaded;

    public bool IsDebugLogWindowVisible
    {
        get => StateService.IsDebugLogWindowVisible;
        set => StateService.IsDebugLogWindowVisible = value;
    }

    public MainViewModel()
    {
        StateService.SavegameLoadChanged += OnSavegameLoadChanged;
        StateService.DebugLogWindowVisibilityChanged += OnDebugLogWindowVisibilityChanged;
    }

    [RelayCommand]
    private void OnShowAboutModal() => ModalService.ShowAboutModal();

    private void OnSavegameLoadChanged(SavegameLoadState state)
    {
        Title = state switch
        {
            SavegameLoadState.Opened => $"{StateService.AppName} - {StateService.SavegameLoadInfo!.WorldName}",
            SavegameLoadState.Closed => StateService.AppName,
            _ => throw new NotImplementedException()
        };
        OnPropertyChanged(nameof(HasSavegameLoaded));
    }

    private void OnDebugLogWindowVisibilityChanged(bool obj)
    {
        OnPropertyChanged(nameof(IsDebugLogWindowVisible));
    }
}
#pragma warning restore CA1822 // Mark members as static

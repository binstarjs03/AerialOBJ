using System;

using binstarjs03.AerialOBJ.WpfAppNew2.Components;
using binstarjs03.AerialOBJ.WpfAppNew2.Services;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace binstarjs03.AerialOBJ.WpfAppNew2.ViewModels;
public partial class MainViewModel : ObservableObject
{
    private readonly GlobalState _globalState;
    private readonly IModalService _modalService;

    public MainViewModel(GlobalState globalState, IModalService modalService)
    {
        _globalState = globalState;
        _modalService = modalService;

        _globalState.DebugLogViewVisibilityChanged += OnDebugLogViewVisibilityChanged;
    }

    public string Title => GlobalState.AppName;

    public bool IsDebugLogViewVisible
    {
        get => _globalState.IsDebugLogWindowVisible;
        set => _globalState.IsDebugLogWindowVisible = value;
    }

    public event Action? CloseViewRequested;

    private void OnDebugLogViewVisibilityChanged(bool visible)
    {
        OnPropertyChanged(nameof(IsDebugLogViewVisible));
    }

    [RelayCommand]
    private void OpenSavegame(string? path)
    {
        _modalService.ShowMessageBox($"Savegame Open Invoked, Path: {path}");
    }

    [RelayCommand]
    private void CloseSavegame(CloseSavegameSender sender)
    {
        _modalService.ShowMessageBox($"Savegame Close Invoked, Sender: {sender}");
    }

    [RelayCommand]
    private void OnClose()
    {
        CloseViewRequested?.Invoke();
    }

    [RelayCommand]
    private void ShowAboutModal()
    {
        _modalService.ShowAbout();
    }
}

public enum CloseSavegameSender
{
    OpenSavegameCommand,
    MenuCloseButton
}
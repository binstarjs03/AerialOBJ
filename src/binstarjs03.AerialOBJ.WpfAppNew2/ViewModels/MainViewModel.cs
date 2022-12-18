using System;

using binstarjs03.AerialOBJ.WpfAppNew2.Components;
using binstarjs03.AerialOBJ.WpfAppNew2.Services;

using CommunityToolkit.Mvvm.Input;

namespace binstarjs03.AerialOBJ.WpfAppNew2.ViewModels;
public partial class MainViewModel
{
    private readonly IModalService _modalService;

    public GlobalState GlobalState { get; }

    public MainViewModel(GlobalState globalState, IModalService modalService)
    {
        _modalService = modalService;
        GlobalState = globalState;
    }

    public event Action? CloseRequested;

    [RelayCommand]
    private void OnOpenSavegame(string? path)
    {
        _modalService.ShowMessageBox($"Savegame Open Invoked, Path: {path}");
    }

    [RelayCommand]
    private void OnCloseSavegame(CloseSavegameSender sender)
    {
        _modalService.ShowMessageBox($"Savegame Close Invoked, Sender: {sender}");
    }

    [RelayCommand]
    private void OnClose()
    {
        CloseRequested?.Invoke();
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
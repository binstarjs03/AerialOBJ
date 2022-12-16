using System;

using binstarjs03.AerialOBJ.WpfAppNew2.Components;

using CommunityToolkit.Mvvm.Input;

namespace binstarjs03.AerialOBJ.WpfAppNew2.ViewModels;
public partial class MainViewModel : IViewModelBase, ICloseCommand, IShowMessageBox
{
    public MainViewModel(GlobalState globalState)
    {
        GlobalState = globalState;
    }

    public GlobalState GlobalState { get; }

    public event Action? CloseRequested;
    public event Action<string>? ShowMessageBoxRequested;

    [RelayCommand]
    private void OnOpenSavegame(string? path)
    {
        ShowMessageBoxRequested?.Invoke($"Savegame Open Invoked, Path: {path}");
    }

    [RelayCommand]
    private void OnCloseSavegame(CloseSavegameSender sender)
    {
        ShowMessageBoxRequested?.Invoke($"Savegame Close Invoked, Sender: {sender}");
    }

    [RelayCommand]
    private void OnClose()
    {
        CloseRequested?.Invoke();
    }
}

public enum CloseSavegameSender
{
    OpenSavegameCommand,
    MenuCloseButton
}
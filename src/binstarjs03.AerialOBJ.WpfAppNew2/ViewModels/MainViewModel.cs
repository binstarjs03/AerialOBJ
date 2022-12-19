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
    private readonly ILogService _logService;
    private readonly ISavegameLoaderService _savegameLoaderService;

    public MainViewModel(GlobalState globalState, IModalService modalService, ILogService logService, ISavegameLoaderService savegameLoaderService)
    {
        _globalState = globalState;
        _modalService = modalService;
        _logService = logService;
        _savegameLoaderService = savegameLoaderService;
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
        if (path is null)
        {
            _logService.Log("Attempting to load savegame...");
            FolderDialogResult result = _modalService.ShowFolderBrowserDialog();
            if (!result.Result)
            {
                _logService.Log("Dialog cancelled. Loading savegame aborted",
                                LogStatus.Aborted,
                                useSeparator: true);
                return;
            }
            path = result.SelectedDirectoryPath;
        }
        _logService.Log($"Selected path: \"{path}\"");
        _logService.Log($"Loading selected path as Minecraft savegame folder...");
        try
        {
            SavegameLoadInfo? loadInfo = _savegameLoaderService.LoadSavegame(path);
        }
        catch (Exception e)
        {
            _logService.Log(e.Message, LogStatus.Error);
            _logService.Log("Exception Details:");
            _logService.Log(e.ToString());
            _logService.Log("Loading savegame aborted",
                            LogStatus.Aborted,
                            useSeparator: true);

            _modalService.ShowErrorMessageBox(new MessageBoxArg()
            {
                Caption = "Error Opening Minecraft Savegame",
                Message = e.Message,
            });
        }
    }

    [RelayCommand]
    private void CloseSavegame(CloseSavegameSender sender)
    {
        _modalService.ShowMessageBox(new MessageBoxArg()
        {
            Caption = "Information",
            Message = $"Savegame Close Invoked, Sender: {sender}"
        });
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
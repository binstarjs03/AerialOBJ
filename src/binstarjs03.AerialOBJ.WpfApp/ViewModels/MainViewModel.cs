using System;

using binstarjs03.AerialOBJ.WpfApp.Components;
using binstarjs03.AerialOBJ.WpfApp.Services;
using binstarjs03.AerialOBJ.WpfApp.Services.ModalServices;
using binstarjs03.AerialOBJ.WpfApp.Services.SavegameLoaderServices;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace binstarjs03.AerialOBJ.WpfApp.ViewModels;
[ObservableObject]
public partial class MainViewModel
{
    private readonly IModalService _modalService;
    private readonly ILogService _logService;
    private readonly ISavegameLoaderService _savegameLoaderService;

    [ObservableProperty]
    private string _title = GlobalState.AppName;

    public MainViewModel(GlobalState globalState, IModalService modalService, ILogService logService, ISavegameLoaderService savegameLoaderService)
    {
        GlobalState = globalState;
        _modalService = modalService;
        _logService = logService;
        _savegameLoaderService = savegameLoaderService;

        GlobalState.PropertyChanged += OnPropertyChanged;
        GlobalState.SavegameLoadChanged += GlobalState_SavegameLoadChanged;
    }

    public GlobalState GlobalState { get; }

    public event Action? CloseViewRequested;

    private void GlobalState_SavegameLoadChanged(SavegameLoadState state)
    {
        Title = state switch
        {
            SavegameLoadState.Opened => $"{GlobalState.AppName} - {GlobalState.SavegameLoadInfo!.WorldName}",
            SavegameLoadState.Closed => GlobalState.AppName,
            _ => throw new NotImplementedException(),
        };
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
            SavegameLoadInfo loadInfo = _savegameLoaderService.LoadSavegame(path);
            // close savegame if already loaded
            if (GlobalState.HasSavegameLoaded)
            {
                _logService.Log("Savegame already loaded, closing...");
                CloseSavegame(CloseSavegameSender.OpenSavegameCommand);
            }
            GlobalState.SavegameLoadInfo = loadInfo;
            _logService.Log($"Successfully loaded {loadInfo.WorldName}", useSeparator: true);
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
        // default worldname if there is no savegame loaded
        string worldName = "savegame";
        if (GlobalState.SavegameLoadInfo is not null)
            worldName = GlobalState.SavegameLoadInfo.WorldName;

        GlobalState.SavegameLoadInfo = null;
        bool useSeparator = sender == CloseSavegameSender.MenuCloseButton;
        _logService.Log($"Successfully closed {worldName}", useSeparator);
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
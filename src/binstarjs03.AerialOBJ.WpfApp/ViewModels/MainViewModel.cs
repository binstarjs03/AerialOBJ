using System;

using binstarjs03.AerialOBJ.WpfApp.Components;
using binstarjs03.AerialOBJ.WpfApp.Services;
using binstarjs03.AerialOBJ.WpfApp.Services.ModalServices;
using binstarjs03.AerialOBJ.WpfApp.Services.SavegameLoaderServices;
using binstarjs03.AerialOBJ.WpfApp.Views;

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

    public MainViewModel(GlobalState globalState,
                         ViewState viewState,
                         IModalService modalService,
                         ILogService logService,
                         ISavegameLoaderService savegameLoaderService,
                         IView viewportView)
    {
        GlobalState = globalState;
        ViewState = viewState;
        _modalService = modalService;
        _logService = logService;
        _savegameLoaderService = savegameLoaderService;

        GlobalState.PropertyChanged += OnPropertyChanged;
        GlobalState.SavegameLoadChanged += GlobalState_SavegameLoadChanged;
        ViewState.PropertyChanged += OnPropertyChanged;
        ViewportView = viewportView;
    }

    public GlobalState GlobalState { get; }
    public ViewState ViewState { get; }
    public IView ViewportView { get; }
    public IClosableView? View { get; set; }

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
        if (path is null && !isPathConfirmedFromBrowserDialog(out path))
            return;
        try
        {
            SavegameLoadInfo loadInfo = _savegameLoaderService.LoadSavegame(path);
            // close savegame if already loaded
            if (GlobalState.HasSavegameLoaded)
                CloseSavegame(CloseSavegameSender.OpenSavegameCommand);
            GlobalState.SavegameLoadInfo = loadInfo;
        }
        catch (Exception e) { handleException(e); }

        bool isPathConfirmedFromBrowserDialog(out string path)
        {
            FolderDialogResult result = _modalService.ShowFolderBrowserDialog();
            path = result.SelectedDirectoryPath;
            return result.Result;
        }

        void handleException(Exception e)
        {
            _logService.LogException($"Cannot open {path}", e, "Loading savegame aborted");
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
        GlobalState.SavegameLoadInfo = null;
    }

    [RelayCommand]
    private void OnClose(CloseViewSender sender)
    {
        GlobalState.SavegameLoadInfo = null;
        if (sender == CloseViewSender.MenuExitButton)
            View?.Close();
    }

    [RelayCommand]
    private void ShowAboutModal() => _modalService.ShowAboutView();

    [RelayCommand]
    private void ShowDefinitionManagerModal() => _modalService.ShowDefinitionManagerView();

}

public enum CloseSavegameSender
{
    OpenSavegameCommand,
    MenuCloseButton,
}

public enum CloseViewSender
{
    WindowCloseButton,
    MenuExitButton,
}
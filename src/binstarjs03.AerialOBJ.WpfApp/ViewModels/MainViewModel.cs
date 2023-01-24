using System;

using binstarjs03.AerialOBJ.WpfApp.Models;
using binstarjs03.AerialOBJ.WpfApp.Services;
using binstarjs03.AerialOBJ.WpfApp.Services.IOService.SavegameLoader;
using binstarjs03.AerialOBJ.WpfApp.Services.ModalServices;
using binstarjs03.AerialOBJ.WpfApp.Views;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace binstarjs03.AerialOBJ.WpfApp.ViewModels;

[ObservableObject]
public partial class MainViewModel : IMainViewModel
{
    private readonly AbstractViewModel _abstractViewModel;
    private readonly IModalService _modalService;
    private readonly ILogService _logService;
    private readonly ISavegameLoader _savegameLoaderService;

    [ObservableProperty]
    private string _title = GlobalState.AppName;

    public MainViewModel(GlobalState globalState,
                         ViewState viewState,
                         AbstractViewModel abstractViewModel,
                         IModalService modalService,
                         ILogService logService,
                         ISavegameLoader savegameLoaderService,
                         IView viewportView)
    {
        GlobalState = globalState;
        ViewState = viewState;
        _abstractViewModel = abstractViewModel;
        _modalService = modalService;
        _logService = logService;
        _savegameLoaderService = savegameLoaderService;

        GlobalState.SavegameLoadInfoChanged += OnGlobalState_SavegameLoadChanged;
        ViewportView = viewportView;
    }

    public GlobalState GlobalState { get; }
    public ViewState ViewState { get; }
    public IView ViewportView { get; }

    private void OnGlobalState_SavegameLoadChanged(SavegameLoadState state)
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
                CloseSavegame();
            GlobalState.SavegameLoadInfo = loadInfo;
        }
        catch (Exception e) { handleException(e); }

        bool isPathConfirmedFromBrowserDialog(out string path)
        {
            FolderDialogResult result = _modalService.ShowFolderBrowserDialog();
            path = result.SelectedDirectoryPath;
            return result.Confirmed;
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
    private void CloseSavegame() => GlobalState.SavegameLoadInfo = null;

    [RelayCommand]
    private void Close(IClosableView view)
    {
        OnClosing();
        _abstractViewModel.CloseCommand.Execute(view);
    }

    [RelayCommand]
    private void OnClosing() => CloseSavegame();

    [RelayCommand]
    private void ShowAboutModal() => _modalService.ShowAboutView();

    [RelayCommand]
    private void ShowDefinitionManagerModal() => _modalService.ShowDefinitionManagerView();
}
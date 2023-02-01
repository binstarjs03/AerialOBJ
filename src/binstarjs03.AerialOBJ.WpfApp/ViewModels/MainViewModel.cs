using System;
using System.Threading;
using System.Threading.Tasks;

using binstarjs03.AerialOBJ.Core;
using binstarjs03.AerialOBJ.WpfApp.Models;
using binstarjs03.AerialOBJ.WpfApp.Services;
using binstarjs03.AerialOBJ.WpfApp.Services.Diagnostics;
using binstarjs03.AerialOBJ.WpfApp.Services.IOService.SavegameLoader;
using binstarjs03.AerialOBJ.WpfApp.Services.ModalServices;
using binstarjs03.AerialOBJ.WpfApp.Views;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace binstarjs03.AerialOBJ.WpfApp.ViewModels;

[ObservableObject]
public partial class MainViewModel
{
    private readonly AbstractViewModel _abstractViewModel;
    private readonly IModalService _modalService;
    private readonly ILogService _logService;
    private readonly ISavegameLoader _savegameLoaderService;

    [ObservableProperty]
    private string _title;

    public MainViewModel(AppInfo appInfo,
                         GlobalState globalState,
                         ViewState viewState,
                         AbstractViewModel abstractViewModel,
                         IModalService modalService,
                         ILogService logService,
                         ISavegameLoader savegameLoaderService,
                         IMemoryInfo memoryInfo)
    {
        AppInfo = appInfo;
        GlobalState = globalState;
        ViewState = viewState;
        _abstractViewModel = abstractViewModel;
        _modalService = modalService;
        _logService = logService;
        _savegameLoaderService = savegameLoaderService;
        MemoryInfo = memoryInfo;
        _title = AppInfo.AppName;

        GlobalState.SavegameLoadInfoChanged += OnGlobalState_SavegameLoadChanged;
        MemoryInfo.MemoryInfoUpdated += OnMemoryInfoUpdated;
    }

    public AppInfo AppInfo { get; }
    public GlobalState GlobalState { get; }
    public ViewState ViewState { get; }
    public IMemoryInfo MemoryInfo { get; }

    public string UsedMemory => MathUtils.DataUnitToString(MemoryInfo.MemoryUsedSize);
    public string AllocatedMemory => MathUtils.DataUnitToString(MemoryInfo.MemoryAllocatedSize);

    private void OnGlobalState_SavegameLoadChanged(SavegameLoadState state)
    {
        if (state == SavegameLoadState.Opened)
            MemoryInfo.StartMonitorMemory();
        else
            MemoryInfo.StopMonitorMemory();

        Title = state switch
        {
            SavegameLoadState.Opened => $"{AppInfo.AppName} - {GlobalState.SavegameLoadInfo!.WorldName}",
            SavegameLoadState.Closed => AppInfo.AppName,
            _ => throw new NotImplementedException(),
        };
    }

    private void OnMemoryInfoUpdated()
    {
        OnPropertyChanged(nameof(UsedMemory));
        OnPropertyChanged(nameof(AllocatedMemory));
    }

    [RelayCommand]
    private void OpenSavegame(string? path)
    {
        SavegameLoadInfo loadInfo;
        if (path is null && !isPathConfirmedFromBrowserDialog(out path))
            return;

        try
        {
            loadInfo = _savegameLoaderService.LoadSavegame(path);
        }
        catch (Exception e)
        {
            handleException(e);
            return;
        }

        // close savegame if already loaded
        if (GlobalState.HasSavegameLoaded)
            CloseSavegame();
        GlobalState.SavegameLoadInfo = loadInfo;

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
    private void CloseSavegame()
    {
        GlobalState.SavegameLoadInfo = null;
    }

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

    [RelayCommand]
    private void ShowSettingModal() => _modalService.ShowSettingView();
}
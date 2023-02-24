using System;

using binstarjs03.AerialOBJ.Core;
using binstarjs03.AerialOBJ.MvvmAppCore.Models;
using binstarjs03.AerialOBJ.MvvmAppCore.Services;
using binstarjs03.AerialOBJ.MvvmAppCore.Services.Diagnostics;
using binstarjs03.AerialOBJ.MvvmAppCore.Services.IOService.SavegameLoader;
using binstarjs03.AerialOBJ.MvvmAppCore.Services.ModalServices;
using binstarjs03.AerialOBJ.MvvmAppCore.ViewTraits;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace binstarjs03.AerialOBJ.MvvmAppCore.ViewModels;

public partial class MainViewModel : ObservableObject, IGotoViewModelClosedRecipient
{
    private readonly AppInfo _appInfo;
    private readonly IModalService _modalService;
    private readonly ILogService _logService;
    private readonly ISavegameLoader _savegameLoaderService;
    private readonly IMemoryInfo _memoryInfo;

    [ObservableProperty]
    private string _title;
    private bool _isGotoWindowShown;

    public MainViewModel(AppInfo appInfo,
                         GlobalState globalState,
                         SharedViewModelState sharedViewModelState,
                         IModalService modalService,
                         ILogService logService,
                         ISavegameLoader savegameLoaderService,
                         IMemoryInfo memoryInfo)
    {
        _appInfo = appInfo;
        GlobalState = globalState;
        SharedViewModelState = sharedViewModelState;
        _modalService = modalService;
        _logService = logService;
        _savegameLoaderService = savegameLoaderService;
        _memoryInfo = memoryInfo;
        _title = _appInfo.AppName;

        GlobalState.SavegameLoadInfoChanged += OnGlobalState_SavegameLoadChanged;
        _memoryInfo.MemoryInfoUpdated += OnMemoryInfoUpdated;
        //ViewportViewModel = viewportViewModel;
    }

    public IClosable? Closable { get; set; }

    public GlobalState GlobalState { get; }
    public SharedViewModelState SharedViewModelState { get; }
    //public ViewportViewModel ViewportViewModel { get; }

    public string UsedMemory => MathUtils.DataUnitToString(_memoryInfo.MemoryUsedSize);
    public string AllocatedMemory => MathUtils.DataUnitToString(_memoryInfo.MemoryAllocatedSize);

    private void OnGlobalState_SavegameLoadChanged(SavegameLoadInfo? info)
    {
        if (info is not null)
            _memoryInfo.StartMonitorMemory();
        else
            _memoryInfo.StopMonitorMemory();

        Title = info switch
        {
            not null => $"{_appInfo.AppName} - {GlobalState.SavegameLoadInfo!.WorldName}",
            null => _appInfo.AppName,
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
    private void CloseSavegame() => GlobalState.SavegameLoadInfo = null;

    [RelayCommand]
    private void CloseWindow()
    {
        OnWindowClosing();
        Closable?.Close();
    }

    [RelayCommand]
    private void OnWindowClosing() => CloseSavegame();

    [RelayCommand]
    private void ShowAboutWindow() => _modalService.ShowAboutWindow();

    [RelayCommand]
    private void ShowDefinitionManagerWindow() => _modalService.ShowDefinitionManagerWindow();

    [RelayCommand]
    private void ShowSettingWindow() => _modalService.ShowSettingWindow();

    [RelayCommand]
    private void ShowGotoWindow()
    {
        if (_isGotoWindowShown)
            return;
        _isGotoWindowShown = true;
        _modalService.ShowGotoWindow();
    }

    void IGotoViewModelClosedRecipient.Notify() => _isGotoWindowShown = false;
}
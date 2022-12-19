using System;

using binstarjs03.AerialOBJ.WpfAppNew2.Components;
using binstarjs03.AerialOBJ.WpfAppNew2.Services;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace binstarjs03.AerialOBJ.WpfAppNew2.ViewModels;
public partial class DebugLogViewModel : ObservableObject
{
    private readonly GlobalState _globalState;
    private readonly ILogService _logService;
    private readonly IModalService _modalService;
    private readonly IIOService _iOService;

    public DebugLogViewModel(GlobalState globalState, ILogService logService, IModalService modalService, IIOService iOService)
    {
        _globalState = globalState;
        _logService = logService;
        _modalService = modalService;
        _iOService = iOService;
        _globalState.DebugLogViewVisibilityChanged += OnVisibilityChanged;
        _logService.Logging += OnLogServiceLogging;
    }

    public event Action? CloseViewRequested;
    public event Action? ScrollToEndRequested;

    public bool IsVisible
    {
        get => _globalState.IsDebugLogWindowVisible;
        set => _globalState.IsDebugLogWindowVisible = value;
    }

    public string LogContent => _logService.LogContent;

    private void OnVisibilityChanged(bool isVisible)
    {
        OnPropertyChanged(nameof(IsVisible));
    }

    private void OnLogServiceLogging(string message, LogStatus status)
    {
        OnPropertyChanged(nameof(LogContent));
        ScrollToEndRequested?.Invoke();
    }

    [RelayCommand]
    private void ClearLog()
    {
        _logService.Clear();
        _logService.LogRuntimeInfo();
    }

    [RelayCommand]
    private void SaveLog()
    {
        SaveFileDialogResult dialogResult = _modalService.ShowSaveFileDialog(new SaveFileDialogArg()
        {
            FileName = $"{GlobalState.AppName} Log",
            FileExtension = ".txt",
            FileExtensionFilter = "Text Document (.txt)|*.txt",
        });
        if (dialogResult.Result != true)
            return;
        _iOService.WriteText(dialogResult.Path, _logService.LogContent, out Exception? e);
        if (e is not null)
            _modalService.ShowErrorMessageBox(new MessageBoxArg()
            {
                Caption = "Canot save log",
                Message = $"Canot save log content to {dialogResult.Path}:\n{e}",
            });
    }

    [RelayCommand]
    private void CloseView()
    {
        CloseViewRequested?.Invoke();
    }
}

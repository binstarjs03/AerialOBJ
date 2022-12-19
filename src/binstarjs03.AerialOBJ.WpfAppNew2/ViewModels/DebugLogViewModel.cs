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

    public DebugLogViewModel(GlobalState globalState, ILogService logService, IModalService modalService)
    {
        _globalState = globalState;
        _logService = logService;
        _modalService = modalService;
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
    }

    [RelayCommand]
    private void SaveLog()
    {
        _modalService.ShowMessageBox("Saved log content");
    }

    [RelayCommand]
    private void CloseView()
    {
        CloseViewRequested?.Invoke();
    }
}

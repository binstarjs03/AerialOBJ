using System;

using binstarjs03.AerialOBJ.WpfApp.Services;
using binstarjs03.AerialOBJ.WpfApp.Services.ModalServices;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace binstarjs03.AerialOBJ.WpfApp.ViewModels;
[ObservableObject]
public partial class DebugLogViewModel
{
    private readonly IModalService _modalService;
    private readonly IIOService _iOService;

    public DebugLogViewModel(GlobalState globalState, ViewState viewState, ILogService logService, IModalService modalService, IIOService iOService)
    {
        GlobalState = globalState;
        ViewState = viewState;
        LogService = logService;
        _modalService = modalService;
        _iOService = iOService;

        GlobalState.PropertyChanged += OnPropertyChanged;
        ViewState.PropertyChanged += OnPropertyChanged;
        LogService.Logging += OnLogServiceLogging;
    }

    public GlobalState GlobalState { get; }
    public ViewState ViewState { get; }
    public ILogService LogService { get; }

    public event Action? RequestScrollToEnd;

    private void OnLogServiceLogging(string message, LogStatus status)
    {
        OnPropertyChanged(nameof(LogService));
        RequestScrollToEnd?.Invoke();
    }

    [RelayCommand]
    private void ClearLog()
    {
        LogService.Clear();
        LogService.LogRuntimeInfo();
    }

    [RelayCommand]
    private void SaveLog()
    {
        FileDialogResult dialogResult = _modalService.ShowSaveFileDialog(new FileDialogArg()
        {
            FileName = $"{GlobalState.AppName} Log",
            FileExtension = ".txt",
            FileExtensionFilter = "Text Document|*.txt",
        });
        if (dialogResult.Result != true)
            return;
        _iOService.WriteText(dialogResult.SelectedFilePath, LogService.LogContent, out Exception? e);
        if (e is not null)
        {
            string msg = $"Canot save log content to {dialogResult.SelectedFilePath}:\n{e}";
            LogService.Log(msg, LogStatus.Error);
            _modalService.ShowErrorMessageBox(new MessageBoxArg()
            {
                Caption = "Canot save log",
                Message = msg,
            });
        }
        else
        {
            LogService.Log($"Saved log content to {dialogResult.SelectedFilePath}", LogStatus.Success);
        }
    }
}

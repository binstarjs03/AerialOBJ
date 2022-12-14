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

    public DebugLogViewModel(GlobalState globalState,
                             ViewState viewState,
                             ILogService logService,
                             IModalService modalService,
                             IIOService iOService)
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
        if (!isSavePathConfirmedFromFileDialog(out string path))
            return;
        try
        {
            _iOService.WriteText(path, LogService.LogContent);
            LogService.Log($"Saved log content to {path}", LogStatus.Success);
        }
        catch (Exception e) { handleException(e); }

        bool isSavePathConfirmedFromFileDialog(out string path)
        {
            FileDialogResult dialogResult = _modalService.ShowSaveFileDialog(new FileDialogArg()
            {
                FileName = $"{GlobalState.AppName} Log",
                FileExtension = ".txt",
                FileExtensionFilter = "Text Document|*.txt",
            });
            path = dialogResult.SelectedFilePath;
            return dialogResult.Result;
        }

        void handleException(Exception e)
        {
            string caption = "Cannot save log content";
            LogService.LogException(caption, e);
            _modalService.ShowErrorMessageBox(new MessageBoxArg()
            {
                Caption = caption,
                Message = e.Message,
            });
        }
    }
}

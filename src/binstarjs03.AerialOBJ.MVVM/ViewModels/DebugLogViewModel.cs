using System;

using binstarjs03.AerialOBJ.MVVM.Services;
using binstarjs03.AerialOBJ.MVVM.Services.IOService;
using binstarjs03.AerialOBJ.MVVM.Services.ModalServices;
using binstarjs03.AerialOBJ.MVVM.ViewTraits;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace binstarjs03.AerialOBJ.MVVM.ViewModels;
public partial class DebugLogViewModel : ObservableObject
{
    private readonly IModalService _modalService;
    private readonly IAbstractIO _abstractIO;
    private readonly ILogService _logService;

    public DebugLogViewModel(AppInfo appInfo,
                             SharedViewModelState sharedViewModelState,
                             ILogService logService,
                             IModalService modalService,
                             IAbstractIO abstractIO)
    {
        AppInfo = appInfo;
        SharedViewModelState = sharedViewModelState;
        _logService = logService;
        _modalService = modalService;
        _abstractIO = abstractIO;
        _logService.Logging += OnLogServiceLogging;
    }

    public IClosable? Closable { get; set; }
    public IScrollable? Scrollable { get; set; }

    public AppInfo AppInfo { get; }
    public SharedViewModelState SharedViewModelState { get; }
    public string LogContent => _logService.LogContent;

    private void OnLogServiceLogging()
    {
        OnPropertyChanged(nameof(LogContent));
        Scrollable?.ScrollToEnd();
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
        if (!isSavePathConfirmedFromFileDialog(out string path))
            return;
        try
        {
            _abstractIO.WriteText(path, _logService.LogContent);
            _logService.Log($"Saved log content to {path}", LogStatus.Success);
        }
        catch (Exception e) { handleException(e); }

        bool isSavePathConfirmedFromFileDialog(out string path)
        {
            FileDialogResult dialogResult = _modalService.ShowSaveFileDialog(new FileDialogArg
            {
                FileName = $"{AppInfo.AppName} Log",
                FileExtension = ".txt",
                FileExtensionFilter = "Text Document|*.txt",
            });
            path = dialogResult.SelectedFilePath;
            return dialogResult.Confirmed;
        }

        void handleException(Exception e)
        {
            string caption = "Cannot save log content";
            _logService.LogException(caption, e);
            _modalService.ShowErrorMessageBox(new MessageBoxArg
            {
                Caption = caption,
                Message = e.Message,
            });
        }
    }

    [RelayCommand]
    private void Close() => Closable?.Close();
}

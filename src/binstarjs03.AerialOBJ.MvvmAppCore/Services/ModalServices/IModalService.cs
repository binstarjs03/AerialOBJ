namespace binstarjs03.AerialOBJ.MvvmAppCore.Services.ModalServices;
public interface IModalService
{
    void ShowMessageBox(MessageBoxArg dialogArg);
    void ShowErrorMessageBox(MessageBoxArg dialogArg);
    void ShowWarningMessageBox(MessageBoxArg dialogArg);
    bool ShowConfirmationBox(MessageBoxArg dialogArg);
    bool ShowWarningConfirmationBox(MessageBoxArg dialogArg);
    void ShowAboutWindow();
    void ShowDefinitionManagerWindow();
    void ShowSettingWindow();
    void ShowGotoWindow();
    FileDialogResult ShowSaveFileDialog(FileDialogArg dialogArg);
    FileDialogResult ShowOpenFileDialog(FileDialogArg dialogArg);
    FolderDialogResult ShowFolderBrowserDialog();
}


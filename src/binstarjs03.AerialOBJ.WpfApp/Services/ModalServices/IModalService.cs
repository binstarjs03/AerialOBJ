namespace binstarjs03.AerialOBJ.WpfApp.Services.ModalServices;
public interface IModalService
{
    void ShowMessageBox(MessageBoxArg dialogArg);
    void ShowErrorMessageBox(MessageBoxArg dialogArg);
    bool ShowConfirmationBox(MessageBoxArg dialogArg);
    bool ShowWarningConfirmationBox(MessageBoxArg dialogArg);
    void ShowAboutView();
    void ShowDefinitionManagerView();
    FileDialogResult ShowSaveFileDialog(FileDialogArg dialogArg);
    FileDialogResult ShowOpenFileDialog(FileDialogArg dialogArg);
    FolderDialogResult ShowFolderBrowserDialog();
}


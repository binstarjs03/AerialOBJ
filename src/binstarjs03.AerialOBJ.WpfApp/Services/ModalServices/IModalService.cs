namespace binstarjs03.AerialOBJ.WpfApp.Services.ModalServices;
public interface IModalService
{
    void ShowMessageBox(MessageBoxArg dialogArg);
    void ShowErrorMessageBox(MessageBoxArg dialogArg);
    void ShowAbout();
    void ShowDefinitionManager();
    FileDialogResult ShowSaveFileDialog(FileDialogArg dialogArg);
    FolderDialogResult ShowFolderBrowserDialog();
}


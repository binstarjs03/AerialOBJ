namespace binstarjs03.AerialOBJ.WpfAppNew2.Services;
public interface IModalService
{
    void ShowMessageBox(MessageBoxArg dialogArg);
    void ShowErrorMessageBox(MessageBoxArg dialogArg);
    void ShowAbout();
    FileDialogResult ShowSaveFileDialog(FileDialogArg dialogArg);
    FolderDialogResult ShowFolderBrowserDialog();
}


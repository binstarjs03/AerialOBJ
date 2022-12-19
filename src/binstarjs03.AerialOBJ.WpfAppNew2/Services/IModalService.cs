namespace binstarjs03.AerialOBJ.WpfAppNew2.Services;
public interface IModalService
{
    void ShowMessageBox(MessageBoxArg dialogArg);
    void ShowErrorMessageBox(MessageBoxArg dialogArg);
    void ShowAbout();
    SaveFileDialogResult ShowSaveFileDialog(SaveFileDialogArg dialogArg);
    FolderBrowserDialogResult ShowFolderBrowserDialog();
}


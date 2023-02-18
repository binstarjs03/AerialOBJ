using System.Windows;

using binstarjs03.AerialOBJ.MVVM.Services.ModalServices;

using Ookii.Dialogs.Wpf;

namespace binstarjs03.AerialOBJ.WpfApp.Services;
public class ModalService : IModalService
{
    public void ShowAboutWindow()
    {
        throw new System.NotImplementedException();
    }

    public bool ShowConfirmationBox(MessageBoxArg dialogArg)
    {
        throw new System.NotImplementedException();
    }

    public void ShowDefinitionManagerWindow()
    {
        throw new System.NotImplementedException();
    }

    public void ShowErrorMessageBox(MessageBoxArg dialogArg)
    {
        MessageBox.Show(dialogArg.Message, dialogArg.Caption, MessageBoxButton.OK, MessageBoxImage.Error);
    }

    public FolderDialogResult ShowFolderBrowserDialog()
    {
        throw new System.NotImplementedException();
    }

    public void ShowGotoWindow()
    {
        throw new System.NotImplementedException();
    }

    public void ShowMessageBox(MessageBoxArg dialogArg)
    {
        throw new System.NotImplementedException();
    }

    public FileDialogResult ShowOpenFileDialog(FileDialogArg dialogArg)
    {
        throw new System.NotImplementedException();
    }

    public FileDialogResult ShowSaveFileDialog(FileDialogArg dialogArg)
    {
        VistaSaveFileDialog dialog = new()
        {
            FileName = dialogArg.FileName,
            DefaultExt = dialogArg.FileExtension,
            Filter = dialogArg.FileExtensionFilter
        };
        bool? result = dialog.ShowDialog();
        return new FileDialogResult()
        {
            SelectedFilePath = dialog.FileName,
            Confirmed = result == true,
        };
    }

    public void ShowSettingWindow()
    {
        throw new System.NotImplementedException();
    }

    public bool ShowWarningConfirmationBox(MessageBoxArg dialogArg)
    {
        throw new System.NotImplementedException();
    }

    public void ShowWarningMessageBox(MessageBoxArg dialogArg)
    {
        throw new System.NotImplementedException();
    }
}

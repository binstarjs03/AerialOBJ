using System.Windows;

using binstarjs03.AerialOBJ.MVVM.Services.ModalServices;
using binstarjs03.AerialOBJ.WpfApp.Views;

using Microsoft.Extensions.DependencyInjection;

using Ookii.Dialogs.Wpf;

namespace binstarjs03.AerialOBJ.WpfApp.Services;
public class ModalService : IModalService
{
    public void ShowMessageBox(MessageBoxArg dialogArg)
    {
        MessageBox.Show(dialogArg.Message, dialogArg.Caption, MessageBoxButton.OK);
    }

    public void ShowWarningMessageBox(MessageBoxArg dialogArg)
    {
        MessageBox.Show(dialogArg.Message, dialogArg.Caption, MessageBoxButton.OK, MessageBoxImage.Warning);
    }

    public void ShowErrorMessageBox(MessageBoxArg dialogArg)
    {
        MessageBox.Show(dialogArg.Message, dialogArg.Caption, MessageBoxButton.OK, MessageBoxImage.Error);
    }

    public bool ShowConfirmationBox(MessageBoxArg dialogArg)
    {
        MessageBoxResult result = MessageBox.Show(dialogArg.Message, dialogArg.Caption, MessageBoxButton.OKCancel, MessageBoxImage.Question);
        return result == MessageBoxResult.OK;
    }

    public bool ShowWarningConfirmationBox(MessageBoxArg dialogArg)
    {
        MessageBoxResult result = MessageBox.Show(dialogArg.Message, dialogArg.Caption, MessageBoxButton.OKCancel, MessageBoxImage.Warning);
        return result == MessageBoxResult.OK;
    }

    public void ShowAboutWindow()
    {
        var mainWindow = App.Current.MainWindow;
        var aboutWindow = App.Current.ServiceProvider.GetRequiredService<AboutWindow>();
        aboutWindow.Owner = mainWindow;
        aboutWindow.ShowDialog();
    }

    public void ShowDefinitionManagerWindow()
    {
        throw new System.NotImplementedException();
    }

    public void ShowGotoWindow()
    {
        var mainWindow = App.Current.MainWindow;
        var gotoWindow = App.Current.ServiceProvider.GetRequiredService<GotoWindow>();
        gotoWindow.Owner = mainWindow;
        gotoWindow.Show();
    }

    public void ShowSettingWindow()
    {
        var mainWindow = App.Current.MainWindow;
        var settingWindow = App.Current.ServiceProvider.GetRequiredService<SettingWindow>();
        settingWindow.Owner = mainWindow;
        settingWindow.ShowDialog();
    }

    public FolderDialogResult ShowFolderBrowserDialog()
    {
        VistaFolderBrowserDialog dialog = new();
        bool? result = dialog.ShowDialog();
        return new FolderDialogResult()
        {
            Confirmed = result == true,
            SelectedDirectoryPath = dialog.SelectedPath,
        };
    }

    public FileDialogResult ShowOpenFileDialog(FileDialogArg dialogArg)
    {
        VistaOpenFileDialog dialog = new()
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
}

using System;
using System.Windows;

using binstarjs03.AerialOBJ.WpfApp.Views;

using Ookii.Dialogs.Wpf;

namespace binstarjs03.AerialOBJ.WpfApp.Services.ModalServices;

public delegate void ShowMessageBoxHandler(MessageBoxArg dialogArg);
public delegate FileDialogResult ShowSaveFileDialogHandler(FileDialogArg dialogArg);

public class ModalService : IModalService
{
    private readonly Func<IDialogView> _aboutViewFactory;
    private readonly Func<IDialogView> _definitionManagerViewFactory;
    private readonly Func<IDialogView> _settingViewFactory;

    public ModalService(Func<IDialogView> aboutViewFactory, Func<IDialogView> definitionManagerViewFactory, Func<IDialogView> settingViewFactory)
    {
        _aboutViewFactory = aboutViewFactory;
        _definitionManagerViewFactory = definitionManagerViewFactory;
        _settingViewFactory = settingViewFactory;
    }

    public void ShowMessageBox(MessageBoxArg dialogArg)
    {
        MessageBox.Show(dialogArg.Message, dialogArg.Caption, MessageBoxButton.OK);
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

    public void ShowAboutView() => _aboutViewFactory().ShowDialog();

    public void ShowDefinitionManagerView() => _definitionManagerViewFactory().ShowDialog();

    public void ShowSettingView() => _settingViewFactory().ShowDialog();

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
}

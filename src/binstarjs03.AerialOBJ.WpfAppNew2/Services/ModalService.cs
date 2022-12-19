using System.Windows;

using binstarjs03.AerialOBJ.WpfAppNew2.Factories;
using binstarjs03.AerialOBJ.WpfAppNew2.Views;

using Microsoft.Win32;

namespace binstarjs03.AerialOBJ.WpfAppNew2.Services;

public delegate void ShowMessageBoxHandler(MessageBoxArg dialogArg);
public delegate SaveFileDialogResult ShowSaveFileDialogHandler(SaveFileDialogArg dialogArg);

public class ModalService : IModalService
{
    private readonly IAbstractFactory<IAboutView> _aboutViewFactory;

    public ModalService(IAbstractFactory<IAboutView> aboutViewFactory)
    {
        _aboutViewFactory = aboutViewFactory;
    }

    public void ShowMessageBox(MessageBoxArg dialogArg)
    {
        MessageBox.Show(dialogArg.Message, dialogArg.Caption, MessageBoxButton.OK);
    }

    public void ShowErrorMessageBox(MessageBoxArg dialogArg)
    {
        MessageBox.Show(dialogArg.Message, dialogArg.Caption, MessageBoxButton.OK, MessageBoxImage.Error);
    }

    public void ShowAbout()
    {
        _aboutViewFactory.Create()
                         .ShowDialog();
    }

    public SaveFileDialogResult ShowSaveFileDialog(SaveFileDialogArg dialogArg)
    {
        SaveFileDialog dialog = new()
        {
            FileName = dialogArg.FileName,
            DefaultExt = dialogArg.FileExtension,
            Filter = dialogArg.FileExtensionFilter
        };
        bool? result = dialog.ShowDialog();
        return new SaveFileDialogResult()
        {
            Path = dialog.FileName,
            Result = result == true,
        };
    }
}

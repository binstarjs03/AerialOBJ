using binstarjs03.AerialOBJ.WpfAppNew2.Factories;
using binstarjs03.AerialOBJ.WpfAppNew2.Views;

namespace binstarjs03.AerialOBJ.WpfAppNew2.Services;

public delegate void ShowMessageBoxHandler(MessageBoxArg dialogArg);
public delegate SaveFileDialogResult ShowSaveFileDialogHandler(SaveFileDialogArg dialogArg);

public class ModalService : IModalService
{
    private readonly IAbstractFactory<IAboutView> _aboutViewFactory;
    private readonly ShowMessageBoxHandler _showMessageBoxHandler;
    private readonly ShowMessageBoxHandler _showErrorMessageBoxHandler;
    private readonly ShowSaveFileDialogHandler _showSaveFileDialogHandler;

    public ModalService(
        IAbstractFactory<IAboutView> aboutViewFactory,
        ShowMessageBoxHandler showMessageBoxHandler,
        ShowMessageBoxHandler showErrorMessageBoxHandler,
        ShowSaveFileDialogHandler showSaveFileDialogHandler)
    {
        _aboutViewFactory = aboutViewFactory;
        _showMessageBoxHandler = showMessageBoxHandler;
        _showErrorMessageBoxHandler = showErrorMessageBoxHandler;
        _showSaveFileDialogHandler = showSaveFileDialogHandler;
    }

    public void ShowMessageBox(MessageBoxArg dialogArg)
    {
        _showMessageBoxHandler(dialogArg);
    }

    public void ShowErrorMessageBox(MessageBoxArg dialogArg)
    {
        _showErrorMessageBoxHandler(dialogArg);
    }

    public void ShowAbout()
    {
        _aboutViewFactory.Create()
                         .ShowDialog();
    }

    public SaveFileDialogResult ShowSaveFileDialog(SaveFileDialogArg dialogArg)
    {
        return _showSaveFileDialogHandler(dialogArg);
    }
}

using System;

using binstarjs03.AerialOBJ.WpfAppNew2.Factories;
using binstarjs03.AerialOBJ.WpfAppNew2.Views;

namespace binstarjs03.AerialOBJ.WpfAppNew2.Services;
public class ModalService : IModalService
{
    private readonly IAbstractFactory<IAboutView> _aboutViewFactory;
    private readonly Action<string> _showMessageBoxHandler;

    public ModalService(IAbstractFactory<IAboutView> aboutViewFactory, Action<string> showMessageBoxHandler)
    {
        _aboutViewFactory = aboutViewFactory;
        _showMessageBoxHandler = showMessageBoxHandler;
    }

    public void ShowMessageBox(string message)
    {
        _showMessageBoxHandler(message);
    }

    public void ShowAbout()
    {
        _aboutViewFactory.Create()
                         .ShowDialog();
    }
}

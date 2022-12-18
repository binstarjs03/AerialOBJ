using System;
using System.Windows;

using binstarjs03.AerialOBJ.WpfAppNew2.Factories;
using binstarjs03.AerialOBJ.WpfAppNew2.Views;

namespace binstarjs03.AerialOBJ.WpfAppNew2.Services;
public class ModalService : IModalService
{
    private readonly IAbstractFactory<IAboutView> _aboutViewFactory;
    //private readonly Action _showMessageBox;

    public ModalService(IAbstractFactory<IAboutView> aboutViewFactory)
    {
        _aboutViewFactory = aboutViewFactory;
    }

    public void ShowMessageBox(string message)
    {
        MessageBox.Show(message); // TODO not unit testable,
                                  // make this dependent from WPF dialogs or whatnot
    }

    public void ShowAbout()
    {
        _aboutViewFactory.Create()
                         .ShowDialog();
    }
}

using System;
using System.Windows;
using binstarjs03.AerialOBJ.WpfAppNew2.Factories;
using binstarjs03.AerialOBJ.WpfAppNew2.Views;

namespace binstarjs03.AerialOBJ.WpfAppNew2.Services;
public class ModalService : IModalService
{
    private readonly Func<IAboutView> _aboutViewFactory;

    public ModalService(Func<IAboutView> aboutViewFactory)
    {
        _aboutViewFactory = aboutViewFactory;
    }

    public void ShowMessageBox(string message)
    {
        MessageBox.Show(message); // TODO make this dependent from WPF dialogs or whatnot
    }

    public void ShowAbout()
    {
        _aboutViewFactory.Invoke() // System.ObjectDisposedException on DI factory delegate
                         .ShowDialog();
    }
}

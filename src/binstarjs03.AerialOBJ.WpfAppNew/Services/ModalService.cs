using System.Windows;

using binstarjs03.AerialOBJ.WpfAppNew.View;

namespace binstarjs03.AerialOBJ.WpfAppNew.Services;

public static class ModalService
{
    public static void ShowAboutModal()
    {
        new AboutWindow().ShowDialog();
    }

    public static void ShowErrorOKModal(string caption, string errorMsg) 
    {
        MessageBox.Show(errorMsg, caption, MessageBoxButton.OK, MessageBoxImage.Error);
    }
}

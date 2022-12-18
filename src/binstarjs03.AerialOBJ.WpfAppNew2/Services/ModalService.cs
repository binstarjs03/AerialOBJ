using System.Windows;

namespace binstarjs03.AerialOBJ.WpfAppNew2.Services;
public class ModalService : IModalService
{
    public void ShowMessageBox(string message)
    {
        MessageBox.Show(message);
    }
}

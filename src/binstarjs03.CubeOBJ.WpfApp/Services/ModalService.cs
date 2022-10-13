using System.Windows.Forms;

using binstarjs03.AerialOBJ.WpfApp.UIElements.Modals;

namespace binstarjs03.AerialOBJ.WpfApp.Services;

public static class ModalService
{
    public static void ShowAboutModal()
    {
        new AboutModal().ShowDialog();
    }

    public static void ShowErrorOK(string caption, string errorMsg)
    {
        MessageBox.Show(errorMsg,
                        caption,
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
    }
}


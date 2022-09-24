using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using binstarjs03.MineSharpOBJ.WpfApp.UIElements.Modals;

namespace binstarjs03.MineSharpOBJ.WpfApp.Services;
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

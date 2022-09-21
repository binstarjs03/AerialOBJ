using System.Windows.Forms;

using binstarjs03.MineSharpOBJ.Core.Utils;
using binstarjs03.MineSharpOBJ.WpfApp.UIElements.Controls;
using binstarjs03.MineSharpOBJ.WpfApp.UIElements.Modals;

namespace binstarjs03.MineSharpOBJ.WpfApp.Services;

public static class ModalService {
    public static void ShowAboutModal() {
        new AboutModal().ShowDialog();
    }

    public static PointF? ShowGotoModal(ViewportControl viewport) {
        GotoModalDialog dialog = new(viewport);
        bool? result = dialog.ShowDialog();
        if (result == true)
            return dialog.CameraPos;
        return null;
    }

    public static void ShowErrorOK(string caption, string errorMsg) {
        MessageBox.Show(errorMsg,
                        caption,
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
    }

    public static void ShowInfoOK(string caption, string errorMsg) {
        MessageBox.Show(errorMsg,
                        caption,
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
    }
}

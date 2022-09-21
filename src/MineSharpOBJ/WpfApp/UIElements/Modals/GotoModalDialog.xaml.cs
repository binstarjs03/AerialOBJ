using System;
using System.Windows;

using binstarjs03.MineSharpOBJ.Core.Utils;
using binstarjs03.MineSharpOBJ.WpfApp.Services;
using binstarjs03.MineSharpOBJ.WpfApp.UIElements.Controls;

namespace binstarjs03.MineSharpOBJ.WpfApp.UIElements.Modals;

public partial class GotoModalDialog : Window {
    private readonly ViewportControl _viewport;
    private PointF _cameraPos;

    public GotoModalDialog(ViewportControl viewport) {
        InitializeComponent();
        _viewport = viewport;
    }

    public PointF CameraPos { 
        get {
            if (DialogResult != true)
                throw new InvalidOperationException("Dialog result was cancelled");
            return _cameraPos; 
        }
        private set {
            _cameraPos = value;
        }
    }

    private void OnOK(object sender, RoutedEventArgs e) {
        if (string.IsNullOrWhiteSpace(XField.Text) || string.IsNullOrWhiteSpace(ZField.Text)) {
            string msg = "One or more fields cannot be empty. "
                       + "Please enter valid coordinates to jump to.";
            ModalService.ShowInfoOK("Fields cannot be empty", msg);
            return;
        }
        try {
            CameraPos = new(Convert.ToDouble(XField.Text),
                            Convert.ToDouble(ZField.Text));
            DialogResult = true;
            Close();
        }
        catch (FormatException) {
            string msg = "Invalid coordinates entered. "
                       + "Please enter only number, no comma (decimal) "
                       + "or any other characters.";
            ModalService.ShowErrorOK("Invalid Coordinates", msg);
        }
    }

    private void OnCurrent(object sender, RoutedEventArgs e) {
        PointF cameraPos = _viewport.ViewportCameraPos;
        XField.Text = Convert.ToInt32(cameraPos.X).ToString();
        ZField.Text = Convert.ToInt32(cameraPos.Y).ToString();
    }

    private void OnCancel(object sender, RoutedEventArgs e) {
        Close();
    }
}

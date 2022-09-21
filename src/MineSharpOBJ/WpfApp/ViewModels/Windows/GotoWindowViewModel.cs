using System;
using System.Windows;
using System.Windows.Input;

using binstarjs03.MineSharpOBJ.Core.Utils;
using binstarjs03.MineSharpOBJ.WpfApp.ViewModels.Controls;
using binstarjs03.MineSharpOBJ.WpfApp.Views.Windows;

namespace binstarjs03.MineSharpOBJ.WpfApp.ViewModels.Windows;
public class GotoWindowViewModel : ViewModelWindow<GotoWindowViewModel, GotoWindow> {
    private string _xfield = "";
    private string _zfield = "";

    public GotoWindowViewModel(GotoWindow control) : base(control) { 
        // initialize states

        // assign command implementation to commands
        OK = new RelayCommand(OnOK);
        Cancel = new RelayCommand(OnCancel);
        Current = new RelayCommand(OnCurrent);
    }

    // States -----------------------------------------------------------------

    public string XField {
        get { return _xfield; }
        set {
            if (value == _xfield) 
                return;
            _xfield = value;
            OnPropertyChanged(nameof(XField));
        }
    }

    public string ZField {
        get { return _zfield; }
        set {
            if (value == _zfield)
                return;
            _zfield = value;
            OnPropertyChanged(nameof(ZField));
        }
    }

    // Commands ---------------------------------------------------------------

    public ICommand OK { get; }
    public ICommand Cancel { get; }
    public ICommand Current { get; }

    // Command Implementations ------------------------------------------------

    private void OnOK(object? arg) {
        if (string.IsNullOrWhiteSpace(XField) || string.IsNullOrWhiteSpace(ZField)) {
            MessageBox.Show(
              "Fields cannot be empty. Please enter valid coordinates to jump to.",
              caption: "Info",
              MessageBoxButton.OK,
              MessageBoxImage.Information
            );
            return;
        }
        try {
            PointF cameraPos = new(-Convert.ToDouble(XField),
                                   -Convert.ToDouble(ZField));
            //cameraPos /= ViewportControlViewModel.Context!.Control.ViewportPixelPerBlock;
            ViewportControlViewModel.Context!.Control.SetCameraPosition(cameraPos);
            Window.Close();
        }
        catch (FormatException) {
            string message = "Invalid coordinates entered. "
                           + "Please enter only number, no comma (decimal) "
                           + "or any other special characters.";
            MessageBox.Show(message,
                            caption: "Invalid Coordinates",
                            MessageBoxButton.OK,
                            MessageBoxImage.Error);
        }
    }

    private void OnCancel(object? arg) { 
        Window.Close();
    }

    private void OnCurrent(object? arg) {
        PointF cameraPos = ViewportControlViewModel.Context!.Control.ViewportCameraPos;
        XField = Convert.ToInt32(-cameraPos.X).ToString();
        ZField = Convert.ToInt32(-cameraPos.Y).ToString();
    }
}

namespace binstarjs03.AerialOBJ.WpfApp.Views;

public delegate void WindowPositionHandler(double top, double left);

public interface IDialogView : IClosableView
{
    bool? ShowDialog();
}

namespace binstarjs03.AerialOBJ.WpfApp.Views;
public delegate void WindowPositionHandler(double top, double left);
public interface IView
{
    bool? ShowDialog();
}

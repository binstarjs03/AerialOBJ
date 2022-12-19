namespace binstarjs03.AerialOBJ.WpfAppNew2.Views;
public delegate void WindowPositionHandler(double top, double left);
public interface IView
{
    bool? ShowDialog();
}

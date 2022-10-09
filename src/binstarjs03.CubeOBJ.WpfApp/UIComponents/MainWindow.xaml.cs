using System.Windows;

namespace binstarjs03.CubeOBJ.WpfApp.UIComponents;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        DataContext = new MainWindowVM(this);
    }

}

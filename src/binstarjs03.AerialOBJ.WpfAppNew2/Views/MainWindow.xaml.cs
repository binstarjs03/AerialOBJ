using System.Windows;

using binstarjs03.AerialOBJ.WpfAppNew2.ViewModels;

namespace binstarjs03.AerialOBJ.WpfAppNew2.Views;
public partial class MainWindow : Window
{
    public MainWindow(IMainWindowVM mainWindowVM)
    {
        InitializeComponent();
        DataContext = mainWindowVM;
        mainWindowVM.CloseRequested += OnCloseRequested;
    }

    private void OnCloseRequested()
    {
        Close();
    }
}

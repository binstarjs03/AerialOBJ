using System.Windows;

using binstarjs03.AerialOBJ.WpfAppNew2.ViewModels;

namespace binstarjs03.AerialOBJ.WpfAppNew2.Views;
public partial class MainWindow : Window
{
    public MainWindow(MainViewModel mainWindowVM)
    {
        InitializeComponent();
        DataContext = mainWindowVM;
        mainWindowVM.CloseRequested += OnCloseRequested;
        mainWindowVM.ShowMessageBoxRequested += OnShowMessageBoxRequested;
    }

    private void OnShowMessageBoxRequested(string message)
    {
        MessageBox.Show(message);
    }

    private void OnCloseRequested()
    {
        Close();
    }
}

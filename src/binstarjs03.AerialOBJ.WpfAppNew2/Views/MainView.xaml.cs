using System.Windows;

using binstarjs03.AerialOBJ.WpfAppNew2.ViewModels;

namespace binstarjs03.AerialOBJ.WpfAppNew2.Views;
public partial class MainView : Window
{
    public MainView(MainViewModel mainViewModel)
    {
        InitializeComponent();
        DataContext = mainViewModel;
        mainViewModel.CloseRequested += MainViewModel_CloseRequested;
    }

    private void MainViewModel_CloseRequested()
    {
        Close();
    }
}

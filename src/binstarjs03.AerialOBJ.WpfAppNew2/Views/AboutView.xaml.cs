using System.Windows;

using binstarjs03.AerialOBJ.WpfAppNew2.ViewModels;

namespace binstarjs03.AerialOBJ.WpfAppNew2.Views;
public partial class AboutView : Window, IAboutView
{
    public AboutView(AboutViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
        viewModel.CloseRequested += MainViewModel_CloseRequested;
    }

    private void MainViewModel_CloseRequested()
    {
        Close();
    }
}
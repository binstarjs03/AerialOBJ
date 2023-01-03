using System.Windows;

using binstarjs03.AerialOBJ.WpfApp.ViewModels;

namespace binstarjs03.AerialOBJ.WpfApp.Views;
public partial class AboutView : Window, IDialogView
{
    public AboutView(IViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
    }
}
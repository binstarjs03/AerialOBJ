using System.Windows;

using binstarjs03.AerialOBJ.WpfApp.ViewModels;

using Microsoft.Extensions.DependencyInjection;

namespace binstarjs03.AerialOBJ.WpfApp.Views;
public partial class AboutView : Window, IDialogView
{
    public AboutView()
    {
        InitializeComponent();
        DataContext = App.Current.ServiceProvider.GetRequiredService<AbstractViewModel>();
    }
}
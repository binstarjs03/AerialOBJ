using System.Windows;

using binstarjs03.AerialOBJ.MvvmAppCore.ViewModels;

using Microsoft.Extensions.DependencyInjection;

namespace binstarjs03.AerialOBJ.WpfApp.Views;
public partial class GotoWindow : Window
{
    public GotoWindow()
    {
        InitializeComponent();
        DataContext = App.Current.ServiceProvider.GetRequiredService<GotoViewModel>();
    }
}

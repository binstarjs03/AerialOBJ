using System.Windows;

using binstarjs03.AerialOBJ.MVVM.Services.ViewServices;
using binstarjs03.AerialOBJ.MVVM.ViewModels;

using Microsoft.Extensions.DependencyInjection;

namespace binstarjs03.AerialOBJ.WpfApp.Views;
public partial class MainWindow : Window, IClosable
{
    public MainWindow()
    {
        InitializeComponent();
        var viewmodel = App.Current.ServiceProvider.GetRequiredService<MainViewModel>();
        viewmodel.Closable = this;
        DataContext = viewmodel;
    }
}

using System.Windows;

using binstarjs03.AerialOBJ.MVVM.Services.ViewServices;
using binstarjs03.AerialOBJ.MVVM.ViewModels;

using Microsoft.Extensions.DependencyInjection;

namespace binstarjs03.AerialOBJ.WpfApp.Views;

public partial class AboutWindow : Window, IClosable
{
    public AboutWindow()
    {
        InitializeComponent();
        var viewmodel = App.Current.ServiceProvider.GetRequiredService<ClosableViewModel>();
        viewmodel.Closable = this;
        DataContext = viewmodel;
    }
}

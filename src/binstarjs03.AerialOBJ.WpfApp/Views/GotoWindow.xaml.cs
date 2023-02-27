using System.Diagnostics;
using System.Windows;

using binstarjs03.AerialOBJ.MvvmAppCore.ViewModels;
using binstarjs03.AerialOBJ.MvvmAppCore.ViewTraits;

using Microsoft.Extensions.DependencyInjection;

namespace binstarjs03.AerialOBJ.WpfApp.Views;
public partial class GotoWindow : Window, IClosable
{
    public GotoWindow()
    {
        InitializeComponent();
        var viewmodel = App.Current.ServiceProvider.GetRequiredService<GotoViewModel>();
        viewmodel.Closable = this;
        DataContext = viewmodel;
    }
}

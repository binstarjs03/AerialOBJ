using System.Windows;

using binstarjs03.AerialOBJ.MvvmAppCore.ViewModels;

using Microsoft.Extensions.DependencyInjection;

namespace binstarjs03.AerialOBJ.WpfApp.Views;
public partial class DefinitionManagerWindow : Window
{
    public DefinitionManagerWindow()
    {
        InitializeComponent();
        DataContext = App.Current.ServiceProvider.GetRequiredService<DefinitionManagerViewModel>();
    }
}

using System.Windows;

using binstarjs03.AerialOBJ.WpfApp.ViewModels;

using Microsoft.Extensions.DependencyInjection;

namespace binstarjs03.AerialOBJ.WpfApp.Views;
public partial class DefinitionManagerView : Window, IDialogView
{
    public DefinitionManagerView()
    {
        InitializeComponent();
        DataContext = App.Current.ServiceProvider.GetRequiredService<DefinitionManagerViewModel>();
    }
}

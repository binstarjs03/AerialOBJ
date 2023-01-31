using System.Windows;

using binstarjs03.AerialOBJ.WpfApp.ViewModels;

using Microsoft.Extensions.DependencyInjection;

namespace binstarjs03.AerialOBJ.WpfApp.Views;
public partial class NewDefinitionManagerView : Window, IDialogView
{
    public NewDefinitionManagerView()
    {
        InitializeComponent();
        DataContext = App.Current.ServiceProvider.GetRequiredService<DefinitionManagerViewModel>();
    }
}

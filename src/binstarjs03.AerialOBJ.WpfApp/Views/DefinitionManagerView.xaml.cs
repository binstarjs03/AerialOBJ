using System.Windows;

using binstarjs03.AerialOBJ.WpfApp.ViewModels;

namespace binstarjs03.AerialOBJ.WpfApp.Views;
public partial class DefinitionManagerView : Window, IDialogView
{
    public DefinitionManagerView(DefinitionManagerViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
    }
}

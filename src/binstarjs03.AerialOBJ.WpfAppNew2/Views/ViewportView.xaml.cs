using System.Windows.Controls;

using binstarjs03.AerialOBJ.WpfAppNew2.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace binstarjs03.AerialOBJ.WpfAppNew2.Views;
public partial class ViewportView : UserControl
{
    public ViewportView()
    {
        InitializeComponent();
        DataContext = App.Current?.Host.Services.GetRequiredService<ViewportViewModel>();
    }
}

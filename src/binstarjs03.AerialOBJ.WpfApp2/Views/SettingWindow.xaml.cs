using System.Windows;

using binstarjs03.AerialOBJ.MVVM.ViewModels;

using Microsoft.Extensions.DependencyInjection;

namespace binstarjs03.AerialOBJ.WpfApp.Views;
public partial class SettingWindow : Window
{
    public SettingWindow()
    {
        InitializeComponent();
        DataContext = App.Current.ServiceProvider.GetRequiredService<SettingViewModel>();
    }
}

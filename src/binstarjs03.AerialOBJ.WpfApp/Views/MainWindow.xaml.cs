using System;
using System.Windows;

using binstarjs03.AerialOBJ.MvvmAppCore.ViewModels;
using binstarjs03.AerialOBJ.MvvmAppCore.ViewTraits;

using Microsoft.Extensions.DependencyInjection;

namespace binstarjs03.AerialOBJ.WpfApp.Views;
public partial class MainWindow : Window, IClosable
{
    private ISettablePosition? _settablePosition;

    public MainWindow()
    {
        InitializeComponent();
        if (App.Current is null)
            return;
        _settablePosition = App.Current.ServiceProvider.GetService<ISettablePosition>();
        var viewmodel = App.Current.ServiceProvider.GetRequiredService<MainViewModel>();
        viewmodel.Closable = this;
        DataContext = viewmodel;
    }

    protected override void OnLocationChanged(EventArgs e) => SyncSettablePosition();

    protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo) => SyncSettablePosition();

    private void SyncSettablePosition() => _settablePosition?.SetTopLeft((int)Top, (int)(Left + ActualWidth));
}

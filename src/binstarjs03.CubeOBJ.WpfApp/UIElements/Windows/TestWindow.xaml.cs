using System.Windows;

namespace binstarjs03.AerialOBJ.WpfApp.UIElements.Windows;

public partial class TestWindow : Window
{
    public TestWindow()
    {
        InitializeComponent();
        DataContext = new TestWindowVM(this);
    }

    private void OnIncrease(object sender, RoutedEventArgs e)
    {
        ((TestWindowVM)DataContext).MyDouble++;
        ((TestWindowVM)DataContext).MyInt++;
    }

    private void OnDecrease(object sender, RoutedEventArgs e)
    {
        ((TestWindowVM)DataContext).MyDouble--;
        ((TestWindowVM)DataContext).MyInt--;
    }
}

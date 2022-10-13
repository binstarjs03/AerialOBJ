namespace binstarjs03.AerialOBJ.WpfApp.UIElements.Windows;

public class TestWindowVM : ViewModelWindow<TestWindowVM, TestWindow>
{
    public TestWindowVM(TestWindow window) : base(window) { }

    private double _myDouble = 5.5;
    public double MyDouble
    {
        get => _myDouble;
        set => SetAndNotifyPropertyChanged(value, ref _myDouble);
    }

    private int _myInt = 5;
    public int MyInt
    {
        get => _myInt;
        set => SetAndNotifyPropertyChanged(value, ref _myInt);
    }
}

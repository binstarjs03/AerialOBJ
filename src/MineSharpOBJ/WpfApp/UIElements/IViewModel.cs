using System.Windows.Controls;

namespace binstarjs03.MineSharpOBJ.WpfApp.UIElements;
public interface IViewModel<T, U> where T : ViewModelBase<T, U> where U : Control {
    public T ViewModel { get; }
}

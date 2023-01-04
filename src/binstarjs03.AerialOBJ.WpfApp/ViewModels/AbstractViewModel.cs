using binstarjs03.AerialOBJ.WpfApp.Views;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace binstarjs03.AerialOBJ.WpfApp.ViewModels;
[ObservableObject]
public partial class AbstractViewModel
{
    public AbstractViewModel(GlobalState globalState)
    {
        GlobalState = globalState;
    }

    public GlobalState GlobalState { get; }

    [RelayCommand]
    private void Close(IClosableView view) => view.Close();
}

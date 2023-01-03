using binstarjs03.AerialOBJ.WpfApp.Components;
using binstarjs03.AerialOBJ.WpfApp.Views;

using CommunityToolkit.Mvvm.Input;

namespace binstarjs03.AerialOBJ.WpfApp.ViewModels;
public partial class AboutViewModel : IViewModel
{
    public GlobalState GlobalState { get; }

    public AboutViewModel(GlobalState globalState)
	{
        GlobalState = globalState;
    }

    [RelayCommand]
    private void Close(IClosableView view)
    {
        view.Close();
    }
}

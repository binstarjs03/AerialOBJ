using CommunityToolkit.Mvvm.Input;

namespace binstarjs03.AerialOBJ.WpfAppNew2.ViewModels;
public interface IMainWindowVM : IViewModelBase
{
    GlobalState GlobalState { get; }
    IRelayCommand CloseCommand { get; }
}

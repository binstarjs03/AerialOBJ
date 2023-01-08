using binstarjs03.AerialOBJ.WpfApp.Views;

using CommunityToolkit.Mvvm.Input;

namespace binstarjs03.AerialOBJ.WpfApp.ViewModels;

public interface IMainViewModel
{
    string Title { get; }
    GlobalState GlobalState { get; }
    ViewState ViewState { get; }
    IView ViewportView { get; }

    IRelayCommand<string?> OpenSavegameCommand { get; }
    IRelayCommand CloseSavegameCommand { get; }
    IRelayCommand<IClosableView> CloseCommand { get; }
    IRelayCommand ClosingCommand { get; }
    IRelayCommand ShowAboutModalCommand { get; }
    IRelayCommand ShowDefinitionManagerModalCommand { get; }
}

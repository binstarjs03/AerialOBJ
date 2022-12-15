using System;

using CommunityToolkit.Mvvm.Input;

namespace binstarjs03.AerialOBJ.WpfAppNew2.ViewModels;
public partial class MainWindowVM : IMainWindowVM
{
    public MainWindowVM(GlobalState globalState)
    {
        GlobalState = globalState;
    }

    public GlobalState GlobalState { get; }

    public event Action? CloseRequested;

    [RelayCommand]
    private void OnClose()
    {
        CloseRequested?.Invoke();
    }
}

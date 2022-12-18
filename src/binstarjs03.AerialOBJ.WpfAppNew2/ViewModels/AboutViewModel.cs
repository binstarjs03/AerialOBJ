using System;

using binstarjs03.AerialOBJ.WpfAppNew2.Components;

using CommunityToolkit.Mvvm.Input;

namespace binstarjs03.AerialOBJ.WpfAppNew2.ViewModels;
public partial class AboutViewModel
{
    public GlobalState GlobalState { get; }

    public AboutViewModel(GlobalState globalState)
	{
        GlobalState = globalState;
    }

    public event Action? CloseRequested;

    [RelayCommand]
    private void OnClose()
    {
        CloseRequested?.Invoke();
    }
}

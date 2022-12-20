using binstarjs03.AerialOBJ.WpfAppNew2.Components;

using CommunityToolkit.Mvvm.ComponentModel;

namespace binstarjs03.AerialOBJ.WpfAppNew2.ViewModels;
[ObservableObject]
public partial class ViewportViewModel
{

    public ViewportViewModel(GlobalState globalState)
    {
        GlobalState = globalState;
        GlobalState.PropertyChanged += OnPropertyChanged;
    }

    public GlobalState GlobalState { get; }
}

using binstarjs03.AerialOBJ.WpfAppNew.Components;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace binstarjs03.AerialOBJ.WpfAppNew.ViewModel;

public partial class BaseViewModel : ObservableObject
{
    [RelayCommand]
    private static void OnClose(IClosable closable) => closable.Close();
}

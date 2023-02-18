using binstarjs03.AerialOBJ.MVVM.Services.ViewServices;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace binstarjs03.AerialOBJ.MVVM.ViewModels;
public partial class ClosableViewModel : ObservableObject
{
    public IClosable? Closable { get; set; }

    [RelayCommand]
    protected void Close() => Closable?.Close();
}

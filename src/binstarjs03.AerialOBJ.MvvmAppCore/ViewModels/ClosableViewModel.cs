using binstarjs03.AerialOBJ.MvvmAppCore.ViewTraits;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace binstarjs03.AerialOBJ.MvvmAppCore.ViewModels;
public partial class ClosableViewModel : ObservableObject
{
    public IClosable? Closable { get; set; }

    [RelayCommand]
    protected void Close() => Closable?.Close();
}

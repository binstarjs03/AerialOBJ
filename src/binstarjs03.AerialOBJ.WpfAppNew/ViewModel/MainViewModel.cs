using binstarjs03.AerialOBJ.WpfAppNew.Services;

using CommunityToolkit.Mvvm.Input;

namespace binstarjs03.AerialOBJ.WpfAppNew.ViewModel;

public partial class MainViewModel : BaseViewModel
{
    [RelayCommand]
    private static void OnShowAboutModal() => ModalService.ShowAboutModal();
}

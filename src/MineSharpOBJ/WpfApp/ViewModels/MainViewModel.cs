using System.IO;
using System.Windows.Forms;
using System.Windows.Input;
using binstarjs03.MineSharpOBJ.WpfApp.Services;
using binstarjs03.MineSharpOBJ.WpfApp.Views;
namespace binstarjs03.MineSharpOBJ.WpfApp.ViewModels;

public class MainViewModel : ViewModelBase<MainViewModel, MainView> {
    private bool _isDebugLogWindowVisible;
    private string _title;

    public MainViewModel(MainView view) : base(view) {
        // initialize states
        _isDebugLogWindowVisible = false;
        _title = "MineSharpOBJ";

        // assign command implementation to commands
        LoadSavegameFolder = new RelayCommand(OnLoadSavegameFolder);
        OpenAboutView = new RelayCommand(OnOpenAboutView);
    }

    // States -----------------------------------------------------------------

    public bool IsDebugLogViewVisible {
        get { return _isDebugLogWindowVisible; }
        set {
            if (value == _isDebugLogWindowVisible)
                return;
            _isDebugLogWindowVisible = value;
            DebugLogViewModel.Context.IsVisible = value;
            OnPropertyChanged(nameof(IsDebugLogViewVisible));
        }
    }

    public string Title {
        get { return _title; }
        set { 
            _title = value; 
            OnPropertyChanged(nameof(Title)); 
        }
    }

    // TODO redundant, pending removing
    public bool IsClosing { get; set; }

    // Commands ---------------------------------------------------------------

    public ICommand LoadSavegameFolder { get; }
    
    public ICommand OpenAboutView { get; }

    // Command Implementations ------------------------------------------------

    private void OnLoadSavegameFolder(object? arg) {
        // TODO use IOService, write dialog methods, use it throughout the code
        // and remove repetitiveness everytime needs to open dialog

        FolderBrowserDialog dialog = new();
        DialogResult dialogResult = dialog.ShowDialog();
        if (dialogResult != DialogResult.OK)
            return;
        IOService.LoadSavegame(dialog.SelectedPath);

        //bool loadResult = IOService.LoadSavegame(savegamePath);
        //if (!loadResult) {
        //    //LogService.LogNotification($"Failed loading savegame folder");
        //    return;
        //}
        //LogService.LogNotification($"Loaded savegame folder");
    }

    private void OnOpenAboutView(object? arg) {
        new AboutView().ShowDialog();
    }
}
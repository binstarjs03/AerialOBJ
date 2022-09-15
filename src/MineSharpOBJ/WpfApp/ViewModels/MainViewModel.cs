using System;
using System.Windows.Forms;
using System.Windows.Input;
using binstarjs03.MineSharpOBJ.WpfApp.Services;
using binstarjs03.MineSharpOBJ.WpfApp.Views;
namespace binstarjs03.MineSharpOBJ.WpfApp.ViewModels;

public class MainViewModel : ViewModelBase<MainViewModel, MainView> {
    private bool _isDebugLogWindowVisible;
    private string _title;
    private SessionInfo? _session;

    public MainViewModel(MainView view) : base(view) {
        // initialize states
        _isDebugLogWindowVisible = false;
        _title = "MineSharpOBJ";

        // assign command implementation to commands
        LoadSavegameFolder = new RelayCommand(OnLoadSavegameFolder);
        CloseSession = new RelayCommand(OnCloseSession, CanCloseSession);
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

    public ICommand CloseSession { get; }
    
    public ICommand OpenAboutView { get; }

    // Command Implementations ------------------------------------------------

    private void OnLoadSavegameFolder(object? arg) {
        using FolderBrowserDialog dialog = new();
        DialogResult dialogResult = dialog.ShowDialog();
        if (dialogResult != DialogResult.OK)
            return;
        SessionInfo? session = IOService.LoadSavegame(dialog.SelectedPath);
        if (session is null) {
            LogService.Log("Failed changing session. Aborting...", useSeparator: true);
            return;
        }
        ChangeTitle(session.Value.WorldName);
        _session = session;
        LogService.Log("Successfully changed session.", useSeparator: true);
    }

    private void OnCloseSession(object? arg) {
        ChangeTitle();
        _session = null;
        LogService.Log("Successfully closed session.", useSeparator: true);
    }

    private void OnOpenAboutView(object? arg) {
        new AboutView().ShowDialog();
    }

    // Command Availability ---------------------------------------------------

    private bool CanCloseSession(object? arg) {
        return _session is not null;
    }

    // Private Methods --------------------------------------------------------

    private void ChangeTitle(string? title = null) {
        if (title is null)
            Title = $"MineSharpOBJ";
        else
            Title = $"MineSharpOBJ - {title}";
    }
}
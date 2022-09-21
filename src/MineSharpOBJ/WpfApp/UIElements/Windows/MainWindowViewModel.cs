using System.ComponentModel;
using System.Windows.Forms;
using System.Windows.Input;

using binstarjs03.MineSharpOBJ.Core.Utils;
using binstarjs03.MineSharpOBJ.WpfApp.Services;

namespace binstarjs03.MineSharpOBJ.WpfApp.UIElements.Windows;

public class MainWindowViewModel : ViewModelWindow<MainWindowViewModel, MainWindow> {
    private SessionInfo? _session = null;
    private string _title = "MineSharpOBJ";
    private bool _isDebugLogWindowVisible = false;
    private bool _isViewportDebugControlVisible = false;
    private bool _isViewportCameraPositionGuideVisible = false;

    public MainWindowViewModel(MainWindow view) : base(view) {
        // assign command implementation to commands
        LoadSavegameFolder = new RelayCommand(OnLoadSavegameFolder);
        CloseSession = new RelayCommand(OnCloseSession, HasSessionCommandAvailability);
        OpenAboutView = new RelayCommand(OnOpenAboutView);
        ViewportGoto = new RelayCommand(OnViewportGoto, HasSessionCommandAvailability);
    }

    public override void StartEventListening() {
        DebugLogWindowViewModel.Context!.PropertyChanged += OnOtherViewModelPropertyChanged;
    }

    // States -----------------------------------------------------------------

    public SessionInfo? Session {
        get { return _session; }
        private set {
            _session = value;
            OnPropertyChanged(nameof(HasSession));
        }
    }

    public bool HasSession => Session is not null;

    public string Title {
        get { return _title; }
        private set { 
            _title = value; 
            OnPropertyChanged(nameof(Title)); }
    }

    public bool IsDebugLogViewVisible {
        get { return _isDebugLogWindowVisible; }
        set {
            if (value == _isDebugLogWindowVisible)
                return;
            _isDebugLogWindowVisible = value;
            OnPropertyChanged(nameof(IsDebugLogViewVisible));
        }
    }

    public bool IsViewportDebugControlVisible {
        get { return _isViewportDebugControlVisible; }
        set {
            if (value == _isViewportDebugControlVisible)
                return;
            _isViewportDebugControlVisible = value;
            OnPropertyChanged(nameof(IsViewportDebugControlVisible));
        }
    }

    public bool IsViewportCameraPositionGuideVisible {
        get { return _isViewportCameraPositionGuideVisible; }
        set {
            if (value == _isViewportCameraPositionGuideVisible)
                return;
            _isViewportCameraPositionGuideVisible = value;
            OnPropertyChanged(nameof(IsViewportCameraPositionGuideVisible));
        }
    }

    // Commands ---------------------------------------------------------------

    public ICommand LoadSavegameFolder { get; }
    public ICommand CloseSession { get; }
    public ICommand OpenAboutView { get; }
    public ICommand ViewportGoto { get; }

    // Command Implementations ------------------------------------------------

    private void OnLoadSavegameFolder(object? arg) {
        LogService.Log("Attempting to loading savegame...");
        using FolderBrowserDialog dialog = new();
        DialogResult dialogResult = dialog.ShowDialog();
        if (dialogResult != DialogResult.OK)
            return;
        SessionInfo? session = IOService.LoadSavegame(dialog.SelectedPath);
        if (session is null) {
            LogService.Log("Failed changing session. Aborting...", useSeparator: true);
            return;
        }

        Title = $"MineSharpOBJ - {session.WorldName}";
        Session = session;
        LogService.Log("Successfully changed session.", useSeparator: true);
    }

    private void OnCloseSession(object? arg) {
        LogService.Log("Attempting to closing savegame...");
        Title = "MineSharpOBJ";
        Session = null;
        IsViewportDebugControlVisible = false;
        IsViewportCameraPositionGuideVisible = false;
        LogService.Log("Successfully closed session.", useSeparator: true);
    }

    private void OnOpenAboutView(object? arg) {
        ModalService.ShowAboutModal();
    }

    private void OnViewportGoto(object? arg) {
        PointF? cameraPos = ModalService.ShowGotoModal(Window.ViewportControl);
        if (cameraPos is null)
            return;
        Window.ViewportControl.SetCameraPosition((PointF)cameraPos);
    }

    // Command Availability ---------------------------------------------------

    private bool HasSessionCommandAvailability(object? arg) {
        return HasSession;
    }

    // Event Handlers ---------------------------------------------------------

    protected override void OnOtherViewModelPropertyChanged(object? sender, PropertyChangedEventArgs e) {
        
    }

    // Private Methods --------------------------------------------------------

}

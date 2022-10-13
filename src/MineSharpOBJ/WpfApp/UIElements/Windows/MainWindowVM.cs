using System.ComponentModel;
using System.Windows.Forms;
using System.Windows.Input;

using binstarjs03.MineSharpOBJ.WpfApp.Services;

namespace binstarjs03.MineSharpOBJ.WpfApp.UIElements.Windows;

public class MainWindowVM : ViewModelWindow<MainWindowVM, MainWindow>
{
    public MainWindowVM(MainWindow window) : base(window)
    {
        // listen to shared property change
        SharedProperty.PropertyChanged += OnSharedPropertyChanged;

        // assign command implementation to commands
        OpenCommand = new RelayCommand(OnOpen);
        CloseCommand = new RelayCommand(OnClose, (arg) => HasSession );
        AboutCommand = new RelayCommand(OnAbout);
    }

    // States -----------------------------------------------------------------

    private string _title = SharedProperty.SessionInfo is null ? "MineSharpOBJ" : $"MineSharpOBJ - {SharedProperty.SessionInfo.WorldName}";
    public string Title
    {
        get => _title;
        set => SetAndNotifyPropertyChanged(value, ref _title);
    }

    public bool IsDebugLogWindowVisible
    {
        get => SharedProperty.IsDebugLogWindowVisible;
        set => SetSharedPropertyChanged
        (
            value,
            SharedProperty.IsDebugLogWindowVisibleUpdater
        );
    }

    public bool IsViewportDebugInfoVisible
    {
        get => SharedProperty.IsViewportDebugInfoVisible;
        set => SetSharedPropertyChanged
        (
            value,
            SharedProperty.IsViewportDebugInfoVisibleUpdater
        );
    }

    public bool IsViewportCameraPositionGuideVisible
    {
        get => SharedProperty.IsViewportCameraPositionGuideVisible;
        set => SetSharedPropertyChanged
        (
            value, 
            SharedProperty.IsViewportCameraPositionGuideVisibleUpdater
        );
    }

    // we can make this property as static, but XAML databinding
    // intellisense won't detect this property anymore
    public bool HasSession => SharedProperty.HasSession;

    // Commands ---------------------------------------------------------------

    public ICommand AboutCommand { get; }
    private void OnAbout(object? arg)
    {
        ModalService.ShowAboutModal();
    }

    public ICommand OpenCommand { get; }
    private void OnOpen(object? arg)
    {
        LogService.Log("Attempting to loading savegame...");
        using FolderBrowserDialog dialog = new();
        DialogResult dialogResult = dialog.ShowDialog();
        if (dialogResult != DialogResult.OK)
        {
            LogService.LogAborted("Dialog cancelled. Aborting...", useSeparator: true);
            return;
        }
        SessionInfo? session = IOService.LoadSavegame(dialog.SelectedPath);
        if (session is null)
        {
            LogService.LogAborted("Failed changing session. Aborting...", useSeparator: true);
            return;
        }
        SharedProperty.SessionInfoUpdater(session);
        LogService.LogSuccess("Successfully changed session.", useSeparator: true);
    }

    public ICommand CloseCommand { get; }
    private void OnClose(object? arg)
    {
        string logSuccessMsg;
        LogService.Log("Attempting to closing savegame...");

        if (SharedProperty.SessionInfo is null)
        {
            LogService.LogWarning($"{nameof(SharedProperty.SessionInfo)} is already null!");
            logSuccessMsg = "Successfully closed session.";
        }
        else
        {
            string worldName = SharedProperty.SessionInfo.WorldName;
            logSuccessMsg = $"Successfully closed session \"{worldName}\".";
        }

        SharedProperty.SessionInfoUpdater(null);
        LogService.LogSuccess(logSuccessMsg, useSeparator: true);
    }

    // Event Handlers ---------------------------------------------------------

    // TODO reduce coupling by making shared properties below to local when make sense,
    // use events and listen to it to update and synchronize the properties between VM
    protected override void OnSharedPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        base.OnSharedPropertyChanged(sender, e);
        if (e.PropertyName! == nameof(SharedProperty.SessionInfo))
        {
            NotifyPropertyChanged(nameof(HasSession));
            if (SharedProperty.SessionInfo is null)
                Title = "MineSharpOBJ";
            else
                Title = $"MineSharpOBJ - {SharedProperty.SessionInfo.WorldName}";
        }

    }
}

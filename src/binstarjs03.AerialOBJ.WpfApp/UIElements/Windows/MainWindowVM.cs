using System;
using System.ComponentModel;
using System.Windows.Forms;
using System.Windows.Input;

using binstarjs03.AerialOBJ.WpfApp.Services;

namespace binstarjs03.AerialOBJ.WpfApp.UIElements.Windows;

public class MainWindowVM : ViewModelWindow<MainWindowVM, MainWindow>
{
    public MainWindowVM(MainWindow window) : base(window)
    {
        SharedProperty.PropertyChanged += OnSharedPropertyChanged;

        // set commands to its corresponding implementations
        AboutCommand = new RelayCommand(OnAbout);
        OpenCommand = new RelayCommand(OnOpen);
        CloseCommand = new RelayCommand(OnClose, (arg) => HasSession);
        ForceGCCommand = new RelayCommand(OnForceGCCommand);
    }

    #region States - Fields and Properties

    private string _title = SharedProperty.SessionInfo is null ? App.AppName : $"{App.AppName} - {SharedProperty.SessionInfo.WorldName}";
    private static bool HasSession => SharedProperty.HasSession;

    #endregion

    #region Data Binders

    public string TitleBinding => _title;

    public bool HasSessionBinding => HasSession;

    public bool UISidePanelVisibleBinding
    {
        get => SharedProperty.UISidePanelVisible;
        set => SharedProperty.UpdateUISidePanelVisible(value);
    }

    public bool UIDebugLogWindowVisibleBinding
    {
        get => SharedProperty.UIDebugLogWindowVisible;
        set => SharedProperty.UpdateUIDebugLogWindowVisible(value);
    }

    #endregion

    #region Commands

    public ICommand AboutCommand { get; }
    public ICommand OpenCommand { get; }
    public ICommand CloseCommand { get; }
    public ICommand ForceGCCommand { get; }

    private void OnAbout(object? arg)
    {
        ModalService.ShowAboutModal();
    }

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
        SharedProperty.UpdateSessionInfo(session);
        LogService.LogSuccess("Successfully changed session.", useSeparator: true);
    }

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

        SharedProperty.UpdateSessionInfo(null);
        LogService.LogSuccess(logSuccessMsg, useSeparator: true);
    }

    private void OnForceGCCommand(object? arg)
    {
        GC.Collect();
        LogService.LogWarning("Garbage collection was forced done by the user!", useSeparator: true);
    }

    #endregion

    #region Updaters

    private void UpdateTitle(string title)
    {
        _title = title;
        NotifyPropertyChanged(nameof(TitleBinding));
    }

    #endregion

    #region Event Handlers

    // TODO we need to find a new way to automatically notify shared property change as
    // data binders are all have "Binding" suffix
    // solution: maybe on base implementation, add another
    // NotifyPropertyChanged call with "Binding" suffix added
    protected override void OnSharedPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        base.OnSharedPropertyChanged(sender, e);
        string propName = e.PropertyName!;

        if (propName == nameof(SharedProperty.SessionInfo))
        {
            if (SharedProperty.SessionInfo is null)
                UpdateTitle(App.AppName);
            else
                UpdateTitle($"{App.AppName} - {SharedProperty.SessionInfo.WorldName}");
        }
    }

    #endregion
}

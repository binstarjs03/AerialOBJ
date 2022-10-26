using System;
using System.ComponentModel;
using System.Windows.Forms;
using System.Windows.Input;

using binstarjs03.AerialOBJ.WpfApp.Services;

using Application = System.Windows.Application;

namespace binstarjs03.AerialOBJ.WpfApp.UIElements.Windows;

public class MainWindowVM : ViewModelWindow<MainWindowVM, MainWindow>
{
    public MainWindowVM(MainWindow window) : base(window)
    {
        App.CurrentCast.Properties.PropertyChanged += OnSharedPropertyChanged;

        // set commands to its corresponding implementations
        AboutCommand = new RelayCommand(OnAbout);
        OpenCommand = new RelayCommand(OnOpen);
        CloseCommand = new RelayCommand(OnClose, (arg) => HasSession);
        SettingCommand = new RelayCommand(OnSetting);
        ForceGCCommand = new RelayCommand(OnForceGCCommand);
    }

    #region States - Fields and Properties

    private string _title = App.CurrentCast.Properties.SessionInfo is null ?
        App.AppProperty.AppName : $"{App.AppProperty.AppName} - {App.CurrentCast.Properties.SessionInfo.WorldName}";
    private static bool HasSession => App.CurrentCast.Properties.HasSession;

    #endregion

    #region Data Binders

    public string TitleBinding => _title;

    public bool HasSessionBinding => HasSession;

    public bool UIDebugLogWindowVisibleBinding
    {
        get => App.CurrentCast.Properties.UIDebugLogWindowVisible;
        set => App.CurrentCast.Properties.UpdateUIDebugLogWindowVisible(value);
    }

    #endregion

    #region Commands

    public ICommand AboutCommand { get; }
    public ICommand OpenCommand { get; }
    public ICommand CloseCommand { get; }
    public ICommand SettingCommand { get; }
    public ICommand ForceGCCommand { get; }

    protected override void OnWindowClose(object? arg)
    {
        Application.Current.Shutdown();
    }

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

        // close if session exist, this is to ensure
        // every components are reinitialized, such as ChunkManager, etc
        if (App.CurrentCast.Properties.HasSession)
        {
            LogService.Log("Session exist, closing...");
            OnClose(null);
        }

        App.CurrentCast.Properties.UpdateSessionInfo(session);
        LogService.LogSuccess("Successfully changed session.", useSeparator: true);
    }

    private void OnClose(object? arg)
    {
        string logSuccessMsg;
        LogService.Log("Attempting to closing savegame...");

        if (App.CurrentCast.Properties.SessionInfo is null)
        {
            LogService.LogWarning($"{nameof(App.CurrentCast.Properties.SessionInfo)} is already null!");
            logSuccessMsg = "Successfully closed session.";
        }
        else
        {
            string worldName = App.CurrentCast.Properties.SessionInfo.WorldName;
            logSuccessMsg = $"Successfully closed session \"{worldName}\".";
        }

        App.CurrentCast.Properties.UpdateSessionInfo(null);
        LogService.LogSuccess(logSuccessMsg, useSeparator: true);
    }

    private void OnSetting(object? arg)
    {
        ModalService.ShowSettingModal();
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

        if (propName == nameof(App.CurrentCast.Properties.SessionInfo))
        {
            if (App.CurrentCast.Properties.SessionInfo is null)
                UpdateTitle(App.AppProperty.AppName);
            else
                UpdateTitle($"{App.AppProperty.AppName} - {App.CurrentCast.Properties.SessionInfo.WorldName}");
        }
    }

    #endregion
}

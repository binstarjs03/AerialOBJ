using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Threading;

using binstarjs03.AerialOBJ.WpfApp.Services;
using binstarjs03.AerialOBJ.WpfApp.UIElements.Windows;

namespace binstarjs03.AerialOBJ.WpfApp;

public partial class App : Application
{
    public static App CurrentCast => (App)Current;
    public new AppProperty Properties { get; }

    /// <summary>
    /// Invoked after all user interface are loaded
    /// </summary>
    public event StartupEventHandler? Initializing;

    public App()
    {
        Properties = new AppProperty(DateTime.Now);
    }

    protected override void OnStartup(StartupEventArgs e)
    {
        Current.DispatcherUnhandledException += OnUnhandledException;
        ShutdownMode = ShutdownMode.OnMainWindowClose;

        DebugLogWindow debugLogWindow = new();
        MainWindow = new MainWindow(debugLogWindow);
        MainWindow.Show();
        debugLogWindow.Owner = MainWindow;
        //Properties.UpdateUIDebugLogWindowVisible(true);
        LogService.LogRuntimeInfo();
        LogService.Log("Starting Initialization...");
        Initializing?.Invoke(this, e);
        LogService.Log("Initialization complete", useSeparator: true);
    }

    private void OnUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
    {
        string modalTitle = $"{AppProperty.AppName} Crashed";
        string msg = $"An unhandled exception occured: \n"
                   + $"{e.Exception}\n"
                   + $"{e.Exception.Message}\n\n";
        LogService.Log(msg);
        MessageBox.Show(msg, modalTitle, MessageBoxButton.OK, MessageBoxImage.Error);

        string lauchTime = Properties.LaunchTime.ToString().Replace('/', '-').Replace(':', '-');
        string path = $"{Environment.CurrentDirectory}/{AppProperty.AppName} Crash Log {lauchTime}.txt";

        string logSaveMsg;
        MessageBoxImage messageBoxIcon;
        if (LogService.WriteLogToDiskOnCrashed(path))
        {
            logSaveMsg = $"Debug log content has been saved to {path}";
            messageBoxIcon = MessageBoxImage.Information;
        }
        else
        {
            logSaveMsg = $"Failed writing Debug log content to {path}";
            messageBoxIcon = MessageBoxImage.Warning;
        }
        MessageBox.Show(logSaveMsg, modalTitle, MessageBoxButton.OK, messageBoxIcon);
    }

    public class AppProperty
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        private readonly static Type s_this = typeof(AppProperty);

        public AppProperty(DateTime launchTime)
        {
            LaunchTime = launchTime;
        }

        #region Internal Logic

        private void NotifyPropertyChanged<T>(T newValue, ref T oldValue, bool canNull = false, [CallerMemberName] string propertyName = "")
        {
            if (canNull)
            {
                if (newValue is null && oldValue is null)
                    return;
            }
            else
            {
                if (newValue is null || oldValue is null)
                    throw new ArgumentNullException
                    (
                        "newValue or oldValue",
                        "Argument oldValue passed to ValueChanged of ViewModelBase is null"
                    );
                if (newValue.Equals(oldValue))
                    return;
            }
            oldValue = newValue;
            PropertyChanged?.Invoke(s_this, new PropertyChangedEventArgs(propertyName));
        }

        private void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(s_this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

        #region Property Backing Field

        private bool _uiDebugLogWindowVisible = false;
        private SessionInfo? _sessionInfo = null;

        #endregion

        #region Property

        public DateTime LaunchTime { get; }
        public static string AppName => "AerialOBJ";

        public bool UIDebugLogWindowVisible
        {
            get => _uiDebugLogWindowVisible;
            set => NotifyPropertyChanged(value, ref _uiDebugLogWindowVisible);
        }

        public SessionInfo? SessionInfo
        {
            get => _sessionInfo;
            set
            {
                NotifyPropertyChanged(value, ref _sessionInfo, canNull: true);
                NotifyPropertyChanged(nameof(HasSession));
            }
        }

        public bool HasSession => SessionInfo is not null;

        #endregion

        #region Property Updater

        public void UpdateUIDebugLogWindowVisible(bool value)
        {
            UIDebugLogWindowVisible = value;
        }
        public void UpdateSessionInfo(SessionInfo? value)
        {
            SessionInfo = value;
        }

        #endregion

    }

}

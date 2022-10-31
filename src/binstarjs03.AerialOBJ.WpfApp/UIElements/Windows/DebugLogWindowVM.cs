using System.IO;
using System.Windows.Forms;
using System.Windows.Input;

using binstarjs03.AerialOBJ.WpfApp.Services;

namespace binstarjs03.AerialOBJ.WpfApp.UIElements.Windows;

public class DebugLogWindowVM : ViewModelWindow<DebugLogWindowVM, DebugLogWindow>
{
    public string Title => $"{App.AppProperty.AppName} - Debug Log";

    public bool UIDebugLogWindowVisible
    {
        get => App.CurrentCast.Properties.UIDebugLogWindowVisible;
        set => App.CurrentCast.Properties.UpdateUIDebugLogWindowVisible(value);
    }

    public string LogContent => LogService.GetLogContent();

    public DebugLogWindowVM(DebugLogWindow window) : base(window)
    {
        App.CurrentCast.Properties.PropertyChanged += OnSharedPropertyChanged;

        // listen to service events
        LogService.Logging += OnLogServiceLogging;

        // set commands to its corresponding implementations
        SaveLogCommand = new RelayCommand(OnSaveLog);
        ClearLogCommand = new RelayCommand(OnClearLog);
        WriteSingleCommand = new RelayCommand(OnWriteSingle);
        WriteHorizontalCommand = new RelayCommand(OnWriteHorizontal);
        WriteVerticalCommand = new RelayCommand(OnWriteVertical);
    }

    #region Commands

    public ICommand SaveLogCommand { get; }
    public ICommand ClearLogCommand { get; }
    public ICommand WriteSingleCommand { get; }
    public ICommand WriteHorizontalCommand { get; }
    public ICommand WriteVerticalCommand { get; }

    private void OnSaveLog(object? arg)
    {
        using SaveFileDialog dialog = new()
        {
            FileName = $"MineSharpOBJ Log",
            DefaultExt = ".txt",
            Filter = "Text Document (.txt)|*.txt"
        };
        DialogResult result = dialog.ShowDialog();
        if (result != DialogResult.OK)
            return;
        string path = dialog.FileName;
        try
        {
            IOService.WriteText(path, Window.LogTextBox.Text);
        }
        catch (IOException ex)
        {
            ModalService.ShowErrorOK($"Unhandled Exception ({ex})", ex.Message);
        }
    }

    private void OnClearLog(object? arg)
    {
        LogService.ClearLogContent();
        LogService.LogRuntimeInfo();
    }

    private void OnWriteSingle(object? arg)
    {
        LogService.Log("Hello World!");
    }

    private void OnWriteHorizontal(object? arg)
    {
        LogService.Log("Lorem ipsum dolor sit amet, consectetur adipiscing elit. Donec eget hendrerit nisl. In ut gravida metus. Suspendisse vitae gravida lacus. Nulla faucibus congue velit, at iaculis dolor interdum a. Nunc id metus sed nunc molestie varius. Cras lobortis auctor urna, ut pulvinar ante. Pellentesque vehicula lobortis nunc et iaculis. Vivamus at lacus tortor. Vivamus euismod eget quam sed rhoncus. Curabitur ipsum velit, venenatis et accumsan vitae, dictum nec augue.");
    }

    private void OnWriteVertical(object? arg)
    {
        string[] contents = new string[] {
                "Lorem ipsum dolor sit amet",
                "Suspendisse vitae gravida lacus",
                "Nunc id metus sed nunc molestie varius",
                "Vivamus euismod eget quam sed rhoncus",
                "venenatis et accumsan vitae",
                "at iaculis dolor interdum",
                "consectetur adipiscing elit",
                "Donec eget hendrerit nisl",
                "Nulla faucibus congue velit",
            };
        foreach (string content in contents)
        {
            LogService.Log(content);
        }
    }

    #endregion

    #region Event Handlers

    private void OnLogServiceLogging(string content)
    {
        NotifyPropertyChanged(nameof(LogContent));
        Window.LogTextBox.ScrollToEnd();
    }

    #endregion
}

using System;
using System.IO;
using System.Windows;
using System.Windows.Input;

using SaveFileDialog = System.Windows.Forms.SaveFileDialog;
using DialogResult = System.Windows.Forms.DialogResult;

using binstarjs03.MineSharpOBJ.WpfApp.Services;
using binstarjs03.MineSharpOBJ.WpfApp.Views.Windows;
namespace binstarjs03.MineSharpOBJ.WpfApp.ViewModels.Windows;

public class DebugLogWindowViewModel : ViewModelWindow<DebugLogWindowViewModel, DebugLogWindow> {
    private string _logContent = string.Empty;

    public DebugLogWindowViewModel(DebugLogWindow view) : base(view) {
        // listen to service events
        LogService.LogHandlers += LogHandler;

        // initialize states
        _isVisible = MainWindowViewModel.Context!.IsDebugLogViewVisible;

        // assign command implementation to commands
        SaveLog = new RelayCommand(OnSaveLog);
        ClearLog = new RelayCommand(OnClearLog);
        WriteSingle = new RelayCommand(OnWriteSingle);
        WriteHorizontal = new RelayCommand(OnWriteHorizontal);
        WriteVertical = new RelayCommand(OnWriteVertical);
    }

    // States -----------------------------------------------------------------

    public new bool IsVisible {
        get { return _isVisible; }
        set {
            if (value == _isVisible)
                return;
            _isVisible = value;
            if (MainWindowViewModel.Context is not null)
                MainWindowViewModel.Context.IsDebugLogViewVisible = value;
            OnPropertyChanged(nameof(IsVisible));
        }
    }

    public string LogContent {
        get { return _logContent; }
        set {
            _logContent = value;
            OnPropertyChanged(nameof(LogContent));
        }
    }

    // Commands ---------------------------------------------------------------

    public ICommand SaveLog { get; }

    public ICommand ClearLog { get; }

    public ICommand WriteSingle { get; }

    public ICommand WriteHorizontal { get; }

    public ICommand WriteVertical { get; }

    // Command Implementations ------------------------------------------------

    private void OnSaveLog(object? arg) {
        using SaveFileDialog dialog = new() {
            FileName = $"MineSharpOBJ Log",
            DefaultExt = ".txt",
            Filter = "Text Document (.txt)|*.txt"
        };
        DialogResult result = dialog.ShowDialog();
        if (result != DialogResult.OK)
            return;
        string path = dialog.FileName;
        try {
            IOService.WriteText(path, _window.LogTextBox.Text);
        }
        catch (IOException ex) {
            MessageBox.Show(ex.Message);
        }
    }

    private void OnClearLog(object? arg) {
        LogContent = "";
        LogService.LogRuntimeInfo();
    }

    private void OnWriteSingle(object? arg) {
        LogService.Log("Hello World!");
    }

    private void OnWriteHorizontal(object? arg) {
        LogService.Log("Lorem ipsum dolor sit amet, consectetur adipiscing elit. Donec eget hendrerit nisl. In ut gravida metus. Suspendisse vitae gravida lacus. Nulla faucibus congue velit, at iaculis dolor interdum a. Nunc id metus sed nunc molestie varius. Cras lobortis auctor urna, ut pulvinar ante. Pellentesque vehicula lobortis nunc et iaculis. Vivamus at lacus tortor. Vivamus euismod eget quam sed rhoncus. Curabitur ipsum velit, venenatis et accumsan vitae, dictum nec augue.");
    }

    private void OnWriteVertical(object? arg) {
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
        foreach (string content in contents) {
            LogService.Log(content);
        }
    }

    // Event Handlers ---------------------------------------------------------

    private void LogHandler(string content) {
        _logContent += $"{content}{Environment.NewLine}";
        OnPropertyChanged(nameof(LogContent));
        _window.LogTextBox.ScrollToEnd();
    }
}
using System;
using System.ComponentModel;
using System.IO;
using System.Windows.Forms;
using System.Windows.Input;

using binstarjs03.MineSharpOBJ.WpfApp.Services;

namespace binstarjs03.MineSharpOBJ.WpfApp.UIElements.Windows;

public class DebugLogWindowVM : ViewModelWindow<DebugLogWindowVM, DebugLogWindow>
{
    public DebugLogWindowVM(DebugLogWindow window) : base(window)
    {
        // listen to shared property change
        SharedProperty.PropertyChanged += OnSharedPropertyChanged;

        // listen to service events
        LogService.LogHandlers += OnLogServiceLogging;
        LogService.GetLogContentHandlers += OnLogServiceGetLogContent;

        // assign command implementation to commands
        SaveLogCommand = new RelayCommand(OnSaveLog);
        ClearLogCommand = new RelayCommand(OnClearLog);
        WriteSingleCommand = new RelayCommand(OnWriteSingle);
        WriteHorizontalCommand = new RelayCommand(OnWriteHorizontal);
        WriteVerticalCommand = new RelayCommand(OnWriteVertical);
    }

    // States -----------------------------------------------------------------

    public bool IsDebugLogWindowVisible
    {
        get => SharedProperty.IsDebugLogWindowVisible;
        set => SetSharedPropertyChanged
        (
            value,
            SharedProperty.IsDebugLogWindowVisibleUpdater
        );
    }

    private string _logContent = string.Empty;
    public string LogContent
    {
        get => _logContent;
        set => SetAndNotifyPropertyChanged(value, ref _logContent);
    }

    // Commands ---------------------------------------------------------------

    public ICommand SaveLogCommand { get; }
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

    public ICommand ClearLogCommand { get; }
    private void OnClearLog(object? arg)
    {
        LogContent = "";
        LogService.LogRuntimeInfo();
    }

    public ICommand WriteSingleCommand { get; }
    private void OnWriteSingle(object? arg)
    {
        LogService.Log("Hello World!");
   }

    public ICommand WriteHorizontalCommand { get; }
    private void OnWriteHorizontal(object? arg)
    {
        LogService.Log("Lorem ipsum dolor sit amet, consectetur adipiscing elit. Donec eget hendrerit nisl. In ut gravida metus. Suspendisse vitae gravida lacus. Nulla faucibus congue velit, at iaculis dolor interdum a. Nunc id metus sed nunc molestie varius. Cras lobortis auctor urna, ut pulvinar ante. Pellentesque vehicula lobortis nunc et iaculis. Vivamus at lacus tortor. Vivamus euismod eget quam sed rhoncus. Curabitur ipsum velit, venenatis et accumsan vitae, dictum nec augue.");
    }

    public ICommand WriteVerticalCommand { get; }
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

    // Event Handlers ---------------------------------------------------------

    protected override void OnSharedPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        string propertyName = e.PropertyName!;
        if (propertyName == nameof(IsDebugLogWindowVisible))
            NotifyPropertyChanged(nameof(IsDebugLogWindowVisible));
    }

    private void OnLogServiceLogging(string content)
    {
        LogContent += $"{content}{Environment.NewLine}";
        Window.LogTextBox.ScrollToEnd();
    }

    private string OnLogServiceGetLogContent()
    {
        return LogContent;
    }
}

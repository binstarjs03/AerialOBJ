using System;
using System.Windows.Interop;

using binstarjs03.AerialOBJ.WpfAppNew.Components.Interfaces;
using binstarjs03.AerialOBJ.WpfAppNew.Services;

using CommunityToolkit.Mvvm.Input;

using Microsoft.Win32;

namespace binstarjs03.AerialOBJ.WpfAppNew.ViewModel;

#pragma warning disable CA1822 // Mark members as static
public partial class DebugLogViewModel : BaseViewModel, IScroller
{
    public event Action? ScrollToEndRequested;

    public bool IsVisible
    {
        get => StateService.IsDebugLogWindowVisible;
        set => StateService.IsDebugLogWindowVisible = value;
    }

    public string LogContent => LogService.LogContent;

    public DebugLogViewModel()
    {
        StateService.DebugLogWindowVisibilityChanged += OnVisibilityChanged;
        LogService.Logging += OnLogServiceLogging;
    }

    private void OnVisibilityChanged(bool obj) => OnPropertyChanged(nameof(IsVisible));

    private void OnLogServiceLogging(string content)
    {
        OnPropertyChanged(nameof(LogContent));
        ScrollToEndRequested?.Invoke();
    }

    [RelayCommand]
    private void OnSave()
    {
        SaveFileDialog dialog = new()
        {
            FileName = $"{StateService.AppName} Log",
            DefaultExt = ".txt",
            Filter = "Text Document (.txt)|*.txt"
        };
        bool? result = dialog.ShowDialog();
        if (result != true)
            return;
        string path = dialog.FileName;
        try
        {
            IOService.WriteText(path, LogContent);
        }
        catch (Exception ex)
        {
            string msg = $"Canot save log content to {path}:\n{ex}";
            ModalService.ShowErrorOKModal($"Canot save log", msg);
            LogService.LogEmphasis(msg, LogService.Emphasis.Error);
        }
    }

    [RelayCommand]

    private void OnClearLog() => LogService.ClearLogContent();
}
#pragma warning restore CA1822 // Mark members as static

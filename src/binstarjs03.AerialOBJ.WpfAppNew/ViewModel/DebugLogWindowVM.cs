using System;

using binstarjs03.AerialOBJ.WpfAppNew.Components.Interfaces;
using binstarjs03.AerialOBJ.WpfAppNew.Services;

using CommunityToolkit.Mvvm.Input;

using Microsoft.Win32;

namespace binstarjs03.AerialOBJ.WpfAppNew.ViewModel;

#pragma warning disable CA1822 // Mark members as static
public partial class DebugLogWindowVM : BaseViewModel, IScroller
{
    public event Action? RequestScrollToEnd;

    public bool IsVisible
    {
        get => SharedStateService.IsDebugLogWindowVisible;
        set => SharedStateService.IsDebugLogWindowVisible = value;
    }

    public string LogContent => LogService.LogContent;

    public DebugLogWindowVM()
    {
        SharedStateService.DebugLogWindowVisibilityChanged += OnVisibilityChanged;
        LogService.Logging += OnLogServiceLogging;
    }

    private void OnVisibilityChanged(bool obj) => OnPropertyChanged(nameof(IsVisible));

    private void OnLogServiceLogging(string content)
    {
        OnPropertyChanged(nameof(LogContent));
        RequestScrollToEnd?.Invoke();
    }

    [RelayCommand]
    private void OnSave()
    {
        SaveFileDialog dialog = new()
        {
            FileName = $"{SharedStateService.AppName} Log",
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

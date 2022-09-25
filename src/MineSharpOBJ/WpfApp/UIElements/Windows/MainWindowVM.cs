﻿using System.ComponentModel;
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

    private string _title = "MineSharpOBJ";
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

    // we can make this property as static, but XAML databinding
    // intellisense won't detect this property anymore
    public bool HasSession => SharedProperty.SessionInfo is not null;

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
        LogService.Log("Attempting to closing savegame...");
        if (SharedProperty.SessionInfo is null)
            LogService.LogWarning($"{nameof(SharedProperty.SessionInfo)} is already null!");
        SharedProperty.SessionInfoUpdater(null);
        LogService.LogSuccess("Successfully closed session.", useSeparator: true);
    }

    // Event Handlers ---------------------------------------------------------

    protected override void OnSharedPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        string propertyName = e.PropertyName!;
        if (propertyName == nameof(IsDebugLogWindowVisible))
            NotifyPropertyChanged(nameof(IsDebugLogWindowVisible));
        else if (propertyName == nameof(SessionInfo))
        {
            NotifyPropertyChanged(nameof(HasSession));
            if (SharedProperty.SessionInfo is null)
                Title = "MineSharpOBJ";
            else
                Title = $"MineSharpOBJ - {SharedProperty.SessionInfo.WorldName}";
        }

    }
}

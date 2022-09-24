using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        AboutCommand = new RelayCommand(OnAbout);
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

    // Commands ---------------------------------------------------------------

    public ICommand AboutCommand { get; }
    private void OnAbout(object? arg)
    {
        ModalService.ShowAboutModal();
    }

    // Event Handlers ---------------------------------------------------------

    protected override void OnSharedPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        string propertyName = e.PropertyName!;
        if (propertyName == nameof(IsDebugLogWindowVisible))
            NotifyPropertyChanged(nameof(IsDebugLogWindowVisible));
    
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using binstarjs03.MineSharpOBJ.WpfApp.Views;

namespace binstarjs03.MineSharpOBJ.WpfApp.ViewModels;
public class MainViewModel : ViewModelBase<MainViewModel> {
    private bool _isDebugLogWindowVisible;

    public MainViewModel() : base() {
        _isDebugLogWindowVisible = false;
        OpenAboutView = new RelayCommand(OnOpenAboutView);
    }

    public bool IsDebugLogViewVisible {
        get { return _isDebugLogWindowVisible; }
        set {
            if (value == _isDebugLogWindowVisible)
                return;
            _isDebugLogWindowVisible = value;
            DebugLogViewModel.GetInstance.IsVisible = value;
            OnPropertyChanged(nameof(IsDebugLogViewVisible));
        }
    }

    public ICommand OpenAboutView { get; }

    private void OnOpenAboutView(object? arg) {
        new AboutView().ShowDialog();
    }
}
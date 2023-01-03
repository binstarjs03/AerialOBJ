using System;

namespace binstarjs03.AerialOBJ.WpfApp;
public class ViewState
{
    private bool _isDebugLogViewVisible = false;

    public event Action<string>? PropertyChanged;

    public bool IsDebugLogWindowVisible
    {
        get => _isDebugLogViewVisible;
        set
        {
            if (value == _isDebugLogViewVisible)
                return;
            _isDebugLogViewVisible = value;
            OnPropertyChanged();
        }
    }

    private void OnPropertyChanged()
    {
        PropertyChanged?.Invoke(nameof(ViewState));
    }
}

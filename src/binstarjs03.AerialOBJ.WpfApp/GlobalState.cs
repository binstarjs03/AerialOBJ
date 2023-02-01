using System;

using binstarjs03.AerialOBJ.WpfApp.Models;

using CommunityToolkit.Mvvm.ComponentModel;

namespace binstarjs03.AerialOBJ.WpfApp;

[ObservableObject]
public partial class GlobalState
{
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasSavegameLoaded))]
    private SavegameLoadInfo? _savegameLoadInfo = null;

    public bool HasSavegameLoaded => SavegameLoadInfo is not null;

    public event Action<SavegameLoadState>? SavegameLoadInfoChanged;

    partial void OnSavegameLoadInfoChanged(SavegameLoadInfo? value)
    {
        SavegameLoadState state;
        if (value is null)
        {
            state = SavegameLoadState.Closed;
            GC.Collect();
        }
        else
            state = SavegameLoadState.Opened;
        SavegameLoadInfoChanged?.Invoke(state);
    }
}
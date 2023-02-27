using System;

using binstarjs03.AerialOBJ.MvvmAppCore.Models;

using CommunityToolkit.Mvvm.ComponentModel;

namespace binstarjs03.AerialOBJ.MvvmAppCore;

public partial class GlobalState : ObservableObject
{
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasSavegameLoaded))]
    private SavegameLoadInfo? _savegameLoadInfo = null;

    public bool HasSavegameLoaded => SavegameLoadInfo is not null;

    public event Action<SavegameLoadInfo?>? SavegameLoadInfoChanged;

    partial void OnSavegameLoadInfoChanged(SavegameLoadInfo? value)
    {
        if (value is null)
            GC.Collect();
        SavegameLoadInfoChanged?.Invoke(value);
    }
}
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace binstarjs03.MineSharpOBJ.WpfApp.UIElements;
public static class SharedProperty
{
    // Subscribe to this event if your object is using shared property and want
    // to be notified when there is a change
    public static event PropertyChangedEventHandler? PropertyChanged;

    // Internal Logic ---------------------------------------------------------

    private readonly static Type s_this = typeof(SharedProperty);

    private static void NotifyPropertyChanged<T>(T newValue, ref T oldValue, bool canNull = false, [CallerMemberName] string propertyName = "")
    {
        if (canNull)
        {
            if (newValue is null && oldValue is null)
                return;
        }
        else
        {
            if (newValue is null || oldValue is null)
                throw new ArgumentNullException
                (
                    "newValue or oldValue",
                    "Argument oldValue passed to ValueChanged of ViewModelBase is null"
                );
            if (newValue.Equals(oldValue))
                return;
        }
        oldValue = newValue;
        PropertyChanged?.Invoke(s_this, new PropertyChangedEventArgs(propertyName));
    }

    // Shared Property Data ---------------------------------------------------

    private static bool s_isDebugLogWindowVisible = false;
    public static bool IsDebugLogWindowVisible
    {
        get => s_isDebugLogWindowVisible;
        set => NotifyPropertyChanged(value, ref s_isDebugLogWindowVisible);
    }
    public static void IsDebugLogWindowVisibleUpdater(bool value)
    {
        IsDebugLogWindowVisible = value;
    }

    private static SessionInfo? s_sessionInfo = null;
    public static SessionInfo? SessionInfo
    {
        get => s_sessionInfo;
        set => NotifyPropertyChanged(value, ref s_sessionInfo, canNull: true);
    }
    public static void SessionInfoUpdater(SessionInfo? value)
    {
        SessionInfo = value;
    }
}

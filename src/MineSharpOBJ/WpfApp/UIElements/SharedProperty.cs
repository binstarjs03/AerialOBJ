using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace binstarjs03.MineSharpOBJ.WpfApp.UIElements;

// TODO for properties that needs to be synchronized between viewmodels
// and to reduce coupling, make it local in each VM when make sense.
// Use events and make the VM listen to that, so when that event is triggered,
// update the property individually

// For properties that needs to be synchronized between viewmodels,
// put it here.
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

    private static void NotifyPropertyChanged(string propertyName)
    {
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


    // False if SessionInfo is null
    private static bool s_isViewportCameraPositionGuideVisible = false;
    public static bool IsViewportCameraPositionGuideVisible
    {
        get => s_isViewportCameraPositionGuideVisible;
        set => NotifyPropertyChanged(value, ref s_isViewportCameraPositionGuideVisible);
    }
    public static void IsViewportCameraPositionGuideVisibleUpdater(bool value)
    {
        IsViewportCameraPositionGuideVisible = value;
    }


    // False if SessionInfo is null
    private static bool s_isViewportDebugInfoVisible = false;
    public static bool IsViewportDebugInfoVisible
    {
        get => s_isViewportDebugInfoVisible;
        set => NotifyPropertyChanged(value, ref s_isViewportDebugInfoVisible);
    }
    public static void IsViewportDebugInfoVisibleUpdater(bool value)
    {
        IsViewportDebugInfoVisible = value;
    }



    private static SessionInfo? s_sessionInfo = null;
    public static SessionInfo? SessionInfo
    {
        get => s_sessionInfo;
        set
        {
            NotifyPropertyChanged(value, ref s_sessionInfo, canNull: true);
            NotifyPropertyChanged(nameof(HasSession));
        }
    }
    public static void SessionInfoUpdater(SessionInfo? value)
    {
        SessionInfo = value;
        if (value is null)
        {
            IsViewportCameraPositionGuideVisible = false;
            IsViewportDebugInfoVisible = false;
        }
    }

    public static bool HasSession => SessionInfo is not null;
}

using System;

namespace binstarjs03.AerialOBJ.MVVM.Models.Settings;

public class SettingLoadingException : Exception
{
    public SettingLoadingException() { }
    public SettingLoadingException(string message) : base(message) { }
    public SettingLoadingException(string message, Exception inner) : base(message, inner) { }
}

public class SettingIOException : Exception
{
    public SettingIOException() { }
    public SettingIOException(string message) : base(message) { }
    public SettingIOException(string message, Exception inner) : base(message, inner) { }
}

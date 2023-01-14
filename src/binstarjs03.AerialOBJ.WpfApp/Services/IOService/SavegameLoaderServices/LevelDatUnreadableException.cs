using System;

namespace binstarjs03.AerialOBJ.WpfApp.Services.IOService.SavegameLoaderServices;
public class LevelDatUnreadableException : Exception
{
    public LevelDatUnreadableException() { }
    public LevelDatUnreadableException(string? message) : base(message) { }
    public LevelDatUnreadableException(string? message, Exception? innerException) : base(message, innerException) { }
}
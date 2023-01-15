using System;

namespace binstarjs03.AerialOBJ.WpfApp.Services.IOService.SavegameLoader;

public class LevelDatNotFoundException : Exception
{
    public LevelDatNotFoundException() { }
    public LevelDatNotFoundException(string? message) : base(message) { }
    public LevelDatNotFoundException(string? message, Exception? innerException) : base(message, innerException) { }
}

using System;

namespace binstarjs03.AerialOBJ.WpfAppNew2.Services;

public class LevelDatNotFoundException : Exception
{
    public LevelDatNotFoundException() { }
    public LevelDatNotFoundException(string? message) : base(message) { }
    public LevelDatNotFoundException(string? message, Exception? innerException) : base(message, innerException) { }
}

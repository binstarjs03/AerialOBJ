using System;

namespace binstarjs03.AerialOBJ.MvvmAppCore.Services.IOService.SavegameLoader;

public class LevelDatNotFoundException : Exception
{
    public LevelDatNotFoundException() { }
    public LevelDatNotFoundException(string? message) : base(message) { }
    public LevelDatNotFoundException(string? message, Exception? innerException) : base(message, innerException) { }
}

public class LevelDatNotImplementedParserException : Exception
{
    public LevelDatNotImplementedParserException() { }
    public LevelDatNotImplementedParserException(string? message) : base(message) { }
    public LevelDatNotImplementedParserException(string? message, Exception? innerException) : base(message, innerException) { }
}

public class LevelDatUnreadableException : Exception
{
    public LevelDatUnreadableException() { }
    public LevelDatUnreadableException(string? message) : base(message) { }
    public LevelDatUnreadableException(string? message, Exception? innerException) : base(message, innerException) { }
}
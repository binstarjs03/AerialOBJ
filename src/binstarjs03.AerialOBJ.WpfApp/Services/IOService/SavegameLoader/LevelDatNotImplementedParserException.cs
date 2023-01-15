using System;
using System.Runtime.Serialization;

namespace binstarjs03.AerialOBJ.WpfApp.Services.IOService.SavegameLoader;
[Serializable]
internal class LevelDatNotImplementedParserException : Exception
{
    public LevelDatNotImplementedParserException()
    {
    }

    public LevelDatNotImplementedParserException(string? message) : base(message)
    {
    }

    public LevelDatNotImplementedParserException(string? message, Exception? innerException) : base(message, innerException)
    {
    }

    protected LevelDatNotImplementedParserException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
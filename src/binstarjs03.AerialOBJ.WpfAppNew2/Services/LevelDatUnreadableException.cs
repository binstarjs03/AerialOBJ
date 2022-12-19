using System;
using System.Runtime.Serialization;

namespace binstarjs03.AerialOBJ.WpfAppNew2.Services;
[Serializable]
internal class LevelDatUnreadableException : Exception
{
    public LevelDatUnreadableException()
    {
    }

    public LevelDatUnreadableException(string? message) : base(message)
    {
    }

    public LevelDatUnreadableException(string? message, Exception? innerException) : base(message, innerException)
    {
    }

    protected LevelDatUnreadableException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
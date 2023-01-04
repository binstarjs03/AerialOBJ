using System;
using System.Runtime.Serialization;

namespace binstarjs03.AerialOBJ.WpfApp.Services;
[Serializable]
internal class OverwriteException : Exception
{
    public OverwriteException()
    {
    }

    public OverwriteException(string? message) : base(message)
    {
    }

    public OverwriteException(string? message, Exception? innerException) : base(message, innerException)
    {
    }

    protected OverwriteException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
using System;

namespace binstarjs03.AerialOBJ.WpfApp.Services;
public class OverwriteException : Exception
{
    public OverwriteException() { }

    public OverwriteException(string? message) : base(message) { }

    public OverwriteException(string? message, Exception? innerException) : base(message, innerException) { }
}

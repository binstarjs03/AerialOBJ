using System;

namespace binstarjs03.AerialOBJ.WpfAppNew2.Converters;

public class ConverterException : Exception
{
    public ConverterException() { }
    public ConverterException(string? message) : base(message) { }
    public ConverterException(string? message, Exception? innerException) : base(message, innerException) { }
}
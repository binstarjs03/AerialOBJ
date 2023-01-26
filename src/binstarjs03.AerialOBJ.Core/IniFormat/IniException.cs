using System;

namespace binstarjs03.AerialOBJ.Core.IniFormat;


public class IniDeserializationException : Exception
{
    public IniDeserializationException() { }
    public IniDeserializationException(string message) : base(message) { }
    public IniDeserializationException(string message, Exception inner) : base(message, inner) { }
}

public class IniInvalidSectionException : IniDeserializationException
{
    public IniInvalidSectionException() { }
    public IniInvalidSectionException(string message) : base(message) { }
    public IniInvalidSectionException(string message, Exception inner) : base(message, inner) { }
}

public class IniInvalidStringException : IniDeserializationException
{
    public IniInvalidStringException() { }
    public IniInvalidStringException(string message) : base(message) { }
    public IniInvalidStringException(string message, Exception inner) : base(message, inner) { }
}

public class IniInvalidKeyValueException : IniDeserializationException
{
    public IniInvalidKeyValueException() { }
    public IniInvalidKeyValueException(string message) : base(message) { }
    public IniInvalidKeyValueException(string message, Exception inner) : base(message, inner) { }
}
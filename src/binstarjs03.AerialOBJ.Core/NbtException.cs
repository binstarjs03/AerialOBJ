using System;

namespace binstarjs03.AerialOBJ.Core;

/// <summary>
/// Root exception for nbt-related exceptions. instead catching <see cref="Exception"/> , 
/// we can just catch <see cref="NbtException"/> to catch nbt-related exceptions
/// </summary>
public class NbtException : Exception
{
    public NbtException() { }
    public NbtException(string message) : base(message) { }
    public NbtException(string message, Exception innerException) : base(message, innerException) { }
}

/// <summary>
/// The exception that is thrown when parsing nbt from data stream compression 
/// format is unrecognized
/// </summary>
public class NbtUnknownCompressionMethodException : NbtException
{
    public NbtUnknownCompressionMethodException() { }
    public NbtUnknownCompressionMethodException(string message) : base(message) { }
    public NbtUnknownCompressionMethodException(string message, Exception inner) : base(message, inner) { }
}

/// <summary>
/// The exception that is thrown when parsing nbt from data stream encountered 
/// byte data that is unrecognized nbt type
/// </summary>
public class NbtUnknownTypeException : NbtException
{
    public NbtUnknownTypeException() { }
    public NbtUnknownTypeException(string message) : base(message) { }
    public NbtUnknownTypeException(string message, Exception inner) : base(message, inner) { }
}

/// <summary>
/// The exception that is thrown when there is an exception while parsing nbt 
/// from data stream
/// </summary>
public class NbtDeserializationError : NbtException
{
    public NbtDeserializationError() { }
    public NbtDeserializationError(string message) : base(message) { }
    public NbtDeserializationError(string message, Exception inner) : base(message, inner) { }
}

/// <summary>
/// The exception that is thrown when parsing nbt from data stream has no data 
/// (zero-length stream data)
/// </summary>
public class NbtNoDataException : NbtException
{
    public NbtNoDataException() { }
    public NbtNoDataException(string message) : base(message) { }
    public NbtNoDataException(string message, Exception inner) : base(message, inner) { }
}

public class NbtIllegalOperationException : Exception
{
    public NbtIllegalOperationException() { }
    public NbtIllegalOperationException(string message) : base(message) { }
    public NbtIllegalOperationException(string message, Exception inner) : base(message, inner) { }
}
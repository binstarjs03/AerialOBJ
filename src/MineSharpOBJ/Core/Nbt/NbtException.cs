using System;
namespace binstarjs03.MineSharpOBJ.Core.Nbt;

public class NbtException : Exception {
    public NbtException() { }
    public NbtException(string message) : base(message) { }
    public NbtException(string message, Exception innerException) : base(message, innerException) { }
}

public class NbtUnknownTagTypeException : NbtException {
    public NbtUnknownTagTypeException() { }
    public NbtUnknownTagTypeException(string message) : base(message) { }
    public NbtUnknownTagTypeException(string message, Exception inner) : base(message, inner) { }
}

public class NbtDeserializationError : NbtException {
    public NbtDeserializationError() { }
    public NbtDeserializationError(string message) : base(message) { }
    public NbtDeserializationError(string message, Exception inner) : base(message, inner) { }
}

public class NbtUnknownCompressionMethodException : NbtException {
    public NbtUnknownCompressionMethodException() { }
    public NbtUnknownCompressionMethodException(string message) : base(message) { }
    public NbtUnknownCompressionMethodException(string message, Exception inner) : base(message, inner) { }
}

public class NbtNoDataException : NbtException {
    public NbtNoDataException() { }
    public NbtNoDataException(string message) : base(message) { }
    public NbtNoDataException (string message, Exception inner) : base(message, inner) { }
}
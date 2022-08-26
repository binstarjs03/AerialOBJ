using System;
namespace binstarjs03.MineSharpOBJ.Core.Nbt;


public class NbtUnknownTagTypeException : Exception {
    public NbtUnknownTagTypeException() { }
    public NbtUnknownTagTypeException(string message) : base(message) { }
    public NbtUnknownTagTypeException(string message, Exception inner) : base(message, inner) { }
}

public class NbtDeserializationError : Exception {
    public NbtDeserializationError() { }
    public NbtDeserializationError(string message) : base(message) { }
    public NbtDeserializationError(string message, Exception inner) : base(message, inner) { }
}

public class NbtUnknownCompressionMethodException : Exception {
    public NbtUnknownCompressionMethodException() { }
    public NbtUnknownCompressionMethodException(string message) : base(message) { }
    public NbtUnknownCompressionMethodException(string message, Exception inner) : base(message, inner) { }
}

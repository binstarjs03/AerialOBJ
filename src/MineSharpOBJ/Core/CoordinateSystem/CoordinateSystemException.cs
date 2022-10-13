using System;

namespace binstarjs03.MineSharpOBJ.Core.CoordinateSystem;

public class CoordsOutOfRangeException : ArgumentOutOfRangeException
{
    public CoordsOutOfRangeException() { }
    public CoordsOutOfRangeException(string message) : base(message) { }
    public CoordsOutOfRangeException(string message, Exception innerException) : base(message, innerException) { }
}

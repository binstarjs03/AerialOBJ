using System;
using System.Numerics;

namespace binstarjs03.AerialOBJ.Core.Primitives;

public struct Size<TNumber> : IEquatable<Size<TNumber>> where TNumber : struct, INumber<TNumber>
{
    public TNumber Width { get; set; }
    public TNumber Height { get; set; }

    public Size(TNumber width, TNumber height)
    {
        Width = width;
        Height = height;
    }

    public override string ToString()
    {
        return $"({Width}, {Height})";
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Width, Height);
    }

    public override bool Equals(object? obj)
    {
        if (obj is Size<TNumber> o)
            return Equals(o);
        return false;
    }

    public bool Equals(Size<TNumber> other)
    {
        return Width == other.Width
            && Height == other.Height;
    }

    public static bool operator ==(Size<TNumber> left, Size<TNumber> right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(Size<TNumber> left, Size<TNumber> right)
    {
        return !(left == right);
    }
}

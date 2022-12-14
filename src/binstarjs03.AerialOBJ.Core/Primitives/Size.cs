using System;
using System.Numerics;

namespace binstarjs03.AerialOBJ.Core.Primitives;

public struct Size<TNumber> :
    IEquatable<Size<TNumber>>,
    IEqualityOperators<Size<TNumber>, Size<TNumber>, bool>,
    IDivisionOperators<Size<TNumber>, TNumber, Size<TNumber>>
    where TNumber : struct, INumber<TNumber>
{
    public TNumber Width { get; set; }
    public TNumber Height { get; set; }

    public Size(TNumber width, TNumber height)
    {
        Width = width;
        Height = height;
    }

    public Point2<TNumber> GetMidPoint()
    {
        TNumber two = TNumber.One + TNumber.One;
        return new Point2<TNumber>(Width / two, Height / two);
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

    public static Size<TNumber> operator /(Size<TNumber> left, TNumber right)
    {
        return new Size<TNumber>(left.Width / right, left.Height / right);
    }
}

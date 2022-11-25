using System;
using System.Numerics;

namespace binstarjs03.AerialOBJ.Core.Primitives;

public struct Size<TNumber> :
    IEquatable<Size<TNumber>>,
    IEqualityOperators<Size<TNumber>, Size<TNumber>, bool>
    where TNumber : struct, INumber<TNumber>
{
    private TNumber _width;
    private TNumber _height;

    public TNumber Width
    {
        get => _width;
        set
        {
            if (value < TNumber.Zero)
                throw new ArgumentException($"{nameof(Width)} cannot be negative");
            _width = value;
        }
    }
    public TNumber Height
    {
        get => _height;
        set
        {
            if (value < TNumber.Zero)
                throw new ArgumentException($"{nameof(Height)} cannot be negative");
            _height = value;
        }
    }

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
}

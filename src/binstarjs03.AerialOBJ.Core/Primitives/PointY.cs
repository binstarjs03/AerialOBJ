using System;
using System.Numerics;

namespace binstarjs03.AerialOBJ.Core.Primitives;

public struct PointY<TNumber> : IEquatable<PointY<TNumber>> where TNumber : struct, INumber<TNumber>
{
    public static PointY<TNumber> Zero => new();
    public TNumber X { get; set; }
    public TNumber Y { get; set; }

    public PointY()
    {
        X = TNumber.Zero;
        Y = TNumber.Zero;
    }

    public PointY(TNumber x, TNumber y)
    {
        X = x;
        Y = y;
    }

    public override string ToString()
    {
        return $"({X}, {Y})";
    }
    public override int GetHashCode()
    {
        return HashCode.Combine(X, Y);
    }

    public override bool Equals(object? obj)
    {
        if (obj is PointY<TNumber> o)
            return Equals(o);
        return false;
    }

    public bool Equals(PointY<TNumber> other)
    {
        return X == other.X
            && Y == other.Y;
    }

    public static bool operator ==(PointY<TNumber> left, PointY<TNumber> right)
    {
        return left.Equals(right);
    }

    public static implicit operator PointY<TNumber>(PointZ<TNumber> point)
    {
        return new PointY<TNumber>(point.X, point.Z);
    }

    public static bool operator !=(PointY<TNumber> left, PointY<TNumber> right)
    {
        return !(left == right);
    }

    public static PointY<TNumber> operator -(PointY<TNumber> self)
    {
        return new PointY<TNumber>(-self.X, -self.Y);
    }

    public static PointY<TNumber> operator -(PointY<TNumber> left, PointY<TNumber> right)
    {
        return new PointY<TNumber>(left.X - right.X, left.Y - right.Y);
    }

    public static PointY<TNumber> operator *(PointY<TNumber> left, TNumber right)
    {
        return new PointY<TNumber>(left.X * right, left.Y * right);
    }

    public static PointY<TNumber> operator /(PointY<TNumber> left, TNumber right)
    {
        return new PointY<TNumber>(left.X / right, left.Y / right);
    }
}

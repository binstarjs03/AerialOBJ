using System;
using System.Numerics;

namespace binstarjs03.AerialOBJ.Core.Primitives;

public struct PointZ<TNumber> : IEquatable<PointZ<TNumber>>, IComparable<PointZ<TNumber>> where TNumber : struct, INumber<TNumber>
{
    public static PointZ<TNumber> Zero => new();
    public TNumber X { get; set; }
    public TNumber Z { get; set; }

    public PointZ()
    {
        X = TNumber.Zero;
        Z = TNumber.Zero;
    }

    public PointZ(TNumber x, TNumber z)
    {
        X = x;
        Z = z;
    }

    public override string ToString()
    {
        return $"({X}, {Z})";
    }
    public override int GetHashCode()
    {
        return HashCode.Combine(X, Z);
    }

    public override bool Equals(object? obj)
    {
        if (obj is PointZ<TNumber> o)
            return Equals(o);
        return false;
    }

    public bool Equals(PointZ<TNumber> other)
    {
        return X == other.X
            && Z == other.Z;
    }

    public int CompareTo(PointZ<TNumber> other)
    {
        int xval = X.CompareTo(other.X);
        int zval = Z.CompareTo(other.Z);
        int sum = xval * 2 + zval;
        return sum;
    }

    public static bool operator ==(PointZ<TNumber> left, PointZ<TNumber> right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(PointZ<TNumber> left, PointZ<TNumber> right)
    {
        return !(left == right);
    }

    public static implicit operator PointZ<TNumber>(PointY<TNumber> point)
    {
        return new PointZ<TNumber>(point.X, point.Y);
    }

    public static PointZ<TNumber> operator +(PointZ<TNumber> left, PointZ<TNumber> right)
    {
        return new PointZ<TNumber>(left.X + right.X, left.Z + right.Z);
    }

    public static PointZ<TNumber> operator *(PointZ<TNumber> left, TNumber right)
    {
        return new PointZ<TNumber>(left.X * right, left.Z * right);
    }

    public static PointZ<TNumber> operator /(PointZ<TNumber> left, TNumber right)
    {
        return new PointZ<TNumber>(left.X / right, left.Z / right);
    }
}

using System;
using System.Numerics;

namespace binstarjs03.AerialOBJ.Core.Primitives;

public struct PointZ<TNumber> : IEquatable<PointZ<TNumber>> where TNumber : struct, INumber<TNumber>
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

    public static bool operator ==(PointZ<TNumber> left, PointZ<TNumber> right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(PointZ<TNumber> left, PointZ<TNumber> right)
    {
        return !(left == right);
    }
}

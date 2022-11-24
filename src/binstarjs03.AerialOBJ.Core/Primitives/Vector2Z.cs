using System;
using System.Numerics;

namespace binstarjs03.AerialOBJ.Core.Primitives;

public struct Vector2Z<TNumber> :
    IEquatable<Vector2Z<TNumber>>,
    IEqualityOperators<Vector2Z<TNumber>, Vector2Z<TNumber>, bool>
    where TNumber : struct, INumber<TNumber>
{
    public static Vector2Z<TNumber> Zero => new();
    public TNumber X { get; set; }
    public TNumber Z { get; set; }

    public Vector2Z()
    {
        X = TNumber.Zero;
        Z = TNumber.Zero;
    }

    public Vector2Z(TNumber x, TNumber z)
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
        if (obj is Vector2Z<TNumber> o)
            return Equals(o);
        return false;
    }

    public bool Equals(Vector2Z<TNumber> other)
    {
        return X == other.X
            && Z == other.Z;
    }

    public static bool operator ==(Vector2Z<TNumber> left, Vector2Z<TNumber> right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(Vector2Z<TNumber> left, Vector2Z<TNumber> right)
    {
        return !(left == right);
    }
}

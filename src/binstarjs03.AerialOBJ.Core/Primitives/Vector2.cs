using System;
using System.Numerics;

namespace binstarjs03.AerialOBJ.Core.Primitives;

public struct Vector2<TNumber> :
    IEquatable<Vector2<TNumber>>,
    IEqualityOperators<Vector2<TNumber>, Vector2<TNumber>, bool>
    where TNumber : struct, INumber<TNumber>
{
    public static Vector2<TNumber> Zero => new();
    public TNumber X { get; set; }
    public TNumber Y { get; set; }

    public Vector2()
    {
        X = TNumber.Zero;
        Y = TNumber.Zero;
    }

    public Vector2(TNumber x, TNumber y)
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
        if (obj is Vector2<TNumber> o)
            return Equals(o);
        return false;
    }

    public bool Equals(Vector2<TNumber> other)
    {
        return X == other.X
            && Y == other.Y;
    }

    public static bool operator ==(Vector2<TNumber> left, Vector2<TNumber> right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(Vector2<TNumber> left, Vector2<TNumber> right)
    {
        return !(left == right);
    }
}

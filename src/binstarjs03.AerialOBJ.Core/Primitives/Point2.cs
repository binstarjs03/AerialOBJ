using System;
using System.Numerics;

namespace binstarjs03.AerialOBJ.Core.Primitives;
public struct Point2<TNumber> : 
    IEquatable<Point2<TNumber>>, 
    IEqualityOperators<Point2<TNumber>, Point2<TNumber>, bool> 
    where TNumber : struct, INumber<TNumber>
{
    public static Point2<TNumber> Zero => new();
    public TNumber X { get; set; }
    public TNumber Y { get; set; }

    public Point2()
    {
        X = TNumber.Zero;
        Y = TNumber.Zero;
    }

    public Point2(TNumber x, TNumber y)
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
        if (obj is Point2<TNumber> o)
            return Equals(o);
        return false;
    }

    public bool Equals(Point2<TNumber> other)
    {
        return X == other.X
            && Y == other.Y;
    }

    public static bool operator ==(Point2<TNumber> left, Point2<TNumber> right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(Point2<TNumber> left, Point2<TNumber> right)
    {
        return !(left == right);
    }
}

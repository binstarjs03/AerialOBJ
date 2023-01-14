using System;
using System.Numerics;

namespace binstarjs03.AerialOBJ.Core.Primitives;

public struct Point2<TNumber> :
    IEquatable<Point2<TNumber>>,
    IEqualityOperators<Point2<TNumber>, Point2<TNumber>, bool>,
    IAdditionOperators<Point2<TNumber>, Point2<TNumber>, Point2<TNumber>>,
    ISubtractionOperators<Point2<TNumber>, Point2<TNumber>, Point2<TNumber>>,
    IMultiplyOperators<Point2<TNumber>, TNumber, Point2<TNumber>>
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

    public static Point2<TNumber> operator *(Point2<TNumber> left, TNumber right)
    {
        return new Point2<TNumber>(left.X * right, left.Y * right);
    }

    public static Point2<TNumber> operator +(Point2<TNumber> left, Point2<TNumber> right)
    {
        return new Point2<TNumber>(left.X + right.X, left.Y + right.Y);
    }

    public static Point2<TNumber> operator -(Point2<TNumber> left, Point2<TNumber> right)
    {
        return new Point2<TNumber>(left.X - right.X, left.Y - right.Y);
    }

    public static explicit operator Point2<TNumber>(Point2Z<TNumber> point)
    {
        return new Point2<TNumber>(point.X, point.Z);
    }

    public static explicit operator Point2<TNumber>(Size<TNumber> size)
    {
        return new Point2<TNumber>(size.Width, size.Height);
    }
}

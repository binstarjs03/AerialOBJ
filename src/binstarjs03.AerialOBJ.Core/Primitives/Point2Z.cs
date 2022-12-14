using System;
using System.Numerics;

namespace binstarjs03.AerialOBJ.Core.Primitives;

public struct Point2Z<TNumber> :
    IEquatable<Point2Z<TNumber>>,
    IEqualityOperators<Point2Z<TNumber>, Point2Z<TNumber>, bool>,
    IAdditionOperators<Point2Z<TNumber>, Vector2Z<TNumber>, Point2Z<TNumber>>,
    ISubtractionOperators<Point2Z<TNumber>, Vector2Z<TNumber>, Point2Z<TNumber>>,
    IAdditionOperators<Point2Z<TNumber>, TNumber, Point2Z<TNumber>>,
    ISubtractionOperators<Point2Z<TNumber>, TNumber, Point2Z<TNumber>>,
    IMultiplyOperators<Point2Z<TNumber>, TNumber, Point2Z<TNumber>>
    where TNumber : struct, INumber<TNumber>
{
    public static Point2Z<TNumber> Zero => new();
    public TNumber X { get; set; }
    public TNumber Z { get; set; }

    public Point2Z()
    {
        X = TNumber.Zero;
        Z = TNumber.Zero;
    }

    public Point2Z(TNumber x, TNumber z)
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
        if (obj is Point2Z<TNumber> o)
            return Equals(o);
        return false;
    }

    public bool Equals(Point2Z<TNumber> other)
    {
        return X == other.X
            && Z == other.Z;
    }

    public static bool operator ==(Point2Z<TNumber> left, Point2Z<TNumber> right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(Point2Z<TNumber> left, Point2Z<TNumber> right)
    {
        return !(left == right);
    }

    public static Point2Z<TNumber> operator -(Point2Z<TNumber> point)
    {
        return new Point2Z<TNumber>(-point.X, -point.Z);
    }

    public static Point2Z<TNumber> operator +(Point2Z<TNumber> left, Vector2Z<TNumber> right)
    {
        return new Point2Z<TNumber>(left.X + right.X, left.Z + right.Z);
    }

    public static Point2Z<TNumber> operator -(Point2Z<TNumber> left, Vector2Z<TNumber> right)
    {
        return new Point2Z<TNumber>(left.X - right.X, left.Z - right.Z);
    }

    public static Point2Z<TNumber> operator +(Point2Z<TNumber> left, TNumber right)
    {
        return new Point2Z<TNumber>(left.X + right, left.Z + right);
    }

    public static Point2Z<TNumber> operator -(Point2Z<TNumber> left, TNumber right)
    {
        return new Point2Z<TNumber>(left.X - right, left.Z - right);
    }

    public static Point2Z<TNumber> operator *(Point2Z<TNumber> left, TNumber right)
    {
        return new Point2Z<TNumber>(left.X * right, left.Z * right);
    }

    public static explicit operator Point2Z<TNumber>(Size<TNumber> size)
    {
        return new Point2Z<TNumber>(size.Width, size.Height);
    }
}

using System;
using System.Numerics;

namespace binstarjs03.AerialOBJ.Core.Primitives;
public struct Point3<TNumber> : IEquatable<Point3<TNumber>>, IEqualityOperators<Point3<TNumber>, Point3<TNumber>, bool> where TNumber : struct, INumber<TNumber>
{
    public static Point3<TNumber> Zero => new();
    public TNumber X { get; set; }
    public TNumber Y { get; set; }
    public TNumber Z { get; set; }

    public Point3()
    {
        X = TNumber.Zero;
        Y = TNumber.Zero;
        Z = TNumber.Zero;
    }

    public Point3(TNumber x, TNumber y, TNumber z)
    {
        X = x;
        Y = y;
        Z = z;
    }

    public override string ToString()
    {
        return $"({X}, {Y}, {Z})";
    }
    public override int GetHashCode()
    {
        return HashCode.Combine(X, Y, Z);
    }

    public override bool Equals(object? obj)
    {
        if (obj is Point3<TNumber> o)
            return Equals(o);
        return false;
    }

    public bool Equals(Point3<TNumber> other)
    {
        return X == other.X
            && Y == other.Y
            && Z == other.Z;
    }

    public static bool operator ==(Point3<TNumber> left, Point3<TNumber> right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(Point3<TNumber> left, Point3<TNumber> right)
    {
        return !(left == right);
    }
}

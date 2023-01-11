using System;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;

namespace binstarjs03.AerialOBJ.Core.Primitives;
public struct Point2ZRange<TNumber> :
    IEquatable<Point2ZRange<TNumber>>,
    IEqualityOperators<Point2ZRange<TNumber>, Point2ZRange<TNumber>, bool>
    where TNumber : struct, INumber<TNumber>
{
    public Rangeof<TNumber> XRange { get; set; }
    public Rangeof<TNumber> ZRange { get; set; }
    public TNumber Sum => XRange.Sum * ZRange.Sum;

    public Point2ZRange()
    {
        XRange = new();
        ZRange = new();
    }

    public Point2ZRange(Rangeof<TNumber> xRange, Rangeof<TNumber> zRange)
    {
        XRange = xRange;
        ZRange = zRange;
    }

    public Point2ZRange(Point2Z<TNumber> min, Point2Z<TNumber> max)
    {
        XRange = new Rangeof<TNumber>(min.X, max.X);
        ZRange = new Rangeof<TNumber>(min.Z, max.Z);
    }

    public Point2ZRange(TNumber minX, TNumber maxX, TNumber minZ, TNumber maxZ)
    {
        XRange = new Rangeof<TNumber>(minX, maxX);
        ZRange = new Rangeof<TNumber>(minZ, maxZ);
    }

    public bool IsInside(Point2Z<TNumber> point)
    {
        bool inX = XRange.IsInside(point.X);
        bool inZ = ZRange.IsInside(point.Z);
        return inX && inZ;
    }

    public bool IsOutside(Point2Z<TNumber> point)
    {
        return !IsInside(point);
    }

    public void ThrowIfOutside(Point2Z<TNumber> point)
    {
        if (IsOutside(point))
            throw new ArgumentOutOfRangeException(nameof(point));
    }

    public override string ToString()
    {
        return $"({XRange}, {ZRange})";
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(XRange, ZRange);
    }

    public override bool Equals([NotNullWhen(true)] object? obj)
    {
        if (obj is Point2ZRange<TNumber> o)
            return Equals(o);
        return false;
    }

    public bool Equals(Point2ZRange<TNumber> other)
    {
        return XRange == other.XRange
            && ZRange == other.ZRange;
    }

    public static bool operator ==(Point2ZRange<TNumber> left, Point2ZRange<TNumber> right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(Point2ZRange<TNumber> left, Point2ZRange<TNumber> right)
    {
        return !(left == right);
    }
}

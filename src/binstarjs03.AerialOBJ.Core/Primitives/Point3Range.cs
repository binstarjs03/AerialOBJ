using System;
using System.Numerics;

namespace binstarjs03.AerialOBJ.Core.Primitives;
public struct Point3Range<TNumber> : IEquatable<Point3Range<TNumber>> where TNumber : struct, INumber<TNumber>
{
    public Rangeof<TNumber> XRange { get; set; }
    public Rangeof<TNumber> YRange { get; set; }
    public Rangeof<TNumber> ZRange { get; set; }

    public TNumber Sum => XRange.Sum * YRange.Sum * ZRange.Sum;

    public Point3Range()
    {
        XRange = new();
        YRange = new();
        ZRange = new();
    }

    public Point3Range(Rangeof<TNumber> xRange, Rangeof<TNumber> yRange, Rangeof<TNumber> zRange)
    {
        XRange = xRange;
        YRange = yRange;
        ZRange = zRange;
    }

    public Point3Range(Point3<TNumber> min, Point3<TNumber> max)
    {
        XRange = new Rangeof<TNumber>(min.X, max.X);
        YRange = new Rangeof<TNumber>(min.Y, max.Y);
        ZRange = new Rangeof<TNumber>(min.Z, max.Z);
    }

    public Point3Range(TNumber minX, TNumber maxX, TNumber minY, TNumber maxY, TNumber minZ, TNumber maxZ)
    {
        XRange = new Rangeof<TNumber>(minX, maxX);
        YRange = new Rangeof<TNumber>(minY, maxY);
        ZRange = new Rangeof<TNumber>(minZ, maxZ);
    }

    public bool IsInside(Point3<TNumber> point)
    {
        bool inX = XRange.IsInside(point.X);
        bool inY = YRange.IsInside(point.Y);
        bool inZ = ZRange.IsInside(point.Z);
        return inX && inY && inZ;
    }

    public bool IsOutside(Point3<TNumber> point)
    {
        return !IsInside(point);
    }

    public void ThrowIfOutside(Point3<TNumber> point)
    {
        if (IsOutside(point))
            throw new ArgumentOutOfRangeException(nameof(point));
    }

    public override string ToString()
    {
        return $"({XRange}, {YRange}, {ZRange})";
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(XRange, YRange, ZRange);
    }

    public override bool Equals(object? obj)
    {
        if (obj is Point3Range<TNumber> o)
            return Equals(o);
        return false;
    }

    public bool Equals(Point3Range<TNumber> other)
    {
        return XRange == other.XRange
            && YRange == other.YRange
            && ZRange == other.ZRange;
    }

    public static bool operator ==(Point3Range<TNumber> left, Point3Range<TNumber> right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(Point3Range<TNumber> left, Point3Range<TNumber> right)
    {
        return !(left == right);
    }
}

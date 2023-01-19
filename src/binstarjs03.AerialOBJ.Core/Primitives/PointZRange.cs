using System;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;

namespace binstarjs03.AerialOBJ.Core.Primitives;
public struct PointZRange<TNumber> : IEquatable<PointZRange<TNumber>> where TNumber : struct, INumber<TNumber>
{
    public Rangeof<TNumber> XRange { get; set; }
    public Rangeof<TNumber> ZRange { get; set; }
    public TNumber Sum => XRange.Sum * ZRange.Sum;

    public PointZRange()
    {
        XRange = new();
        ZRange = new();
    }

    public PointZRange(Rangeof<TNumber> xRange, Rangeof<TNumber> zRange)
    {
        XRange = xRange;
        ZRange = zRange;
    }

    public PointZRange(PointZ<TNumber> min, PointZ<TNumber> max)
    {
        XRange = new Rangeof<TNumber>(min.X, max.X);
        ZRange = new Rangeof<TNumber>(min.Z, max.Z);
    }

    public PointZRange(TNumber minX, TNumber maxX, TNumber minZ, TNumber maxZ)
    {
        XRange = new Rangeof<TNumber>(minX, maxX);
        ZRange = new Rangeof<TNumber>(minZ, maxZ);
    }

    public bool IsInside(PointZ<TNumber> point)
    {
        bool inX = XRange.IsInside(point.X);
        bool inZ = ZRange.IsInside(point.Z);
        return inX && inZ;
    }

    public bool IsOutside(PointZ<TNumber> point)
    {
        return !IsInside(point);
    }

    public void ThrowIfOutside(PointZ<TNumber> point)
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
        if (obj is PointZRange<TNumber> o)
            return Equals(o);
        return false;
    }

    public bool Equals(PointZRange<TNumber> other)
    {
        return XRange == other.XRange
            && ZRange == other.ZRange;
    }

    public static bool operator ==(PointZRange<TNumber> left, PointZRange<TNumber> right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(PointZRange<TNumber> left, PointZRange<TNumber> right)
    {
        return !(left == right);
    }
}

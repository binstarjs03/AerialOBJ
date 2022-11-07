/*
Copyright (c) 2022, Bintang Jakasurya
All rights reserved. 

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/

using System;

namespace binstarjs03.AerialOBJ.Core.CoordinateSystem;

/// <summary>
/// Represent range of 2-Dimensional Cartesian Coordinate X and Z integer pair 
/// of Minecraft object (may be Region, Chunk, Block, etc) in 
/// </summary>
public struct CoordsRange2 : IEquatable<CoordsRange2>
{
    public Range XRange { get; set; }
    public Range ZRange { get; set; }
    public int Sum => XRange.Sum * ZRange.Sum;

    public CoordsRange2()
    {
        XRange = new Range();
        ZRange = new Range();
    }

    public CoordsRange2(Coords2 min, Coords2 max)
    {
        XRange = new Range(min.X, max.X);
        ZRange = new Range(min.Z, max.Z);
    }

    public CoordsRange2(Range xRange, Range zRange)
    {
        XRange = xRange;
        ZRange = zRange;
    }

    public CoordsRange2(int minX, int maxX, int minZ, int maxZ)
    {
        XRange = new Range(minX, maxX);
        ZRange = new Range(minZ, maxZ);
    }

    public bool IsInside(Coords2 other)
    {
        bool inX = XRange.IsInside(other.X);
        bool inZ = ZRange.IsInside(other.Z);
        return inX && inZ;
    }

    public bool IsOutside(Coords2 other)
    {
        return !IsInside(other);
    }

    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public void ThrowIfOutside(Coords2 other)
    {
        if (IsOutside(other))
            throw new ArgumentOutOfRangeException(nameof(other));
    }

    #region Object overrides

    public override string ToString()
    {
        return $"({XRange}, {ZRange})";
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(XRange, ZRange);
    }

    #endregion

    #region Equality

    public override bool Equals(object? obj)
    {
        if (obj is CoordsRange2 o)
            return Equals(o);
        else
            return false;
    }

    public bool Equals(CoordsRange2 other)
    {
        return XRange == other.XRange
            && ZRange == other.ZRange;
    }

    public static bool operator ==(CoordsRange2 left, CoordsRange2 right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(CoordsRange2 left, CoordsRange2 right)
    {
        return !(left == right);
    }

    #endregion
}

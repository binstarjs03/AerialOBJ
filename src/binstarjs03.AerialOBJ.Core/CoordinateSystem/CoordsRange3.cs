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
/// Represent range of 3-Dimensional Cartesian Coordinate X, Y and Z integer
/// of Minecraft object (may be Region, Chunk, Block, etc) in 
/// </summary>
public struct CoordsRange3 : IEquatable<CoordsRange3>
{
    public Range XRange { get; set; }
    public Range YRange { get; set; }
    public Range ZRange { get; set; }
    public int Sum => XRange.Sum * YRange.Sum * ZRange.Sum;

    public CoordsRange3()
    {
        XRange = new Range();
        YRange = new Range();
        ZRange = new Range();
    }

    public CoordsRange3(Coords3 min, Coords3 max)
    {
        XRange = new Range(min.X, max.X);
        YRange = new Range(min.Y, max.Y);
        ZRange = new Range(min.Z, max.Z);
    }

    public CoordsRange3(Range xRange, Range yRange, Range zRange)
    {
        XRange = xRange;
        YRange = yRange;
        ZRange = zRange;
    }

    public CoordsRange3(int minX, int maxX, int minY, int maxY, int minZ, int maxZ)
    {
        XRange = new Range(minX, maxX);
        YRange = new Range(minY, maxY);
        ZRange = new Range(minZ, maxZ);
    }

    public bool IsInside(Coords3 other)
    {
        bool inX = XRange.IsInside(other.X);
        bool inY = YRange.IsInside(other.Y);
        bool inZ = ZRange.IsInside(other.Z);
        return inX && inY && inZ;
    }

    public bool IsOutside(Coords3 other)
    {
        return !IsInside(other);
    }

    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public void ThrowIfOutside(Coords3 other)
    {

        if (!IsOutside(other))
            throw new ArgumentOutOfRangeException(nameof(other));
    }

    #region Object overrides

    public override string ToString()
    {
        return $"({XRange}, {ZRange})";
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(XRange, YRange, ZRange);
    }

    #endregion

    #region Equality

    public override bool Equals(object? obj)
    {
        if (obj is CoordsRange3 o)
            return Equals(o);
        else
            return false;
    }

    public bool Equals(CoordsRange3 other)
    {
        return XRange == other.XRange
            && YRange == other.YRange
            && ZRange == other.ZRange;
    }

    public static bool operator ==(CoordsRange3 left, CoordsRange3 right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(CoordsRange3 left, CoordsRange3 right)
    {
        return !(left == right);
    }

    #endregion
}

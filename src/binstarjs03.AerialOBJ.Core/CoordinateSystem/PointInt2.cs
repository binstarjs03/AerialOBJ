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
/// Represent 2-Dimensional Cartesian Coordinate X and Y integer
/// </summary>
public struct PointInt2
{
    public static PointInt2 Zero => new(0, 0);
    public int X { get; set; }
    public int Y { get; set; }

    public PointInt2()
    {
        X = 0;
        Y = 0;
    }

    public PointInt2(int x, int y)
    {
        X = x;
        Y = y;
    }

    #region Object overrides

    public override string ToString()
    {
        return $"({X}, {Y})";
    }

    /// <summary>
    /// Float equality is unreliable, beware
    /// </summary>
    public override bool Equals(object? obj)
    {
        if (obj is PointInt2 p)
            return X == p.X && Y == p.Y;
        else
            return false;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(X, Y);
    }

    #endregion

    #region Equality Operators

    public static bool operator ==(PointInt2 left, PointInt2 right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(PointInt2 left, PointInt2 right)
    {
        return !(left == right);
    }

    #endregion

    #region Casters

    public static explicit operator PointInt2(Coords2 coords2)
    {
        return new PointInt2(coords2.X, coords2.Z);
    }

    public static explicit operator PointInt2(Coords3 coords3)
    {
        return new PointInt2(coords3.X, coords3.Z);
    }

    public static explicit operator PointInt2(PointF2 point2)
    {
        return new PointInt2(MathUtils.Floor(point2.X), MathUtils.Floor(point2.Y));
    }

    #endregion

    #region Operator Overloads

    public static PointInt2 operator *(PointInt2 pointInt2, int num)
    {
        return new PointInt2(pointInt2.X * num, pointInt2.Y * num);
    }

    #endregion
}


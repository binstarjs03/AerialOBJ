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
/// Represent 2-Dimensional Cartesian Coordinate X and Y double-precision 
/// float
/// </summary>
public struct PointF2
{
    public static PointF2 Zero => new(0, 0);
    public double X;
    public double Y;
    public PointF2 Floor => new(Math.Floor(X), Math.Floor(Y));

    public PointF2()
    {
        X = 0;
        Y = 0;
    }

    public PointF2(double x, double y)
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
    /// Formatted string, rounded to 2 digits
    /// </summary>
    public string ToStringRounded()
    {
        return $"({Math.Round(X, 2)}, {Math.Round(Y, 2)})";
    }

    /// <summary>
    /// Float equality is unreliable, beware
    /// </summary>
    public override bool Equals(object? obj)
    {
        if (obj is PointF2 p)
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

    public static bool operator ==(PointF2 left, PointF2 right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(PointF2 left, PointF2 right)
    {
        return !(left == right);
    }

    #endregion



    #region Casters

    public static explicit operator PointF2(Coords2 coords2)
    {
        return new PointF2(coords2.X, coords2.Z);
    }

    public static explicit operator PointF2(Coords3 coords3)
    {
        return new PointF2(coords3.X, coords3.Z);
    }

    public static implicit operator PointF2(PointInt2 pointInt2)
    {
        return new PointF2(pointInt2.X, pointInt2.Y);
    }

    #endregion



    // TODO we may need to add more overloads if we wish to reuse this core
    // library across other project
    #region Operator Overloads

    // Unary

    public static PointF2 operator -(PointF2 self)
    {
        return new PointF2(-self.X, -self.Y);
    }

    // Binary - Numeric

    public static PointF2 operator -(PointF2 left, double right)
    {
        return new PointF2(left.X - right, left.Y - right);
    }

    public static PointF2 operator *(PointF2 pointF2, double right)
    {
        return new PointF2(pointF2.X * right, pointF2.Y * right);
    }

    public static PointF2 operator /(PointF2 pointF2, double right)
    {
        return new PointF2(pointF2.X / right, pointF2.Y / right);
    }

    // Binary - PointF2

    public static PointF2 operator +(PointF2 left, PointF2 right)
    {
        return new PointF2(left.X + right.X, left.Y + right.Y);
    }

    public static PointF2 operator -(PointF2 left, PointF2 right)
    {
        return new PointF2(left.X - right.X, left.Y - right.Y);
    }

    // Binary - PointInt2

    public static PointF2 operator +(PointF2 left, PointInt2 right)
    {
        return new PointF2(left.X + right.X, left.Y + right.Y);
    }

    #endregion
}

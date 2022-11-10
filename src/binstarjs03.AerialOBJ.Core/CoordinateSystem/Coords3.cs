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
/// Represent 3-Dimensional Cartesian Coordinate X, Y and Z integers position 
/// of Minecraft object (may be Region, Chunk, Block, etc)
/// </summary>
public struct Coords3 : IEquatable<Coords3>
{
    public static Coords3 Zero => new(0, 0, 0);
    public int X { get; set; }
    public int Y { get; set; }
    public int Z { get; set; }

    public Coords3()
    {
        X = 0;
        Y = 0;
        Z = 0;
    }

    public Coords3(int x, int y, int z)
    {
        X = x;
        Y = y;
        Z = z;
    }

    #region Object overrides 

    public override string ToString()
    {
        return $"({X}, {Y}, {Z})";
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(X, Y, Z);
    }

    #endregion

    #region Equality Operators 

    public override bool Equals(object? obj)
    {
        if (obj is Coords3 o)
            return Equals(o);
        else
            return false;
    }

    public bool Equals(Coords3 other)
    {
        return X == other.X
            && Y == other.Y
            && Z == other.Z;
    }

    public static bool operator ==(Coords3 left, Coords3 right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(Coords3 left, Coords3 right)
    {
        return !(left == right);
    }

    #endregion

    #region Casters 

    public static implicit operator Coords3(Coords2 coords2)
    {
        return new Coords3(coords2.X, 0, coords2.Z);
    }

    public static explicit operator Coords3(PointInt2 pointInt2)
    {
        return new Coords3(pointInt2.X, 0, pointInt2.Y);
    }

    public static explicit operator Coords3(PointF2 pointf2)
    {
        return new Coords3((int)pointf2.X, 0, (int)pointf2.Y);
    }

    #endregion
}

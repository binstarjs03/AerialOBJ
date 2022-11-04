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

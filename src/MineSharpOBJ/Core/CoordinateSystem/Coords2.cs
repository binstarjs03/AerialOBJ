using System;

namespace binstarjs03.MineSharpOBJ.Core.CoordinateSystem;

/// <summary>
/// Represent 2-Dimensional X and Z integer pair position of Minecraft object 
/// (may be Region, Chunk, Block, etc) in Cartesian Coordinate
/// </summary>
public struct Coords2
{
    public Coords2()
    {
        X = 0;
        Z = 0;
    }

    public Coords2(int x, int z)
    {
        X = x;
        Z = z;
    }

    public int X { get; set; }
    public int Z { get; set; }
    
    public static Coords2 Zero => new(0, 0);



    #region Object overrides

    public override string ToString()
    {
        return $"c2({X}, {Z})";
    }

    public override bool Equals(object? obj)
    {
        if (obj is Coords2 p)
            return X == p.X && Z == p.Z;
        else
            return false;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(X, Z);
    }

    #endregion



    #region Equality Operators

    public static bool operator ==(Coords2 left, Coords2 right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(Coords2 left, Coords2 right)
    {
        return !(left == right);
    }

    #endregion



    #region Casters

    public static explicit operator Coords2(Coords3 coords3)
    {
        return new Coords2(coords3.X, coords3.Z);
    }

    public static explicit operator Coords2(PointInt2 pointInt2)
    {
        return new Coords2(pointInt2.X, pointInt2.Y);
    }

    public static explicit operator Coords2(PointF2 pointf2)
    {
        return new Coords2((int)pointf2.X, (int)pointf2.Y);
    }

    #endregion
}

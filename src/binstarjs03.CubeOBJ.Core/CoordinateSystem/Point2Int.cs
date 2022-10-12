using System;

namespace binstarjs03.CubeOBJ.Core.CoordinateSystem;

/// <summary>
/// Represent 2-Dimensional Cartesian Coordinate X and Y integer
/// </summary>
public struct PointInt2
{
    public int X;
    public int Y;

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

    public static PointInt2 Zero => new(0, 0);



    #region Object overrides

    public override string ToString()
    {
        return $"pi2({X}, {Y})";
    }

    /// <summary>
    /// Formatted string version without pi2 prefix, rounded to 2 digits
    /// </summary>
    public string ToStringAnotherFormat()
    {
        return $"({MathF.Round(X, 2)}, {MathF.Round(Y, 2)})";
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
        return new PointInt2((int)point2.X, (int)point2.Y);
    }

    #endregion



    #region Operator Overloads

    public static PointInt2 operator *(PointInt2 pointInt2, int num)
    {
        return new PointInt2(pointInt2.X * num, pointInt2.Y * num);
    }

    #endregion
}


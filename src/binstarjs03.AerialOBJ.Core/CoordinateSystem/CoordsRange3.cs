using System;

namespace binstarjs03.AerialOBJ.Core.CoordinateSystem;

/// <summary>
/// Represent range of 3-Dimensional Cartesian Coordinate X, Y and Z integer
/// of Minecraft object (may be Region, Chunk, Block, etc) in 
/// </summary>
public struct CoordsRange3
{
    public Range XRange;
    public Range YRange;
    public Range ZRange;

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



    #region Object overrides

    public override string ToString()
    {
        return $"cr2({XRange}, {ZRange})";
    }

    public override bool Equals(object? obj)
    {
        if (obj is CoordsRange3 pr)
            return XRange == pr.XRange && YRange == pr.YRange && ZRange == pr.ZRange;
        else
            return false;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(XRange, YRange, ZRange);
    }

    #endregion



    #region Equality Operators

    public static bool operator ==(CoordsRange3 left, CoordsRange3 right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(CoordsRange3 left, CoordsRange3 right)
    {
        return !(left == right);
    }

    #endregion



    #region Methods

    public bool IsInside(Coords3 other)
    {
        bool inX = XRange.IsInside(other.X);
        bool inY = YRange.IsInside(other.Y);
        bool inZ = ZRange.IsInside(other.Z);
        return inX && inY && inZ;
    }

    private bool IsInside(Coords3 other, out string message)
    {
        bool inX = XRange.IsInside(other.X);
        bool inY = YRange.IsInside(other.Y);
        bool inZ = ZRange.IsInside(other.Z);
        string outMsgX = $"X:{other.X} is outside range {XRange}";
        string outMsgY = $"Y:{other.Y} is outside range {XRange}";
        string outMsgZ = $"Z:{other.Z} is outside range {ZRange}";
        message = "";
        if (!inX)
            message += outMsgX + '\n';
        if (!inY)
            message += outMsgY + '\n';
        if (!inZ)
            message += outMsgZ + '\n';
        return inX && inY && inZ;
    }

    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public void ThrowIfOutside(Coords3 other, bool displayMessage = false)
    {
        if (displayMessage)
        {
            if (!IsInside(other, out string message))
                throw new ArgumentOutOfRangeException(nameof(other), message);
        }
        else
        {
            if (!IsInside(other))
                throw new ArgumentOutOfRangeException();
        }
    }

    #endregion
}

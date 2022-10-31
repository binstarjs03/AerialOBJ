using System;

namespace binstarjs03.AerialOBJ.Core.CoordinateSystem;

/// <summary>
/// Represent range of 2-Dimensional Cartesian Coordinate X and Z integer pair 
/// of Minecraft object (may be Region, Chunk, Block, etc) in 
/// </summary>
public struct CoordsRange2
{
    public Range XRange;
    public Range ZRange;

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

    public static CoordsRange2 Zero => new(0, 0, 0, 0);
    public int Sum => (XRange.Max - XRange.Min + 1)
                    * (ZRange.Max - ZRange.Min + 1);

    #region Methods

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

    private bool IsInside(Coords2 other, out string message)
    {
        bool inX = XRange.IsInside(other.X);
        bool inZ = ZRange.IsInside(other.Z);
        string outMsgX = $"X:{other.X} is outside range {XRange}";
        string outMsgZ = $"Z:{other.Z} is outside range {ZRange}";
        message = "";
        if (!inX)
            message += outMsgX + '\n';
        if (!inZ)
            message += outMsgZ + '\n';
        return inX || inZ;
    }

    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public void ThrowIfOutside(Coords2 other, bool displayMessage = false)
    {
        if (displayMessage)
        {
            if (!IsInside(other, out string message))
                throw new ArgumentOutOfRangeException(nameof(other), message);
        }
        else
        {
            if (!IsInside(other))
                throw new ArgumentOutOfRangeException(nameof(other));
        }
    }

    #endregion

    #region Equality Operators

    public static bool operator ==(CoordsRange2 left, CoordsRange2 right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(CoordsRange2 left, CoordsRange2 right)
    {
        return !(left == right);
    }

    #endregion

    #region Object overrides

    public override string ToString()
    {
        return $"cr2({XRange}, {ZRange})";
    }

    public override bool Equals(object? obj)
    {
        if (obj is CoordsRange2 pr)
            return XRange == pr.XRange && ZRange == pr.ZRange;
        else
            return false;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(XRange, ZRange);
    }

    #endregion
}

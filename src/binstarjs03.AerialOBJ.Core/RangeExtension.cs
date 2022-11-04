using System;

using binstarjs03.AerialOBJ.Core.CoordinateSystem;

namespace binstarjs03.AerialOBJ.Core;

public static class RangeExtension
{
    public enum Direction
    {
        NorthSouth,
        WestEast,
        Horizontal,
        Vertical,
        All
    }

    private static void CheckDistance(int distance, bool expanding)
    {
        if (distance < 0 && expanding || distance > 0 && !expanding)
            throw new ArgumentOutOfRangeException(nameof(distance), "Distance cannot be negative");
    }

    private static Range ExpandContract(this Range range, int distance, bool expanding)
    {
        CheckDistance(distance, expanding);
        try
        {
            range.Min -= distance;
            range.Max += distance;
            return range;
        }
        catch (InvalidRangeException e)
        {
            string msg = "Cannot contract any futher, either the range is already infinitely small " 
                       + "or distance is too big to contract any further";
            throw new InvalidRangeException(msg, e);
        }
    }

    public static Range Expand(this Range range, int distance)
    {
        return ExpandContract(range, distance, true);
    }

    public static Range Contract(this Range range, int distance)
    {
        return ExpandContract(range, -distance, false);
    }

    private static CoordsRange2 ExpandContract(this CoordsRange2 coordsRange, int distance, bool positiveDistance, Direction direction)
    {
        CheckDistance(distance, positiveDistance);
        if (direction == Direction.Vertical)
            throw new ArgumentException($"{nameof(CoordsRange2)} is two dimensional horizontal plane so it doesn't have vertical axis");
        switch (direction)
        {
            case Direction.NorthSouth:
                coordsRange.ZRange.Expand(distance);
                break;
            case Direction.WestEast:
                coordsRange.XRange.Expand(distance);
                break;
            case Direction.Horizontal:
            case Direction.All:
                coordsRange.ZRange.Expand(distance);
                coordsRange.XRange.Expand(distance);
                break;
        }
        return coordsRange;
    }

    public static CoordsRange2 Expand(this CoordsRange2 coordsRange, int distance, Direction direction)
    {
        return ExpandContract(coordsRange, distance, true, direction);
    }

    public static CoordsRange2 Contract(this CoordsRange2 coordsRange, int distance, Direction direction)
    {
        return ExpandContract(coordsRange, -distance, false, direction);
    }

    private static CoordsRange3 ExpandContract(this CoordsRange3 coordsRange, int distance, bool expanding, Direction direction)
    {
        CheckDistance(distance, expanding);
        switch (direction)
        {
            case Direction.NorthSouth:
                coordsRange.ZRange.Expand(distance);
                break;
            case Direction.WestEast:
                coordsRange.XRange.Expand(distance);
                break;
            case Direction.Horizontal:
                coordsRange.XRange.Expand(distance);
                coordsRange.ZRange.Expand(distance);
                break;
            case Direction.Vertical:
                coordsRange.YRange.Expand(distance);
                break;
            case Direction.All:
                coordsRange.XRange.Expand(distance);
                coordsRange.YRange.Expand(distance);
                coordsRange.ZRange.Expand(distance);
                break;
        }
        return coordsRange;
    }

    public static CoordsRange3 Expand(this CoordsRange3 coordsRange, int distance, Direction direction)
    {
        return ExpandContract(coordsRange, distance, true, direction);
    }

    public static CoordsRange3 Contract(this CoordsRange3 coordsRange, int distance, Direction direction)
    {
        return ExpandContract(coordsRange, -distance, false, direction);
    }
}

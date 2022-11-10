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

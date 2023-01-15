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
using System.Numerics;

namespace binstarjs03.AerialOBJ.Core.Primitives;

// TODO bug in the implementation, remember that a struct is a value-type
// so arguments passed in are copy of struct, and we just modifying the COPY OF IT!!!
public static class RangeofExtension
{
    public enum Direction
    {
        NorthSouth,
        WestEast,
        Horizontal,
        Vertical,
        All
    }

    private static void CheckDistance<TNumber>(TNumber distance, bool expanding) where TNumber : struct, INumber<TNumber>
    {
        if (distance < TNumber.Zero && expanding || distance > TNumber.Zero && !expanding)
            throw new ArgumentOutOfRangeException(nameof(distance), "Distance cannot be negative");
    }

    private static Rangeof<TNumber> ExpandContract<TNumber>(this Rangeof<TNumber> range, TNumber distance, bool expanding) where TNumber : struct, INumber<TNumber>
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

    public static Rangeof<TNumber> Expand<TNumber>(this Rangeof<TNumber> range, TNumber distance) where TNumber : struct, INumber<TNumber>
    {
        return ExpandContract(range, distance, true);
    }

    public static Rangeof<TNumber> Contract<TNumber>(this Rangeof<TNumber> range, TNumber distance) where TNumber : struct, INumber<TNumber>
    {
        return ExpandContract(range, -distance, false);
    }

    private static Point2ZRange<TNumber> ExpandContract<TNumber>(this Point2ZRange<TNumber> coordsRange, TNumber distance, bool positiveDistance, Direction direction) where TNumber : struct, INumber<TNumber>
    {
        CheckDistance(distance, positiveDistance);
        if (direction == Direction.Vertical)
            throw new ArgumentException($"{nameof(Point2ZRange<TNumber>)} is two dimensional horizontal plane so it doesn't have vertical axis");
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

    public static Point2ZRange<TNumber> Expand<TNumber>(this Point2ZRange<TNumber> coordsRange, TNumber distance, Direction direction) where TNumber : struct, INumber<TNumber>
    {
        return ExpandContract(coordsRange, distance, true, direction);
    }

    public static Point2ZRange<TNumber> Contract<TNumber>(this Point2ZRange<TNumber> coordsRange, TNumber distance, Direction direction) where TNumber : struct, INumber<TNumber>
    {
        return ExpandContract(coordsRange, -distance, false, direction);
    }

    private static Point3Range<TNumber> ExpandContract<TNumber>(this Point3Range<TNumber> coordsRange, TNumber distance, bool expanding, Direction direction) where TNumber : struct, INumber<TNumber>
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

    public static Point3Range<TNumber> Expand<TNumber>(this Point3Range<TNumber> coordsRange, TNumber distance, Direction direction) where TNumber : struct, INumber<TNumber>
    {
        return ExpandContract(coordsRange, distance, true, direction);
    }

    public static Point3Range<TNumber> Contract<TNumber>(this Point3Range<TNumber> coordsRange, TNumber distance, Direction direction) where TNumber : struct, INumber<TNumber>
    {
        return ExpandContract(coordsRange, -distance, false, direction);
    }

    public static bool CanIntersect<TNumber>(this Rangeof<TNumber> self, Rangeof<TNumber> other) where TNumber : struct, INumber<TNumber>
    {
        return self.Max > other.Min || self.Max > other.Min;
    }

    public static bool CanIntersect<TNumber>(this Point2ZRange<TNumber> self, Point2ZRange<TNumber> other) where TNumber : struct, INumber<TNumber>
    {
        return self.XRange.CanIntersect(other.XRange)
            && self.ZRange.CanIntersect(other.ZRange);
    }

    public static Rangeof<TNumber> Intersect<TNumber>(this Rangeof<TNumber> self, Rangeof<TNumber> other) where TNumber : struct, INumber<TNumber>
    {
        TNumber min = selectMin(self.Min, other.Min);
        TNumber max = selectMax(self.Max, other.Max);
        return new Rangeof<TNumber>(min, max);
        TNumber selectMin(TNumber minA, TNumber minB) => minA > minB ? minA : minB;
        TNumber selectMax(TNumber maxA, TNumber maxB) => maxA < maxB ? maxA : maxB;
    }

    public static Point2ZRange<TNumber> Intersect<TNumber>(this Point2ZRange<TNumber> self, Point2ZRange<TNumber> other) where TNumber : struct, INumber<TNumber>
    {
        Rangeof<TNumber> xRange = self.XRange.Intersect(other.XRange);
        Rangeof<TNumber> zRange = self.ZRange.Intersect(other.ZRange);
        return new Point2ZRange<TNumber>(xRange, zRange);
    }
}

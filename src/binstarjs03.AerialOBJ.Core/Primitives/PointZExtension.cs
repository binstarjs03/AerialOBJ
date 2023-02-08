using System.Numerics;

namespace binstarjs03.AerialOBJ.Core.Primitives;
public static class PointZExtension
{
    public static PointZ<TNumber> MoveNorth<TNumber>(this PointZ<TNumber> point, TNumber? distance = null) where TNumber : struct, INumber<TNumber>
    {
        distance ??= TNumber.One;
        point.Z -= distance.Value;
        return point;
    }

    public static PointZ<TNumber> MoveSouth<TNumber>(this PointZ<TNumber> point, TNumber? distance = null) where TNumber : struct, INumber<TNumber>
    {
        distance ??= TNumber.One;
        point.Z += distance.Value;
        return point;
    }

    public static PointZ<TNumber> MoveWest<TNumber>(this PointZ<TNumber> point, TNumber? distance = null) where TNumber : struct, INumber<TNumber>
    {
        distance ??= TNumber.One;
        point.X -= distance.Value;
        return point;
    }

    public static PointZ<TNumber> MoveEast<TNumber>(this PointZ<TNumber> point, TNumber? distance = null) where TNumber : struct, INumber<TNumber>
    {
        distance ??= TNumber.One;
        point.X += distance.Value;
        return point;
    }
}

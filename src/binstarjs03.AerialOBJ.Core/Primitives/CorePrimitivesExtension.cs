namespace binstarjs03.AerialOBJ.Core.Primitives;
public static class PrimitivesExtension
{
    public static PointY<float> Floor(this PointY<float> point)
    {
        return new PointY<float>(point.X.Floor(), point.Y.Floor());
    }

    public static PointZ<int> Floor(this PointZ<float> point)
    {
        return new PointZ<int>(point.X.Floor(), point.Z.Floor());
    }

    public static PointY<float> ToFloat(this PointY<int> point)
    {
        return new PointY<float>(point.X, point.Y);
    }

    public static Size<float> ToFloat(this Size<int> size)
    {
        return new Size<float>(size.Width, size.Height);
    }
}

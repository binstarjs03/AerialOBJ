namespace binstarjs03.AerialOBJ.Core.Primitives;
public static class ColorUtils
{
    /// <summary>
    /// Blend two colors, ignoring alpha (will return opaque color)
    /// </summary>
    /// <exception cref="InvalidRangeException"></exception>
    public static Color SimpleBlend(Color a, Color b, float ratio)
    {
        if (ratio < 0f || ratio > 1f)
            throw new InvalidRangeException();
        return new Color()
        {
            Alpha = byte.MaxValue,
            Red = (byte)(a.Red * (1 - ratio) + b.Red * ratio),
            Green = (byte)(a.Green * (1 - ratio) + b.Green * ratio),
            Blue = (byte)(a.Blue * (1 - ratio) + b.Blue * ratio),
        };
    }

    public static Color SimpleBlend(Color a, Color b, byte ratio)
    {
        float byteMaxRange = 256f;
        return SimpleBlend(a, b, ratio / byteMaxRange);
    }
}

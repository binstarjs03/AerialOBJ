using System;

using binstarjs03.AerialOBJ.Core.Primitives;

namespace binstarjs03.AerialOBJ.Core;
public static class RandomExtension
{
    public static Color NextColor(this Random random)
    {
        return new Color
        {
            Alpha = (byte)random.Next(256),
            Red = (byte)random.Next(256),
            Green = (byte)random.Next(256),
            Blue = (byte)random.Next(256),
        };
    }

    public static Color NextColor(this Random random, Color color, byte distance)
    {
        return new Color
        {
            Alpha = (byte)int.Clamp(color.Alpha + random.Next(distance), byte.MinValue, byte.MaxValue),
            Red = (byte)int.Clamp(color.Red + random.Next(distance), byte.MinValue, byte.MaxValue),
            Green = (byte)int.Clamp(color.Green + random.Next(distance), byte.MinValue, byte.MaxValue),
            Blue = (byte)int.Clamp(color.Blue + random.Next(distance), byte.MinValue, byte.MaxValue),
        };
    }
}

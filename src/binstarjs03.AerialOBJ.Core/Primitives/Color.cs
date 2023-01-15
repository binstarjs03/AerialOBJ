using System;
using System.Globalization;

namespace binstarjs03.AerialOBJ.Core.Primitives;
public struct Color
{
    public required byte Red { get; set; }
    public required byte Green { get; set; }
    public required byte Blue { get; set; }
    public required byte Alpha { get; set; }

    public static Color Parse(string hexColor, byte alpha)
    {
        if (!hexColor.StartsWith("#"))
            throw new FormatException($"Argument {nameof(hexColor)} must start with pound (#) sign");
        if (hexColor.Length != 7)
            throw new FormatException($"Argument {nameof(hexColor)} length must be equal to 7");
        try
        {
            return new Color()
            {
                Red = byte.Parse(hexColor[1..3], NumberStyles.HexNumber),
                Green = byte.Parse(hexColor[3..5], NumberStyles.HexNumber),
                Blue = byte.Parse(hexColor[5..7], NumberStyles.HexNumber),
                Alpha = alpha
            };
        }
        catch (FormatException e)
        {
            throw new FormatException($"String format for argument {nameof(hexColor)} must be \"#RRGGBB\", " +
                                      $"where each digits represent hexadecimal value", e);
        }
    }

    public override string ToString()
    {
        return $"A:{Alpha}, R:{Red}, G:{Green}, B:{Blue}";
    }
}
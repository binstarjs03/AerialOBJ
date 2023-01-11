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
        return new Color()
        {
            Red = byte.Parse(hexColor[1..3], NumberStyles.HexNumber),
            Green = byte.Parse(hexColor[3..5], NumberStyles.HexNumber),
            Blue = byte.Parse(hexColor[5..7], NumberStyles.HexNumber),
            Alpha = alpha
        };
    }

    public override string ToString()
    {
        return $"A:{Alpha}, R:{Red}, G:{Green}, B:{Blue}";
    }
}
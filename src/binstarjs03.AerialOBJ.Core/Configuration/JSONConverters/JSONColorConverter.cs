using System;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

using binstarjs03.AerialOBJ.Core.Primitives;


namespace binstarjs03.AerialOBJ.Core.Configuration.JSONConverters;
public class JSONColorConverter : JsonConverter<Color>
{
    public override Color Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.String)
            throw new JsonException("Token is not a string value");
        string stringvalue = reader.GetString()!;
        if (!stringvalue.StartsWith("#"))
            throw new JsonException("Missing pound (#) sign");
        if (stringvalue.Length != 9)
            throw new JsonException("Length is not equal to 9, \"#AARRGGBB\" format expected");
        try
        {
            return new Color()
            {
                Alpha = byte.Parse(stringvalue[1..3], NumberStyles.HexNumber),
                Red = byte.Parse(stringvalue[3..5], NumberStyles.HexNumber),
                Green = byte.Parse(stringvalue[5..7], NumberStyles.HexNumber),
                Blue = byte.Parse(stringvalue[7..9], NumberStyles.HexNumber),
            };
        }
        catch (FormatException e)
        {
            throw new JsonException("Invalid format. Expected \"#AARRGGBB\" " +
                "format where each digits represent base 16 number (hexadecimal)", e);
        }
    }

    public override void Write(Utf8JsonWriter writer, Color value, JsonSerializerOptions options)
    {
        string serializedColor = $"#{value.Alpha:X2}{value.Red:X2}{value.Green:X2}{value.Blue:X2}";
        writer.WriteStringValue(serializedColor);
    }
}

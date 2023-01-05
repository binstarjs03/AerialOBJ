using System;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

using binstarjs03.AerialOBJ.Core.Primitives;


namespace binstarjs03.AerialOBJ.Core.Definitions.JSONConverters;
public class JSONColorConverter : JsonConverter<Color>
{
    public override Color Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.String)
            throw new JsonException("Token is not a string value");
        string stringvalue = reader.GetString()!;
        if (stringvalue.Length != 7)
            throw new JsonException("Length is not equal to 7, \"#RRGGBB\" format expected");
        if (!stringvalue.StartsWith("#"))
            throw new JsonException("Missing pound (#) sign");
        try
        {
            return new Color()
            {
                Alpha = 255,
                Red = byte.Parse(stringvalue[1..3], NumberStyles.HexNumber),
                Green = byte.Parse(stringvalue[3..5], NumberStyles.HexNumber),
                Blue = byte.Parse(stringvalue[5..7], NumberStyles.HexNumber),
            };
        }
        catch (FormatException e)
        {
            throw new JsonException("Invalid format. Expected \"#RRGGBB\" " +
                "format where each digits represent base 16 number (hexadecimal)", e);
        }
    }

    public override void Write(Utf8JsonWriter writer, Color value, JsonSerializerOptions options)
    {
        string serializedColor = $"#{value.Red:X2}{value.Green:X2}{value.Blue:X2}";
        writer.WriteStringValue(serializedColor);
    }
}

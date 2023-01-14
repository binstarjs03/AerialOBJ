using System;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

using binstarjs03.AerialOBJ.Core.Primitives;

namespace binstarjs03.AerialOBJ.Core.Definitions.JSONConverters;
public class JSONViewportBlockDefinitionConverter : JsonConverter<ViewportBlockDefinition>
{
    public override ViewportBlockDefinition? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        try
        {
            // advance to property name "Color"
            reader.Read();

            // advance to "Color" string value
            reader.Read();
            string colorStr = reader.GetString()!;

            // advance to property name "Alpha"
            reader.Read();

            // advance to "Alpha" number value
            reader.Read();
            byte alpha = reader.GetByte();

            // advance to property name "DisplayName"
            reader.Read();

            // advance to "DisplayName" string value
            reader.Read();
            string displayName = reader.GetString()!;

            // advance to end object
            reader.Read();

            return new ViewportBlockDefinition()
            {
                Color = new Color
                {
                    Alpha = alpha,
                    Red = byte.Parse(colorStr[1..3], NumberStyles.HexNumber),
                    Green = byte.Parse(colorStr[3..5], NumberStyles.HexNumber),
                    Blue = byte.Parse(colorStr[5..7], NumberStyles.HexNumber),
                },
                DisplayName = displayName,
            };
        }
        catch (Exception e)
        {
            throw new JsonException("Invalid JSON format", e);
        }
    }

    public override void Write(Utf8JsonWriter writer, ViewportBlockDefinition value, JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }
}

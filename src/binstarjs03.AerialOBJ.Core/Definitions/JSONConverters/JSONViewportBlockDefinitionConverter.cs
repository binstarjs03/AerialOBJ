using System;
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
            JsonDocument doc = JsonDocument.ParseValue(ref reader);
            JsonElement root = doc.RootElement;

            string colorStr = root.GetProperty("Color").GetString()!;
            byte alpha = root.GetProperty("Alpha").GetByte();
            string displayName = root.GetProperty("DisplayName").GetString()!;

            return new ViewportBlockDefinition()
            {
                Color = Color.Parse(colorStr, alpha),
                DisplayName = displayName,
            };
        }
        catch (Exception e)
        {
            throw new FormatException($"Invalid JSON format for {nameof(ViewportBlockDefinition)}", e);
        }
    }

    public override void Write(Utf8JsonWriter writer, ViewportBlockDefinition value, JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }
}

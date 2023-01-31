using System;
using System.Text.Json;
using System.Text.Json.Serialization;

using binstarjs03.AerialOBJ.Core.Primitives;

namespace binstarjs03.AerialOBJ.Core.Definitions;
public class ViewportBlockDefinitionConverter : JsonConverter<ViewportBlockDefinition>
{
    public override ViewportBlockDefinition? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        JsonDocument doc = JsonDocument.ParseValue(ref reader);
        JsonElement root = doc.RootElement;

        string colorStr = root.GetProperty(nameof(ViewportBlockDefinition.Color)).GetString()!;
        byte alpha = root.GetProperty(nameof(ViewportBlockDefinition.Color.Alpha)).GetByte();
        string displayName = root.GetProperty(nameof(ViewportBlockDefinition.DisplayName)).GetString()!;

        return new ViewportBlockDefinition()
        {
            Color = Color.Parse(colorStr, alpha),
            DisplayName = displayName,
        };
    }

    public override void Write(Utf8JsonWriter writer, ViewportBlockDefinition value, JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }
}

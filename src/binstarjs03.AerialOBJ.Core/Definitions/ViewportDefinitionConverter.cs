using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

using binstarjs03.AerialOBJ.Core.Primitives;

namespace binstarjs03.AerialOBJ.Core.Definitions;
public class ViewportDefinitionConverter : JsonConverter<ViewportDefinition>
{
    public override ViewportDefinition? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        using JsonDocument doc = JsonDocument.ParseValue(ref reader);
        JsonElement root = doc.RootElement;

        string name = root.GetProperty(nameof(ViewportDefinition.Name)).GetString()!;
        int formatVersion = root.GetProperty(nameof(ViewportDefinition.FormatVersion)).GetInt32();
        string mcVersion = root.GetProperty(nameof(ViewportDefinition.MinecraftVersion)).GetString()!;

        JsonElement missingBlockDefinitionE = root.GetProperty(nameof(ViewportDefinition.MissingBlockDefinition));
        ViewportBlockDefinition missingBlockDefinition = readMissingBlockDefinition(missingBlockDefinitionE);

        JsonElement blockDefinitionsE = root.GetProperty(nameof(ViewportDefinition.BlockDefinitions));
        Dictionary<string, ViewportBlockDefinition> blockDefinitions = JsonSerializer.Deserialize<Dictionary<string, ViewportBlockDefinition>>(blockDefinitionsE, options)!;

        return new ViewportDefinition()
        {
            Name = name,
            FormatVersion = formatVersion,
            MinecraftVersion = mcVersion,
            MissingBlockDefinition = missingBlockDefinition,
            BlockDefinitions = blockDefinitions
        };

        static ViewportBlockDefinition readMissingBlockDefinition(JsonElement missingBlockDefinitionE)
        {
            string hexColor = missingBlockDefinitionE.GetProperty(nameof(Color)).GetString()!;
            string displayName = missingBlockDefinitionE.GetProperty(nameof(ViewportBlockDefinition.DisplayName)).GetString()!;

            return new ViewportBlockDefinition
            {
                Namespace = "",
                Color = Color.Parse(hexColor, alpha: byte.MaxValue),
                DisplayName = displayName,
                IsSolid = true,
                IsExcluded = false,
            };
        }
    }

    public override void Write(Utf8JsonWriter writer, ViewportDefinition value, JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }
}

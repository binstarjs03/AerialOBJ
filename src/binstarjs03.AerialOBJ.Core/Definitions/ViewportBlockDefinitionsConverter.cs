using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

using binstarjs03.AerialOBJ.Core.Primitives;

namespace binstarjs03.AerialOBJ.Core.Definitions;

public class ViewportBlockDefinitionsConverter : JsonConverter<Dictionary<string, ViewportBlockDefinition>>
{
    public override Dictionary<string, ViewportBlockDefinition>? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        Dictionary<string, ViewportBlockDefinition> result = new();

        reader.Read(); // advance from start array to start object
        while (reader.TokenType != JsonTokenType.EndArray)
        {
            ViewportBlockDefinition vbd = DeserializeViewportBlockDefinition(ref reader);
            result.Add(vbd.Namespace, vbd);
            reader.Read(); // advance to start object, or end of array if exhausted
        }

        return result;

        static ViewportBlockDefinition DeserializeViewportBlockDefinition(ref Utf8JsonReader reader)
        {
            // check if we are in correct state
            if (reader.TokenType != JsonTokenType.StartObject)
                throw new InvalidOperationException("Expected at start object");

            JsonElement element = JsonElement.ParseValue(ref reader);
            string nameSpace = element.GetProperty(nameof(ViewportBlockDefinition.Namespace)).GetString()!;
            string hexColor = element.GetProperty(nameof(ViewportBlockDefinition.Color)).GetString()!;

            // set alpha to full (opaque) if omitted
            byte alpha;
            if (element.TryGetProperty(nameof(Color.Alpha), out JsonElement alphaE))
                alpha = alphaE.GetByte();
            else
                alpha = byte.MaxValue;

            // infer display name from namespace if omitted
            string displayName;
            if (element.TryGetProperty(nameof(ViewportBlockDefinition.DisplayName), out JsonElement displayNameE))
                displayName = displayNameE.GetString()!;
            else
                displayName = GetDisplayNameFromNamespace(nameSpace);

            // set solid to true if omitted
            bool isSolid;
            if (element.TryGetProperty(nameof(ViewportBlockDefinition.IsSolid), out JsonElement isSolidE))
                isSolid = isSolidE.GetBoolean();
            else
                isSolid = true;

            // set excluded to false if omitted
            bool isExcluded;
            if (element.TryGetProperty(nameof(ViewportBlockDefinition.IsExcluded), out JsonElement isExcludedE))
                isExcluded = isExcludedE.GetBoolean();
            else
                isExcluded = false;

            return new ViewportBlockDefinition()
            {
                Namespace = nameSpace,
                Color = Color.Parse(hexColor, alpha),
                DisplayName = displayName,
                IsSolid = isSolid,
                IsExcluded = isExcluded,
            };
        }

        static string GetDisplayNameFromNamespace(string nameSpace)
        {
            // skip namespace, such as "minecraft" and ":" delimiter,
            // then titlecase and replace underscore with space
            return nameSpace[(nameSpace.IndexOf(':') + 1)..]
                .Replace('_', ' ')
                .ToTitleCase();
        }
    }

    public override void Write(Utf8JsonWriter writer, Dictionary<string, ViewportBlockDefinition> value, JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }
}

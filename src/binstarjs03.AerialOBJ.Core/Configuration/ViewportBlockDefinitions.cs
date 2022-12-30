using System.Collections.Generic;
using System.Text.Json.Serialization;
using binstarjs03.AerialOBJ.Core.Configuration.JSONConverters;
using binstarjs03.AerialOBJ.Core.Primitives;

namespace binstarjs03.AerialOBJ.Core.Configuration;
public class ViewportBlockDefinitions
{
    public required string Name { get; set; }
    public required int FormatVersion { get; set; }
    public required int MinecraftVersion { get; set; }
    public required List<ViewportBlockDefinition> BlockDefinitions { get; set; }
}

public class ViewportBlockDefinition
{
    [JsonPropertyName("name")]
    public required string Name { get; set; }

    [JsonPropertyName("color")]
    [JsonConverter(typeof(JSONColorConverter))]
    public required Color Color { get; set; }
}

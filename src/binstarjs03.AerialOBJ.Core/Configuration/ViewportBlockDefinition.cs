using System.Text.Json.Serialization;

using binstarjs03.AerialOBJ.Core.Configuration.JSONConverters;
using binstarjs03.AerialOBJ.Core.Primitives;

namespace binstarjs03.AerialOBJ.Core.Configuration;

public class ViewportBlockDefinition
{
    public required string Name { get; set; }

    [JsonConverter(typeof(JSONColorConverter))]
    public required Color Color { get; set; }

    public override string ToString()
    {
        return $"Name : \"{Name}\", Color : ({Color})";
    }
}

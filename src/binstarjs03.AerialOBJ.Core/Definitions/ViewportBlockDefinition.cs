using System.Text.Json.Serialization;

using binstarjs03.AerialOBJ.Core.Definitions.JSONConverters;
using binstarjs03.AerialOBJ.Core.Primitives;

namespace binstarjs03.AerialOBJ.Core.Definitions;

[JsonConverter(typeof(JSONViewportBlockDefinitionConverter))]
public class ViewportBlockDefinition
{
    public required Color Color { get; set; }
    public required string DisplayName { get; set; }

    public override string ToString()
    {
        return Color.ToString();
    }
}

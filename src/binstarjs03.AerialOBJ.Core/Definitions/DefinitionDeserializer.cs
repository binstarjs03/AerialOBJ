using System;
using System.Collections.Generic;
using System.Text.Json;

namespace binstarjs03.AerialOBJ.Core.Definitions;
public class DefinitionDeserializer
{
    public static T Deserialize<T>(string input) where T : class, IRootDefinition
    {
        IRootDefinition? definition;

        // get the value of "kind" property
        JsonDocument doc = JsonDocument.Parse(input);
        string? kind;
        try
        {
            kind = doc.RootElement.GetProperty("Kind").GetString();
        }
        catch (KeyNotFoundException e)
        {
            throw new KindNotFoundException("\"Kind\" property is missing", e);
        }

        // after then, deserialize the entirety, select the data type
        // according to "kind" value
        if (kind == "Viewport Definition")
            definition = JsonSerializer.Deserialize<ViewportDefinition>(input);
        else
            throw new NotImplementedException();

        if (definition is T tDefinition)
            return tDefinition;
        else
            throw new InvalidCastException();
    }
}

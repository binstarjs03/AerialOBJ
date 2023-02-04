using System;
using System.Collections.Generic;
using System.Text.Json;

namespace binstarjs03.AerialOBJ.Core.Definitions;
public static class DefinitionDeserializer
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
            throw new KindNotFoundException("Missing \"Kind\" property", e);
        }

        // after then deserialize the entirety,
        // select the data type according to "kind" value
        JsonSerializerOptions options = new()
        {
            PropertyNamingPolicy= JsonNamingPolicy.CamelCase,
            Converters = {
                new ViewportDefinitionConverter(),
                new ViewportBlockDefinitionsConverter() 
            }
        };

        definition = kind switch
        {
            DefinitionKinds.Viewport => JsonSerializer.Deserialize<ViewportDefinition>(input, options),
            _ => throw new UnrecognizedDefinitionKindException($"\"{kind}\" is unrecognized definition kind")
        };

        if (definition is T tDefinition)
            return tDefinition;
        else
            throw new InvalidCastException();
    }
}

public static class DefinitionKinds
{
    public const string Viewport = "Viewport Definition";
}
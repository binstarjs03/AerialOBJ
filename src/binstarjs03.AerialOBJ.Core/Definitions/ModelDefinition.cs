namespace binstarjs03.AerialOBJ.Core.Definitions;
public class ModelDefinition : IRootDefinition
{
    static ModelDefinition()
    {
        DefaultDefinition = GetDefaultDefinition();
    }

    public static ModelDefinition DefaultDefinition { get; }

    public required string DisplayName { get; set; }
    public required int FormatVersion { get; set; }
    public required string MinecraftVersion { get; set; }
    public string? OriginalFilename { get; set; }
    public bool IsDefault { get; private set; }

    private static ModelDefinition GetDefaultDefinition()
    {
        string input = """
        {
            "DisplayName": "Default Model Definition",
            "Kind": "Model",
            "FormatVersion": 1,
            "MinecraftVersion": "1.18"
        }
        """;
        var result = DefinitionDeserializer.Deserialize<ModelDefinition>(input);
        result.IsDefault = true;
        return result;
    }
}

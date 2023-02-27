namespace binstarjs03.AerialOBJ.Core.Definitions;
public interface IRootDefinition
{
    string DisplayName { get; }
    int FormatVersion { get; }
    string MinecraftVersion { get; }
    string? OriginalFilename { get; set; }
    bool IsDefault { get; }
}

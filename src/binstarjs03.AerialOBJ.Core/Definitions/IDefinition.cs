namespace binstarjs03.AerialOBJ.Core.Definitions;
internal interface IDefinition
{
    string Name { get; }
    int FormatVersion { get; }
    string MinecraftVersion { get; }
}

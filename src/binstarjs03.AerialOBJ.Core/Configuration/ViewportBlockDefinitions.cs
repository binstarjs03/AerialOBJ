using System.Collections.Generic;
using System.Text.Json;

namespace binstarjs03.AerialOBJ.Core.Configuration;
public class ViewportBlockDefinitions
{
    public required string Name { get; set; }
    public required int FormatVersion { get; set; }
    public required string MinecraftVersion { get; set; }
    public required List<ViewportBlockDefinition> BlockDefinitions { get; set; }

    public static ViewportBlockDefinitions GetDefaultDefinitions()
    {
        string input = """
{
    "Name": "Default Definitions",
    "FormatVersion": 1,
    "MinecraftVersion": "1.18",
    "BlockDefinitions": [
        {
            "Name": "aerialobj:unknown",
            "Color": "#FFFF00CC"
        },
        {
            "Name": "minecraft:air",
            "Color": "#00000000"
        },
        {
            "Name": "minecraft:cave_air",
            "Color": "#00000000"
        },
        {
            "Name": "minecraft:dirt",
            "Color": "#FF6C4D36"
        },
        {
            "Name": "minecraft:coarse_dirt",
            "Color": "#FF4E3826"
        },
        {
            "Name": "minecraft:grass_block",
            "Color": "#FF5D923A"
        },
        {
            "Name": "minecraft:mycelium",
            "Color": "#FF595155"
        },
        {
            "Name": "minecraft:podzol",
            "Color": "#FF3B2913"
        },
        {
            "Name": "minecraft:water",
            "Color": "#FF22417F"
        },
        {
            "Name": "minecraft:lava",
            "Color": "#FFCC4600"
        }
    ]
}
""";
        return JsonSerializer.Deserialize<ViewportBlockDefinitions>(input)!;
    }
}

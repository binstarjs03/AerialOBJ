using System.Collections.Generic;
using System.Text.Json;

namespace binstarjs03.AerialOBJ.Core.Definitions;
public class ViewportDefinition : IDefinition
{
    public required string Name { get; set; }
    public required int FormatVersion { get; set; }
    public required string MinecraftVersion { get; set; }
    public required ViewportBlockDefinition MissingBlockDefinition { get; set; }
    public required Dictionary<string, ViewportBlockDefinition> BlockDefinitions { get; set; }

    public static ViewportDefinition GetDefaultDefinition()
    {
        string input = """
{
    "Name": "Default Definitions",
    "FormatVersion": 1,
    "MinecraftVersion": "1.18",
	"MissingBlockDefinition" : {
		"Color" : "#FFFF00CC"
	},
    "BlockDefinitions": 
	{
        "minecraft:air" : {
            "Color" : "#00000000"
        },
		"minecraft:cave_air" : {
            "Color" : "#00000000"
        },
		"minecraft:dirt" : {
            "Color" : "#FF6C4D36"
        },
		"minecraft:coarse_dirt" : {
            "Color" : "#FF4E3826"
        },
		"minecraft:grass_block" : {
            "Color" : "#FF5D923A"
        },
		"minecraft:mycelium" : {
            "Color" : "#FF595155"
        },
		"minecraft:podzol" : {
            "Color" : "#FF3B2913"
        },
		"minecraft:water" : {
            "Color" : "#FF22417F"
        },
		"minecraft:lava" : {
            "Color" : "#FFCC4600"
        }
    }
}
""";
        return JsonSerializer.Deserialize<ViewportDefinition>(input)!;
    }
}

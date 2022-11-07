using System.Collections.Generic;
using System.Drawing;

using binstarjs03.AerialOBJ.Core.MinecraftWorld;

namespace binstarjs03.AerialOBJ.ConsoleApp;

public static class BlockToColor
{
    private static ColorConverter converter = new();
    private static Dictionary<string, Color> _blockColors = new()
    {
        {"minecraft:air", FromHex("#00000000")},

        {"minecraft:water", FromHex("#FF22417f")},

        {"minecraft:grass_block", FromHex("#FF5d923a")},
        {"minecraft:coarse_dirt", FromHex("#FF4e3826")},
        {"minecraft:podzol", FromHex("#FF3b2913")},
        {"minecraft:snow", FromHex("#FFf0fbfb")},
        {"minecraft:sand", FromHex("#FFd7ce9c")},

        {"minecraft:stone", FromHex("#FF838383")},
        {"minecraft:gravel", FromHex("#FF615e5d")},

        {"minecraft:oak_leaves", FromHex("#FF266c08")},
        {"minecraft:spruce_leaves", FromHex("#FF3f633f")},
        {"minecraft:birch_leaves", FromHex("#FF52742d")},
        {"minecraft:dark_oak_leaves", FromHex("#FF358b22")},
        {"minecraft:jungle_leaves", FromHex("#00000000")},
        {"minecraft:acacia_leaves", FromHex("#00000000")},

        {"minecraft:brown_mushroom_block", FromHex("#FF735643")},
        {"minecraft:red_mushroom_block", FromHex("#FF941e1d")},

        {"minecraft:grass", FromHex("#ff51872d")},
        {"minecraft:seagrass", FromHex("#ff2d876c")},
        {"minecraft:cornflower", FromHex("#FF46689f")},
        {"minecraft:blue_orchid", FromHex("#FF3ba6ca")},
        {"minecraft:peony", FromHex("#ffb47cac")},
    };

    public static Color Convert(Block block)
    {
        if (_blockColors.ContainsKey(block.Name))
            return _blockColors[block.Name];
        else
            return _blockColors["minecraft:air"];
    }

    public static Color FromHex(string hexColor)
    {
        return (Color)converter.ConvertFromString(hexColor);
    }
}

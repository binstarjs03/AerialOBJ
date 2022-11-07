/*
Copyright (c) 2022, Bintang Jakasurya
All rights reserved. 

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/

using System.Collections.Generic;
using System.Windows.Media;

namespace binstarjs03.AerialOBJ.WpfApp.Converters;

public static class BlockToColor
{
    private static readonly Dictionary<string, Color> s_blockColors = new()
    {
        {"aerialobj:unknown", FromHex("#FFFF00CC")},
        {"minecraft:air", FromHex("#00000000")},

        {"minecraft:water", FromHex("#22417f")},
        {"minecraft:lava", FromHex("#cc4600")},

        {"minecraft:grass_block", FromHex("#5d923a")},
        {"minecraft:mycelium", FromHex("#595155")},
        {"minecraft:podzol", FromHex("#FF3b2913")},
        {"minecraft:snow", FromHex("#FFf0fbfb")},
        {"minecraft:sand", FromHex("#FFd7ce9c")},
        {"minecraft:sandstone", FromHex("#FFd7ce9c")},

        {"minecraft:dirt", FromHex("#6c4d36")},
        {"minecraft:coarse_dirt", FromHex("#4e3826")},
        {"minecraft:stone", FromHex("#6a6a6a")},
        {"minecraft:deepslate", FromHex("#464648")},
        {"minecraft:tuff", FromHex("#575853")},
        {"minecraft:andesite", FromHex("#767676")},
        {"minecraft:gravel", FromHex("#615e5d")},
        {"minecraft:diorite", FromHex("#919194")},
        {"minecraft:granite", FromHex("#7b5c50")},

        {"minecraft:oak_leaves", FromHex("#266c08")},
        {"minecraft:spruce_leaves", FromHex("#3f633f")},
        {"minecraft:birch_leaves", FromHex("#52742d")},
        {"minecraft:dark_oak_leaves", FromHex("#358b22")},
        {"minecraft:jungle_leaves", FromHex("#00000000")},
        {"minecraft:acacia_leaves", FromHex("#00000000")},

        {"minecraft:brown_mushroom_block", FromHex("#735643")},
        {"minecraft:red_mushroom_block", FromHex("#941e1d")},

        {"minecraft:grass", FromHex("#51872d")},
        {"minecraft:seagrass", FromHex("#2d876c")},
        {"minecraft:cornflower", FromHex("#46689f")},
        {"minecraft:blue_orchid", FromHex("#3ba6ca")},
        {"minecraft:peony", FromHex("#b47cac")},
    };

    public static Color Convert(string blockName)
    {
        if (s_blockColors.ContainsKey(blockName))
            return s_blockColors[blockName];
        else
            return s_blockColors["aerialobj:unknown"];
    }

    public static Color FromHex(string hexColor)
    {
        return (Color)ColorConverter.ConvertFromString(hexColor)!;
    }
}

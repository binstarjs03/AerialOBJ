using System.Collections.Generic;
using System.Text.Json;

namespace binstarjs03.AerialOBJ.Core.Definitions;
public class ViewportDefinition : IRootDefinition
{
    public required string Name { get; set; }
    public required int FormatVersion { get; set; }
    public required string MinecraftVersion { get; set; }
    public required ViewportBlockDefinition MissingBlockDefinition { get; set; }
    public required Dictionary<string, ViewportBlockDefinition> BlockDefinitions { get; set; }

    public static ViewportDefinition GetDefaultDefinition()
    {
        string input = /*lang=json*/ """
        {
            "Name": "Default Definitions",
            "FormatVersion": 1,
            "MinecraftVersion": "1.18",
            "MissingBlockDefinition": {
                "Color": "#FF00CC",
                "DisplayName": "Unknown (Missing Definition)"
            },
            "BlockDefinitions": {
                "minecraft:air": {
                    "Color": "#000000",
                    "DisplayName": "Air"
                },
                "minecraft:cave_air": {
                    "Color": "#000000",
                    "DisplayName": "Air (Cave)"
                },
                "minecraft:stone": {
                    "Color": "#737373",
                    "DisplayName": "Stone"
                },
                "minecraft:stone_bricks": {
                    "Color": "#606060",
                    "DisplayName": "Stone Bricks"
                },
                "minecraft:mossy_stone_bricks": {
                    "Color": "#4C6056",
                    "DisplayName": "Mossy Stone Bricks"
                },
                "minecraft:cracked_stone_bricks": {
                    "Color": "#606060",
                    "DisplayName": "Cracked Stone Bricks"
                },
                "minecraft:chiseled_stone_bricks": {
                    "Color": "#606060",
                    "DisplayName": "Chiseled Stone Bricks"
                },
                "minecraft:smooth_stone": {
                    "Color": "#808080",
                    "DisplayName": "Smooth Stone"
                },
                "minecraft:cobblestone": {
                    "Color": "#808080",
                    "DisplayName": "Smooth Stone"
                },
                "minecraft:mossy_cobblestone": {
                    "Color": "#808080",
                    "DisplayName": "Smooth Stone"
                },
                "minecraft:granite": {
                    "Color": "#8D5B48",
                    "DisplayName": "Granite"
                },
                "minecraft:andesite": {
                    "Color": "#656565",
                    "DisplayName": "Andesite"
                },
                "minecraft:diorite": {
                    "Color": "#A0A0A0",
                    "DisplayName": "Diorite"
                },
                "minecraft:polished_granite": {
                    "Color": "#8D5B48",
                    "DisplayName": "Polished Granite"
                },
                "minecraft:polished_andesite": {
                    "Color": "#656565",
                    "DisplayName": "Polished Andesite"
                },
                "minecraft:polished_diorite": {
                    "Color": "#A0A0A0",
                    "DisplayName": "Polished Diorite"
                },
                "minecraft:bedrock": {
                    "Color": "#444444",
                    "DisplayName": "Bedrock"
                },
                "minecraft:calcite": {
                    "Color": "#B3B3B3",
                    "DisplayName": "Calcite"
                },
                "minecraft:tuff": {
                    "Color": "#585852",
                    "DisplayName": "Tuff"
                },
                "minecraft:dripstone_block": {
                    "Color": "#705646",
                    "DisplayName": "Dripstone Block"
                },
                "minecraft:gravel": {
                    "Color": "#6B6161",
                    "DisplayName": "Gravel"
                },
                "minecraft:clay": {
                    "Color": "#808592",
                    "DisplayName": "Clay"
                },
                "minecraft:bricks": {
                    "Color": "#83402D",
                    "DisplayName": "Bricks"
                },
                "minecraft:obsidian": {
                    "Color": "#151123",
                    "DisplayName": "Obsidian"
                },
                "minecraft:crying_obsidian": {
                    "Color": "#2A0954",
                    "DisplayName": "Crying Obsidian"
                },
                "minecraft:deepslate": {
                    "Color": "#535353",
                    "DisplayName": "Deepslate"
                },
                "minecraft:cobbled_deepslate": {
                    "Color": "#535353",
                    "DisplayName": "Cobbled Deepslate"
                },
                "minecraft:polished_deepslate": {
                    "Color": "#535353",
                    "DisplayName": "Polished Deepslate"
                },
                "minecraft:deepslate_bricks": {
                    "Color": "#4C4C4C",
                    "DisplayName": "Deepslate Bricks"
                },
                "minecraft:cracked_deepslate_bricks": {
                    "Color": "#4C4C4C",
                    "DisplayName": "Cracked Deepslate Bricks"
                },
                "minecraft:deepslate_tiles": {
                    "Color": "#2D2D2D",
                    "DisplayName": "Deepslate Tiles"
                },
                "minecraft:cracked_deepslate_tiles": {
                    "Color": "#2D2D2D",
                    "DisplayName": "Cracked Deepslate Tiles"
                },
                "minecraft:chiseled_deepslate_tiles": {
                    "Color": "#2D2D2D",
                    "DisplayName": "Chiseled Deepslate Tiles"
                },
                "minecraft:amethyst_block": {
                    "Color": "#6E4F9C",
                    "DisplayName": "Amethyst Block"
                },
                "minecraft:budding_amethyst": {
                    "Color": "#6E4F9C",
                    "DisplayName": "Budding Amethyst"
                },
                "minecraft:netherrack": {
                    "Color": "#6D3533",
                    "DisplayName": "Netherrack"
                },
                "minecraft:soul_sand": {
                    "Color": "#473326",
                    "DisplayName": "Soul Sand"
                },
                "minecraft:soul_soil": {
                    "Color": "#473326",
                    "DisplayName": "Soul Soil"
                },
                "minecraft:crimson_nylium": {
                    "Color": "#771919",
                    "DisplayName": "Crimson Nylium"
                },
                "minecraft:warped_nylium": {
                    "Color": "#206154",
                    "DisplayName": "Warped Nylium"
                },
                "minecraft:magma_block": {
                    "Color": "#893A09",
                    "DisplayName": "Magma Block"
                },
                "minecraft:glowstone": {
                    "Color": "#CDA870",
                    "DisplayName": "Glowstone"
                },
                "minecraft:basalt": {
                    "Color": "#505050",
                    "DisplayName": "Basalt"
                },
                "minecraft:polished_basalt": {
                    "Color": "#525252",
                    "DisplayName": "Polished Basalt"
                },
                "minecraft:smooth_basalt": {
                    "Color": "#3B3B3B",
                    "DisplayName": "Smooth Basalt"
                },
                "minecraft:nether_bricks": {
                    "Color": "#2B1010",
                    "DisplayName": "Nether Bricks"
                },
                "minecraft:cracked_nether_bricks": {
                    "Color": "#2B1010",
                    "DisplayName": "Cracked Nether Bricks"
                },
                "minecraft:chiseled_nether_bricks": {
                    "Color": "#2B1010",
                    "DisplayName": "Chiseled Nether Bricks"
                },
                "minecraft:red_nether_bricks": {
                    "Color": "#410101",
                    "DisplayName": "Red Nether Bricks"
                },
                "minecraft:blackstone": {
                    "Color": "#2A252C",
                    "DisplayName": "Blackstone"
                },
                "minecraft:giled_blackstone": {
                    "Color": "#2A252C",
                    "DisplayName": "Gilded Blackstone"
                },
                "minecraft:polished_blackstone": {
                    "Color": "#2A252C",
                    "DisplayName": "Polished Blackstone"
                },
                "minecraft:chiseled_polished_blackstone": {
                    "Color": "#3E3641",
                    "DisplayName": "Chiseled Polished Blackstone"
                },
                "minecraft:polished_blackstone_bricks": {
                    "Color": "#3E3641",
                    "DisplayName": "Polished Blackstone Bricks"
                },
                "minecraft:cracked_polished_blackstone_bricks": {
                    "Color": "#3E3641",
                    "DisplayName": "Cracked Polished Blackstone Bricks"
                },
                "minecraft:end_stone": {
                    "Color": "#CBCD97",
                    "DisplayName": "End Stone"
                },
                "minecraft:end_stone_bricks": {
                    "Color": "#CBCD97",
                    "DisplayName": "End Stone Bricks"
                },
                "minecraft:purpur_block": {
                    "Color": "#8A5E8A",
                    "DisplayName": "Purpur Block"
                },
                "minecraft:purpur_pillar": {
                    "Color": "#8A5E8A",
                    "DisplayName": "Purpur Pillar"
                },
                "minecraft:quartz_block": {
                    "Color": "#D1CEC8",
                    "DisplayName": "Quartz Block"
                },
                "minecraft:quartz_bricks": {
                    "Color": "#B3B0AA",
                    "DisplayName": "Quartz Bricks"
                },
                "minecraft:quartz_pillar": {
                    "Color": "#B3B0AA",
                    "DisplayName": "Quartz Pillar"
                },
                "minecraft:chiseled_quartz_block": {
                    "Color": "#B3B0AA",
                    "DisplayName": "Chiseled Quartz Block"
                },
                "minecraft:smooth_quartz": {
                    "Color": "#D1CEC8",
                    "DisplayName": "Smooth Quartz"
                },
                "minecraft:sand": {
                    "Color": "#CEC298",
                    "DisplayName": "Sand"
                },
                "minecraft:sandstone": {
                    "Color": "#B5AD7B",
                    "DisplayName": "Sandstone"
                },
                "minecraft:smooth_sandstone": {
                    "Color": "#B5AD7B",
                    "DisplayName": "Smooth Sandstone"
                },
                "minecraft:cut_sandstone": {
                    "Color": "#B5AD7B",
                    "DisplayName": "Cut Sandstone"
                },
                "minecraft:Chiseled_sandstone": {
                    "Color": "#B5AD7B",
                    "DisplayName": "Chiseled Sandstone"
                },
                "minecraft:red_sand": {
                    "Color": "#933F0B",
                    "DisplayName": "Red Sand"
                },
                "minecraft:red_sandstone": {
                    "Color": "#833906",
                    "DisplayName": "Red Sandstone"
                },
                "minecraft:smooth_red_sandstone": {
                    "Color": "#833906",
                    "DisplayName": "Smooth Red Sandstone"
                },
                "minecraft:cut_red_sandstone": {
                    "Color": "#833906",
                    "DisplayName": "Cut Red Sandstone"
                },
                "minecraft:chiseled_red_sandstone": {
                    "Color": "#833906",
                    "DisplayName": "Chiseled Red Sandstone"
                },
                "minecraft:grass_block": {
                    "Color": "#5D923A",
                    "DisplayName": "Grass Block"
                },
                "minecraft:dirt": {
                    "Color": "#845B3C",
                    "DisplayName": "Dirt"
                },
                "minecraft:coarse_dirt": {
                    "Color": "#69462B",
                    "DisplayName": "Coarse Dirt"
                },
                "minecraft:rooted_dirt": {
                    "Color": "#69462B",
                    "DisplayName": "Rooted Dirt"
                },
                "minecraft:podzol": {
                    "Color": "#51330A",
                    "DisplayName": "Podzol"
                },
                "minecraft:mycelium": {
                    "Color": "#544955",
                    "DisplayName": "Mycelium"
                },
                "minecraft:prismarine": {
                    "Color": "#5DA28C",
                    "DisplayName": "Prismarine"
                },
                "minecraft:prismarine_bricks": {
                    "Color": "#78B5A6",
                    "DisplayName": "Prismarine Bricks"
                },
                "minecraft:dark_prismarine": {
                    "Color": "#244E3D",
                    "DisplayName": "Dark Prismarine"
                },
                "minecraft:sea_lantern": {
                    "Color": "#A7AFA7",
                    "DisplayName": "Sea Lantern"
                },
                "minecraft:sponge": {
                    "Color": "#AAAB27",
                    "DisplayName": "Sponge"
                },
                "minecraft:wet_sponge": {
                    "Color": "#7D7C0D",
                    "DisplayName": "Wet Sponge"
                },
                "minecraft:dried_kelp_block": {
                    "Color": "#333726",
                    "DisplayName": "Dried Kelp Block"
                },
                "minecraft:coal_ore": {
                    "Color": "#303030",
                    "DisplayName": "Coal Ore"
                },
                "minecraft:iron_ore": {
                    "Color": "#9B785F",
                    "DisplayName": "Iron Ore"
                },
                "minecraft:copper_ore": {
                    "Color": "#7B542D",
                    "DisplayName": "Copper Ore"
                },
                "minecraft:gold_ore": {
                    "Color": "#CFC11E",
                    "DisplayName": "Gold Ore"
                },
                "minecraft:redstone_ore": {
                    "Color": "#860000",
                    "DisplayName": "Redstone Ore"
                },
                "minecraft:emerald_ore": {
                    "Color": "#617265",
                    "DisplayName": "Emerald Ore"
                },
                "minecraft:lapis_ore": {
                    "Color": "#092D75",
                    "DisplayName": "Lapis Lazuli Ore"
                },
                "minecraft:diamond_ore": {
                    "Color": "#36C5CE",
                    "DisplayName": "Diamond Ore"
                },
                "minecraft:deepslate_coal_ore": {
                    "Color": "#202020",
                    "DisplayName": "Deepslate Coal Ore"
                },
                "minecraft:deepslate_iron_ore": {
                    "Color": "#735846",
                    "DisplayName": "Deepslate Iron Ore"
                },
                "minecraft:deepslate_copper_ore": {
                    "Color": "#53381E",
                    "DisplayName": "Deepslate Copper Ore"
                },
                "minecraft:deepslate_gold_ore": {
                    "Color": "#A79B18",
                    "DisplayName": "Deepslate Gold Ore"
                },
                "minecraft:deepslate_redstone_ore": {
                    "Color": "#653E3F",
                    "DisplayName": "Deepslate Redstone Ore"
                },
                "minecraft:deepslate_emerald_ore": {
                    "Color": "#3F4A41",
                    "DisplayName": "Deepslate Emerald Ore"
                },
                "minecraft:deepslate_lapis_ore": {
                    "Color": "#051D4D",
                    "DisplayName": "Deepslate Lapis Lazuli Ore"
                },
                "minecraft:deepslate_diamond_ore": {
                    "Color": "#2BA0A6",
                    "DisplayName": "Deepslate Diamond Ore"
                },
                "minecraft:raw_iron_block": {
                    "Color": "#A2836A",
                    "DisplayName": "Block of Raw Iron"
                },
                "minecraft:raw_copper_block": {
                    "Color": "#8A543A",
                    "DisplayName": "Block of Raw Copper"
                },
                "minecraft:raw_gold_block": {
                    "Color": "#BC8C1A",
                    "DisplayName": "Block of Raw Gold"
                },
                "minecraft:coal_block": {
                    "Color": "#171717",
                    "DisplayName": "Block of Coal"
                },
                "minecraft:iron_block": {
                    "Color": "#BEBEBE",
                    "DisplayName": "Block of Iron"
                },
                "minecraft:copper_block": {
                    "Color": "#A75237",
                    "DisplayName": "Block of Copper"
                },
                "minecraft:gold_block": {
                    "Color": "#D0C920",
                    "DisplayName": "Block of Gold"
                },
                "minecraft:redstone_block": {
                    "Color": "#9A1000",
                    "DisplayName": "Block of Redstone"
                },
                "minecraft:emerald_block": {
                    "Color": "#32B657",
                    "DisplayName": "Block of Emerald"
                },
                "minecraft:lapis_block": {
                    "Color": "#0B3EAB",
                    "DisplayName": "Block of Lapis Lazuli"
                },
                "minecraft:diamond_block": {
                    "Color": "#57BBB7",
                    "DisplayName": "Block of Diamond"
                },
                "minecraft:netherite_block": {
                    "Color": "#363234",
                    "DisplayName": "Block of Netherite"
                },
                "minecraft:nether_gold_ore": {
                    "Color": "#78482B",
                    "DisplayName": "Nether Gold Ore"
                },
                "minecraft:nether_quartz_ore": {
                    "Color": "#664743",
                    "DisplayName": "Nether Quartz Ore"
                },
                "minecraft:ancient_debris": {
                    "Color": "#553E38",
                    "DisplayName": "Ancient Debris"
                },
                "minecraft:exposed_copper": {
                    "Color": "#9A7560",
                    "DisplayName": "Exposed Copper"
                },
                "minecraft:weathered_copper": {
                    "Color": "#547853",
                    "DisplayName": "Weathered Copper"
                },
                "minecraft:oxidized_copper": {
                    "Color": "#38896B",
                    "DisplayName": "Oxidized Copper"
                },
                "minecraft:cut_copper": {
                    "Color": "#A75237",
                    "DisplayName": "Cut Copper"
                },
                "minecraft:exposed_cut_copper": {
                    "Color": "#9A7560",
                    "DisplayName": "Exposed Cut Copper"
                },
                "minecraft:weathered_cut_copper": {
                    "Color": "#547853",
                    "DisplayName": "Weathered Cut Copper"
                },
                "minecraft:oxidized_cut_copper": {
                    "Color": "#38896B",
                    "DisplayName": "Oxidized Cut Copper"
                },
                "minecraft:waxed_copper": {
                    "Color": "#A75237",
                    "DisplayName": "Waxed Block of Copper"
                },
                "minecraft:waxed_exposed_copper": {
                    "Color": "#9A7560",
                    "DisplayName": "Waxed Exposed Copper"
                },
                "minecraft:waxed_weathered_copper": {
                    "Color": "#547853",
                    "DisplayName": "Waxed Weathered Copper"
                },
                "minecraft:waxed_oxidized_copper": {
                    "Color": "#38896B",
                    "DisplayName": "Waxed Oxidized Copper"
                },
                "minecraft:waxed_cut_copper": {
                    "Color": "#A75237",
                    "DisplayName": "Cut Copper"
                },
                "minecraft:waxed_exposed_cut_copper": {
                    "Color": "#9A7560",
                    "DisplayName": "Waxed Exposed Cut Copper"
                },
                "minecraft:waxed_weathered_cut_copper": {
                    "Color": "#547853",
                    "DisplayName": "Waxed Weathered Cut Copper"
                },
                "minecraft:waxed_oxidized_cut_copper": {
                    "Color": "#38896B",
                    "DisplayName": "Waxed Oxidized Cut Copper"
                },
                "minecraft:dead_tube_coral_block": {
                    "Color": "#6B645F",
                    "DisplayName": "Dead Tube Coral Block"
                },
                "minecraft:dead_brain_coral_block": {
                    "Color": "#6B645F",
                    "DisplayName": "Dead Brain Coral Block"
                },
                "minecraft:dead_bubble_coral_block": {
                    "Color": "#6B645F",
                    "DisplayName": "Dead Bubble Coral Block"
                },
                "minecraft:dead_fire_coral_block": {
                    "Color": "#6B645F",
                    "DisplayName": "Dead Fire Coral Block"
                },
                "minecraft:dead_horn_coral_block": {
                    "Color": "#6B645F",
                    "DisplayName": "Dead Horn Coral Block"
                },
                "minecraft:tube_coral_block": {
                    "Color": "#2C51C4",
                    "DisplayName": "Tube Coral Block"
                },
                "minecraft:brain_coral_block": {
                    "Color": "#AD4280",
                    "DisplayName": "Brain Coral Block"
                },
                "minecraft:bubble_coral_block": {
                    "Color": "#940C90",
                    "DisplayName": "Bubble Coral Block"
                },
                "minecraft:fire_coral_block": {
                    "Color": "#9B171E",
                    "DisplayName": "Fire Coral Block"
                },
                "minecraft:horn_coral_block": {
                    "Color": "#AD991B",
                    "DisplayName": "Horn Coral Block"
                },
                "minecraft:white_concrete": {
                    "Color": "#C7C7C7",
                    "DisplayName": "White Concrete"
                },
                "minecraft:orange_concrete": {
                    "Color": "#C74800",
                    "DisplayName": "Orange Concrete"
                },
                "minecraft:magenta_concrete": {
                    "Color": "#951C8B",
                    "DisplayName": "Magenta Concrete"
                },
                "minecraft:light_blue_concrete": {
                    "Color": "#0D72B0",
                    "DisplayName": "Light Blue Concrete"
                },
                "minecraft:yellow_concrete": {
                    "Color": "#CD8B00",
                    "DisplayName": "Yellow Concrete"
                },
                "minecraft:lime_concrete": {
                    "Color": "#438E00",
                    "DisplayName": "Lime Concrete"
                },
                "minecraft:pink_concrete": {
                    "Color": "#BA4973",
                    "DisplayName": "Pink Concrete"
                },
                "minecraft:gray_concrete": {
                    "Color": "#323232",
                    "DisplayName": "Gray Concrete"
                },
                "minecraft:light_gray_concrete": {
                    "Color": "#646464",
                    "DisplayName": "Light Gray Concrete"
                },
                "minecraft:cyan_concrete": {
                    "Color": "#026475",
                    "DisplayName": "Cyan Concrete"
                },
                "minecraft:purple_concrete": {
                    "Color": "#56128E",
                    "DisplayName": "Purple Concrete"
                },
                "minecraft:blue_concrete": {
                    "Color": "#212383",
                    "DisplayName": "Blue Concrete"
                },
                "minecraft:brown_concrete": {
                    "Color": "#522E12",
                    "DisplayName": "Brown Concrete"
                },
                "minecraft:green_concrete": {
                    "Color": "#394B14",
                    "DisplayName": "Green Concrete"
                },
                "minecraft:red_concrete": {
                    "Color": "#801313",
                    "DisplayName": "Red Concrete"
                },
                "minecraft:black_concrete": {
                    "Color": "#0D0D0D",
                    "DisplayName": "Black Concrete"
                },
                "minecraft:white_concrete_powder": {
                    "Color": "#E3E3E3",
                    "DisplayName": "White Concrete Powder"
                },
                "minecraft:orange_concrete_powder": {
                    "Color": "#E35300",
                    "DisplayName": "Orange Concrete Powder"
                },
                "minecraft:magenta_concrete_powder": {
                    "Color": "#BC26AF",
                    "DisplayName": "Magenta Concrete Powder"
                },
                "minecraft:light_blue_concrete_powder": {
                    "Color": "#1288D0",
                    "DisplayName": "Light Blue Concrete Powder"
                },
                "minecraft:yellow_concrete_powder": {
                    "Color": "#E89E00",
                    "DisplayName": "Yellow Concrete Powder"
                },
                "minecraft:lime_concrete_powder": {
                    "Color": "#58B600",
                    "DisplayName": "Lime Concrete Powder"
                },
                "minecraft:pink_concrete_powder": {
                    "Color": "#D95687",
                    "DisplayName": "Pink Concrete Powder"
                },
                "minecraft:gray_concrete_powder": {
                    "Color": "#9B9B9B",
                    "DisplayName": "Gray Concrete Powder"
                },
                "minecraft:light_gray_concrete_powder": {
                    "Color": "#7C7C76",
                    "DisplayName": "Light Gray Concrete Powder"
                },
                "minecraft:cyan_concrete_powder": {
                    "Color": "#048EA5",
                    "DisplayName": "Cyan Concrete Powder"
                },
                "minecraft:purple_concrete_powder": {
                    "Color": "#701AB6",
                    "DisplayName": "Purple Concrete Powder"
                },
                "minecraft:blue_concrete_powder": {
                    "Color": "#2F32AF",
                    "DisplayName": "Blue Concrete Powder"
                },
                "minecraft:brown_concrete_powder": {
                    "Color": "#77451E",
                    "DisplayName": "Brown Concrete Powder"
                },
                "minecraft:green_concrete_powder": {
                    "Color": "#597323",
                    "DisplayName": "Green Concrete Powder"
                },
                "minecraft:red_concrete_powder": {
                    "Color": "#AD1D1D",
                    "DisplayName": "Red Concrete Powder"
                },
                "minecraft:black_concrete_powder": {
                    "Color": "#424242",
                    "DisplayName": "Black Concrete Powder"
                },
                "minecraft:terracotta": {
                    "Color": "#8D533A",
                    "DisplayName": "Terracotta"
                },
                "minecraft:white_terracotta": {
                    "Color": "#A98979",
                    "DisplayName": "White Terracotta"
                },
                "minecraft:orange_terracotta": {
                    "Color": "#893C0D",
                    "DisplayName": "Orange Terracotta"
                },
                "minecraft:magenta_terracotta": {
                    "Color": "#7D3F54",
                    "DisplayName": "Magenta Terracotta"
                },
                "minecraft:yellow_terracotta": {
                    "Color": "#9B6504",
                    "DisplayName": "Yellow Terracotta"
                },
                "minecraft:lime_terracotta": {
                    "Color": "#4F5D1B",
                    "DisplayName": "Lime Terracotta"
                },
                "minecraft:pink_terracotta": {
                    "Color": "#893637",
                    "DisplayName": "Pink Terracotta"
                },
                "minecraft:gray_terracotta": {
                    "Color": "#2D1D18",
                    "DisplayName": "Gray Terracotta"
                },
                "minecraft:light_gray_terracotta": {
                    "Color": "#6E5047",
                    "DisplayName": "Light Gray Terracotta"
                },
                "minecraft:cyan_terracotta": {
                    "Color": "#414546",
                    "DisplayName": "Cyan Terracotta"
                },
                "minecraft:purple_terracotta": {
                    "Color": "#623142",
                    "DisplayName": "Purple Terracotta"
                },
                "minecraft:blue_terracotta": {
                    "Color": "#3A2B4B",
                    "DisplayName": "Blue Terracotta"
                },
                "minecraft:brown_terracotta": {
                    "Color": "#3F2416",
                    "DisplayName": "Brown Terracotta"
                },
                "minecraft:green_terracotta": {
                    "Color": "#394018",
                    "DisplayName": "Green Terracotta"
                },
                "minecraft:red_terracotta": {
                    "Color": "#7B291B",
                    "DisplayName": "Red Terracotta"
                },
                "minecraft:black_terracotta": {
                    "Color": "#1C0D08",
                    "DisplayName": "Black Terracotta"
                },
                "minecraft:white_wool": {
                    "Color": "#E3E3E3",
                    "DisplayName": "White Wool"
                },
                "minecraft:orange_wool": {
                    "Color": "#E35300",
                    "DisplayName": "Orange Wool"
                },
                "minecraft:magenta_wool": {
                    "Color": "#BC26AF",
                    "DisplayName": "Magenta Wool"
                },
                "minecraft:light_blue_wool": {
                    "Color": "#1288D0",
                    "DisplayName": "Light Blue Wool"
                },
                "minecraft:yellow_wool": {
                    "Color": "#E89E00",
                    "DisplayName": "Yellow Wool"
                },
                "minecraft:lime_wool": {
                    "Color": "#58B600",
                    "DisplayName": "Lime Wool"
                },
                "minecraft:pink_wool": {
                    "Color": "#D95687",
                    "DisplayName": "Pink Wool"
                },
                "minecraft:gray_wool": {
                    "Color": "#9B9B9B",
                    "DisplayName": "Gray Wool"
                },
                "minecraft:light_gray_wool": {
                    "Color": "#7C7C76",
                    "DisplayName": "Light Gray Wool"
                },
                "minecraft:cyan_wool": {
                    "Color": "#048EA5",
                    "DisplayName": "Cyan Wool"
                },
                "minecraft:purple_wool": {
                    "Color": "#701AB6",
                    "DisplayName": "Purple Wool"
                },
                "minecraft:blue_wool": {
                    "Color": "#2F32AF",
                    "DisplayName": "Blue Wool"
                },
                "minecraft:brown_wool": {
                    "Color": "#77451E",
                    "DisplayName": "Brown Wool"
                },
                "minecraft:green_wool": {
                    "Color": "#597323",
                    "DisplayName": "Green Wool"
                },
                "minecraft:red_wool": {
                    "Color": "#AD1D1D",
                    "DisplayName": "Red Wool"
                },
                "minecraft:black_wool": {
                    "Color": "#424242",
                    "DisplayName": "Black Wool"
                },
                "minecraft:glass": {
                    "Color": "#80A6CED6",
                    "DisplayName": "Glass"
                },
                "minecraft:tinted_glass": {
                    "Color": "#80252523",
                    "DisplayName": "Tinted Glass"
                },
                "minecraft:white_stained_glass": {
                    "Color": "#80E3E3E3",
                    "DisplayName": "White Stained Glass"
                },
                "minecraft:orange_stained_glass": {
                    "Color": "#80E35300",
                    "DisplayName": "Orange Stained Glass"
                },
                "minecraft:magenta_stained_glass": {
                    "Color": "#80BC26AF",
                    "DisplayName": "Magenta Stained Glass"
                },
                "minecraft:light_blue_stained_glass": {
                    "Color": "#801288D0",
                    "DisplayName": "Light Blue Stained Glass"
                },
                "minecraft:yellow_stained_glass": {
                    "Color": "#80E89E00",
                    "DisplayName": "Yellow Stained Glass"
                },
                "minecraft:lime_stained_glass": {
                    "Color": "#8058B600",
                    "DisplayName": "Lime Stained Glass"
                },
                "minecraft:pink_stained_glass": {
                    "Color": "#80D95687",
                    "DisplayName": "Pink Stained Glass"
                },
                "minecraft:gray_stained_glass": {
                    "Color": "#809B9B9B",
                    "DisplayName": "Gray Stained Glass"
                },
                "minecraft:light_gray_stained_glass": {
                    "Color": "#807C7C76",
                    "DisplayName": "Light Gray Stained Glass"
                },
                "minecraft:cyan_stained_glass": {
                    "Color": "#80048EA5",
                    "DisplayName": "Cyan Stained Glass"
                },
                "minecraft:purple_stained_glass": {
                    "Color": "#80701AB6",
                    "DisplayName": "Purple Stained Glass"
                },
                "minecraft:blue_stained_glass": {
                    "Color": "#802F32AF",
                    "DisplayName": "Blue Stained Glass"
                },
                "minecraft:brown_stained_glass": {
                    "Color": "#8077451E",
                    "DisplayName": "Brown Stained Glass"
                },
                "minecraft:green_stained_glass": {
                    "Color": "#80597323",
                    "DisplayName": "Green Stained Glass"
                },
                "minecraft:red_stained_glass": {
                    "Color": "#80AD1D1D",
                    "DisplayName": "Red Stained Glass"
                },
                "minecraft:black_stained_glass": {
                    "Color": "#80424242",
                    "DisplayName": "Black Stained Glass"
                },
                "minecraft:oak_planks": {
                    "Color": "#937840",
                    "DisplayName": "Oak Planks"
                },
                "minecraft:spruce_planks": {
                    "Color": "#654523",
                    "DisplayName": "SprucePlanks"
                },
                "minecraft:birch_planks": {
                    "Color": "#B09A5E",
                    "DisplayName": "Birch Planks"
                },
                "minecraft:jungle_planks": {
                    "Color": "#956440",
                    "DisplayName": "Jungle Planks"
                },
                "minecraft:acacia_planks": {
                    "Color": "#A14F22",
                    "DisplayName": "Acacia Planks"
                },
                "minecraft:dark_oak_planks": {
                    "Color": "#3E240C",
                    "DisplayName": "Dark Oak Planks"
                },
                "minecraft:crimson_planks": {
                    "Color": "#5B243B",
                    "DisplayName": "Crimson Planks"
                },
                "minecraft:warped_planks": {
                    "Color": "#1B5B56",
                    "DisplayName": "Warped Planks"
                },
                "minecraft:oak_log": {
                    "Color": "#634D2D",
                    "DisplayName": "Oak Log"
                },
                "minecraft:spruce_log": {
                    "Color": "#231200",
                    "DisplayName": "Spruce Log"
                },
                "minecraft:birch_log ": {
                    "Color": "#B4BAB1",
                    "DisplayName": "Birch Log"
                },
                "minecraft:jungle_log": {
                    "Color": "#4A360B",
                    "DisplayName": "Jungle Log"
                },
                "minecraft:acacia_log": {
                    "Color": "#4A360B",
                    "DisplayName": "Acacia Log"
                },
                "minecraft:dark_oak_log": {
                    "Color": "#2B1F0D",
                    "DisplayName": "Dark Oak Log"
                },
                "minecraft:crimson_stem": {
                    "Color": "#6B2943",
                    "DisplayName": "Crimson Stem"
                },
                "minecraft:warped_stem": {
                    "Color": "#1F6D69",
                    "DisplayName": "Warped Stem"
                },
                "minecraft:stripped_oak_log": {
                    "Color": "#937840",
                    "DisplayName": "Stripped Oak Log"
                },
                "minecraft:stripped_spruce_log": {
                    "Color": "#231200",
                    "DisplayName": "Stripped Spruce Log"
                },
                "minecraft:stripped_birch_log": {
                    "Color": "#B4BAB1",
                    "DisplayName": "Stripped Birch Log"
                },
                "minecraft:stripped_jungle_log": {
                    "Color": "#4A360B",
                    "DisplayName": "Stripped Jungle Log"
                },
                "minecraft:stripped_acacia_log": {
                    "Color": "#A14F22",
                    "DisplayName": "Stripped Acacia Log"
                },
                "minecraft:stripped_dark_oak_log": {
                    "Color": "#2B1F0D",
                    "DisplayName": "Stripped Dark Oak Log"
                },
                "minecraft:stripped_crimson_stem": {
                    "Color": "#6B2943",
                    "DisplayName": "Stripped Crimson Stem"
                },
                "minecraft:stripped_warped_stem": {
                    "Color": "#1F6D69",
                    "DisplayName": "Stripped Warped Stem"
                },
                "minecraft:stripped_oak_wood": {
                    "Color": "#937840",
                    "DisplayName": "Stripped Oak Wood"
                },
                "minecraft:stripped_spruce_wood": {
                    "Color": "#231200",
                    "DisplayName": "Stripped Spruce Wood"
                },
                "minecraft:stripped_birch_wood": {
                    "Color": "#B4BAB1",
                    "DisplayName": "Stripped Birch Wood"
                },
                "minecraft:stripped_jungle_wood": {
                    "Color": "#4A360B",
                    "DisplayName": "Stripped Jungle Wood"
                },
                "minecraft:stripped_acacia_wood": {
                    "Color": "#A14F22",
                    "DisplayName": "Stripped Acacia Wood"
                },
                "minecraft:stripped_dark_oak_wood": {
                    "Color": "#2B1F0D",
                    "DisplayName": "Stripped Dark Oak Wood"
                },
                "minecraft:stripped_crimson_hyphae": {
                    "Color": "#6B2943",
                    "DisplayName": "Stripped Crimson Hyphae"
                },
                "minecraft:stripped_warped_hyphae": {
                    "Color": "#1F6D69",
                    "DisplayName": "Stripped Warped Hyphae"
                },
                "minecraft:oak_wood": {
                    "Color": "#634D2D",
                    "DisplayName": "Oak Wood"
                },
                "minecraft:spruce_wood": {
                    "Color": "#231200",
                    "DisplayName": "Spruce Wood"
                },
                "minecraft:birch_wood": {
                    "Color": "#B4BAB1",
                    "DisplayName": "Birch Wood"
                },
                "minecraft:jungle_wood": {
                    "Color": "#4A360B",
                    "DisplayName": "Jungle Wood"
                },
                "minecraft:acacia_wood": {
                    "Color": "#4A360B",
                    "DisplayName": "Acacia Wood"
                },
                "minecraft:dark_oak_wood": {
                    "Color": "#2B1F0D",
                    "DisplayName": "Dark Oak Wood"
                },
                "minecraft:crimson_hyphae": {
                    "Color": "#6B2943",
                    "DisplayName": "Crimson Hyphae"
                },
                "minecraft:warped_hyphae": {
                    "Color": "#1F6D69",
                    "DisplayName": "Warped Hyphae"
                },
                "minecraft:nether_wart_block": {
                    "Color": "#74090A",
                    "DisplayName": "Nether Wart Block"
                },
                "minecraft:warped_wart_block": {
                    "Color": "#046767",
                    "DisplayName": "Warped Wart Block"
                },
                "minecraft:stone_slab": {
                    "Color": "#737373",
                    "DisplayName": "Stone Slab"
                },
                "minecraft:smooth_stone_slab": {
                    "Color": "#808080",
                    "DisplayName": "Smooth Stone Slab"
                },
                "minecraft:stone_brick_slab": {
                    "Color": "#606060",
                    "DisplayName": "Stone Brick Slab"
                },
                "minecraft:mossy_stone_brick_slab": {
                    "Color": "#4C6056",
                    "DisplayName": "Mossy Stone Brick Slab"
                },
                "minecraft:cobblestone_slab": {
                    "Color": "#797979",
                    "DisplayName": "Cobblestone Slab"
                },
                "minecraft:granite_slab": {
                    "Color": "#8D5B48",
                    "DisplayName": "Granite Slab"
                },
                "minecraft:diorite_slab": {
                    "Color": "#7B7B7E",
                    "DisplayName": "Diorite Slab"
                },
                "minecraft:andesite_slab": {
                    "Color": "#656569",
                    "DisplayName": "Andesite Slab"
                },
                "minecraft:polished_granite_slab": {
                    "Color": "#8D5B48",
                    "DisplayName": "Polished Granite Slab"
                },
                "minecraft:polished_diorite_slab": {
                    "Color": "#656565",
                    "DisplayName": "Polished Diorite Slab"
                },
                "minecraft:polished_andesite_slab": {
                    "Color": "#A0A0A0",
                    "DisplayName": "Polished Andesite Slab"
                },
                "minecraft:sandstone_slab": {
                    "Color": "#B5AD7B",
                    "DisplayName": "Sandstone Slab"
                },
                "minecraft:smooth_sandstone_slab": {
                    "Color": "#B5AD7B",
                    "DisplayName": "Smooth Sandstone Slab"
                },
                "minecraft:cut_sandstone_slab": {
                    "Color": "#B5AD7B",
                    "DisplayName": "Cut Sandstone Slab"
                },
                "minecraft:red_sandstone_slab": {
                    "Color": "#833906",
                    "DisplayName": "Red Sandstone Slab"
                },
                "minecraft:smooth_red_sandstone_slab": {
                    "Color": "#833906",
                    "DisplayName": "Smooth Red Sandstone Slab"
                },
                "minecraft:cut_red_sandstone_slab": {
                    "Color": "#833906",
                    "DisplayName": "Cut Red Sandstone Slab"
                },
                "minecraft:brick_slab": {
                    "Color": "#83402D",
                    "DisplayName": "Brick Slab"
                },
                "minecraft:nether_brick_slab": {
                    "Color": "#2B1015",
                    "DisplayName": "Nether Brick Slab"
                },
                "minecraft:red_nether_brick_slab": {
                    "Color": "#410103",
                    "DisplayName": "Red Nether Brick Slab"
                },
                "minecraft:quartz_slab": {
                    "Color": "#B3B0AA",
                    "DisplayName": "Quartz Slab"
                },
                "minecraft:smooth_quartz_slab ": {
                    "Color": "#B3B0AA",
                    "DisplayName": "Smooth Quartz Slab"
                },
                "minecraft:prismarine_slab": {
                    "Color": "#5BA296",
                    "DisplayName": "Prismarine Slab"
                },
                "minecraft:prismarine_brick_slab": {
                    "Color": "#4B8778",
                    "DisplayName": "Prismarine Brick Slab"
                },
                "minecraft:dark_prismarine_slab": {
                    "Color": "#244E3D",
                    "DisplayName": "Dark Prismarine Slab"
                },
                "minecraft:cobbled_deepslate_slab": {
                    "Color": "#3C3C3C",
                    "DisplayName": "Cobbled Deepslate Slab"
                },
                "minecraft:polished_deepslate_slab": {
                    "Color": "#3C3C3C",
                    "DisplayName": "Polished Deepslate Slab"
                },
                "minecraft:deepslate_brick_slab": {
                    "Color": "#3C3C3C",
                    "DisplayName": "Deepslate Brick Slab"
                },
                "minecraft:deepslate_tile_slab": {
                    "Color": "#3C3C3C",
                    "DisplayName": "Deepslate Tile Slab"
                },
                "minecraft:end_stone_brick_slab": {
                    "Color": "#CBCD97",
                    "DisplayName": "End Stone Brick Slab"
                },
                "minecraft:purpur_slab": {
                    "Color": "#8A5E8A",
                    "DisplayName": "Purpur Slab"
                },
                "minecraft:blackstone_slab": {
                    "Color": "#241F26",
                    "DisplayName": "Blackstone Slab"
                },
                "minecraft:polished_blackstone_slab": {
                    "Color": "#241F26",
                    "DisplayName": "Polished Blackstone Slab"
                },
                "minecraft:polished_blackstone_brick_slab": {
                    "Color": "#241F26",
                    "DisplayName": "Polished Blackstone Brick Slab"
                },
                "minecraft:cut_copper_slab": {
                    "Color": "#A75237",
                    "DisplayName": "Cut Copper Slab"
                },
                "minecraft:exposed_cut_copper_slab": {
                    "Color": "#9A7560",
                    "DisplayName": "Exposed Cut Copper Slab"
                },
                "minecraft:weathered_cut_copper_slab": {
                    "Color": "#547853",
                    "DisplayName": "Weathered Cut Copper Slab"
                },
                "minecraft:oxidized_cut_copper_slab": {
                    "Color": "#38896B",
                    "DisplayName": "Oxidized Cut Copper Slab"
                },
                "minecraft:waxed_cut_copper_slab": {
                    "Color": "#A75237",
                    "DisplayName": "Waxed Cut Copper Slab"
                },
                "minecraft:waxed_exposed_cut_copper_slab": {
                    "Color": "#9A7560",
                    "DisplayName": "Waxed Exposed Cut Copper Slab"
                },
                "minecraft:waxed_weathered_cut_copper_slab": {
                    "Color": "#547853",
                    "DisplayName": "Waxed Weathered Cut Copper Slab"
                },
                "minecraft:waxed_oxidized_cut_copper_slab": {
                    "Color": "#38896B",
                    "DisplayName": "Waxed Oxidized Cut Copper Slab"
                },
                "minecraft:petrified_oak_slab": {
                    "Color": "#937840",
                    "DisplayName": "Petrified Oak Slab"
                },
                "minecraft:oak_slab": {
                    "Color": "#937840",
                    "DisplayName": "Oak Slab"
                },
                "minecraft:spruce_slab": {
                    "Color": "#654523",
                    "DisplayName": "Spruce Slab"
                },
                "minecraft:birch_slab": {
                    "Color": "#B09A5E",
                    "DisplayName": "Birch Slab"
                },
                "minecraft:jungle_slab": {
                    "Color": "#956440",
                    "DisplayName": "Jungle Slab"
                },
                "minecraft:acacia_slab": {
                    "Color": "#A14F22",
                    "DisplayName": "Acacia Slab"
                },
                "minecraft:dark_oak_slab": {
                    "Color": "#3E240C",
                    "DisplayName": "Dark Oak Slab"
                },
                "minecraft:crimson_slab": {
                    "Color": "#5B243B",
                    "DisplayName": "Crimson Slab"
                },
                "minecraft:warped_slab": {
                    "Color": "#1B5B56",
                    "DisplayName": "Warped Slab"
                },
                "minecraft:stone_stairs": {
                    "Color": "#737373",
                    "DisplayName": "Stone Stairs"
                },
                "minecraft:stone_brick_stairs": {
                    "Color": "#606060",
                    "DisplayName": "Stone Brick Stairs"
                },
                "minecraft:mossy_stone_brick_stairs": {
                    "Color": "#4C6056",
                    "DisplayName": "Mossy Stone Brick Stairs"
                },
                "minecraft:cobblestone_stairs": {
                    "Color": "#797979",
                    "DisplayName": "Cobblestone Stairs"
                },
                "minecraft:mossy_cobblestone_stairs": {
                    "Color": "#65796B",
                    "DisplayName": "Mossy Cobblestone Stairs"
                },
                "minecraft:granite_stairs": {
                    "Color": "#8D5B48",
                    "DisplayName": "Granite Stairs"
                },
                "minecraft:andesite_stairs": {
                    "Color": "#656565",
                    "DisplayName": "Andesite Stairs"
                },
                "minecraft:diorite_stairs": {
                    "Color": "#A0A0A0",
                    "DisplayName": "Diorite Stairs"
                },
                "minecraft:polished_granite_stairs": {
                    "Color": "#8D5B48",
                    "DisplayName": "Polished Granite Stairs"
                },
                "minecraft:polished_andesite_stairs": {
                    "Color": "#656565",
                    "DisplayName": "Polished Andesite Stairs"
                },
                "minecraft:polished_diorite_stairs": {
                    "Color": "#A0A0A0",
                    "DisplayName": "Polished Diorite Stairs"
                },
                "minecraft:cobbled_deepslate_stairs": {
                    "Color": "#535353",
                    "DisplayName": "Cobbled Deepslate Stairs"
                },
                "minecraft:polished_deepslate_stairs": {
                    "Color": "#535353",
                    "DisplayName": "Polished Deepslate Stairs"
                },
                "minecraft:deepslate_brick_stairs": {
                    "Color": "#4C4C4C",
                    "DisplayName": "Deepslate Brick Stairs"
                },
                "minecraft:deepslate_tile_stairs": {
                    "Color": "#2D2D2E",
                    "DisplayName": "Deepslate Tile Stairs"
                },
                "minecraft:brick_stairs": {
                    "Color": "#83402D",
                    "DisplayName": "Brick Stairs"
                },
                "minecraft:sandstone_stairs": {
                    "Color": "#B5AD7B",
                    "DisplayName": "Sandstone Stairs"
                },
                "minecraft:smooth_sandstone_stairs": {
                    "Color": "#B5AD7B",
                    "DisplayName": "Smooth Sandstone Stairs"
                },
                "minecraft:red_sandstone_stairs": {
                    "Color": "#833906",
                    "DisplayName": "Red Sandstone Stairs"
                },
                "minecraft:smooth_red_sandstone_stairs": {
                    "Color": "#833906",
                    "DisplayName": "Smooth Red Sandstone Stairs"
                },
                "minecraft:nether_brick_stairs": {
                    "Color": "#2B1015",
                    "DisplayName": "Nether Brick Stairs"
                },
                "minecraft:red_nether_brick_stairs": {
                    "Color": "#410103",
                    "DisplayName": "Red Nether Brick Stairs"
                },
                "minecraft:quartz_stairs": {
                    "Color": "#B3B0AA",
                    "DisplayName": "Quartz Stairs"
                },
                "minecraft:smooth_quartz_stairs": {
                    "Color": "#D1CEC8",
                    "DisplayName": "Smooth Quartz Stairs"
                },
                "minecraft:blackstone_stairs": {
                    "Color": "#2A252C",
                    "DisplayName": "Blackstone Stairs"
                },
                "minecraft:polished_blackstone_stairs": {
                    "Color": "#2A252C",
                    "DisplayName": "Polished Blackstone Stairs"
                },
                "minecraft:polished_blackstone_brick_stairs": {
                    "Color": "#3E3641",
                    "DisplayName": "Polished Blackstone Brick Stairs"
                },
                "minecraft:end_stone_brick_stairs": {
                    "Color": "#CBCD97",
                    "DisplayName": "End Stone Stairs"
                },
                "minecraft:purpur_stairs": {
                    "Color": "#8A5E8A",
                    "DisplayName": "Purpur Stairs"
                },
                "minecraft:prismarine_stairs": {
                    "Color": "#5DA28C",
                    "DisplayName": "Prismarine Stairs"
                },
                "minecraft:prismarine_brick_stairs": {
                    "Color": "#78B5A6",
                    "DisplayName": "Prismarine Brick Stairs"
                },
                "minecraft:dark_prismarine_stairs": {
                    "Color": "#244E3D",
                    "DisplayName": "Dark Prismarine Stairs"
                },
                "minecraft:cut_copper_stairs": {
                    "Color": "#A75237",
                    "DisplayName": "Cut Copper Stairs"
                },
                "minecraft:exposed_cut_copper_stairs": {
                    "Color": "#9A7560",
                    "DisplayName": "Exposed Cut Copper Stairs"
                },
                "minecraft:weathered_cut_copper_stairs": {
                    "Color": "#547853",
                    "DisplayName": "WeatheredCut Copper Stairs"
                },
                "minecraft:oxidized_cut_copper_stairs": {
                    "Color": "#38896B",
                    "DisplayName": "Oxidized Cut Copper Stairs"
                },
                "minecraft:waxed_cut_copper_stairs": {
                    "Color": "#A75237",
                    "DisplayName": "Waxed Cut Copper Stairs"
                },
                "minecraft:waxed_exposed_cut_copper_stairs": {
                    "Color": "#9A7560",
                    "DisplayName": "Waxed Exposed Cut Copper Stairs"
                },
                "minecraft:waxed_weathered_cut_copper_stairs": {
                    "Color": "#547853",
                    "DisplayName": "Waxed WeatheredCut Copper Stairs"
                },
                "minecraft:waxed_oxidized_cut_copper_stairs": {
                    "Color": "#38896B",
                    "DisplayName": "Waxed Oxidized Cut Copper Stairs"
                },
                "minecraft:oak_stairs": {
                    "Color": "#937840",
                    "DisplayName": "Oak Stairs"
                },
                "minecraft:acacia_stairs": {
                    "Color": "#654523",
                    "DisplayName": "Spruce Stairs"
                },
                "minecraft:dark_oak_stairs": {
                    "Color": "#B09A5E",
                    "DisplayName": "Birch Stairs"
                },
                "minecraft:spruce_stairs": {
                    "Color": "#956440",
                    "DisplayName": "Jungle Stairs"
                },
                "minecraft:birch_stairs": {
                    "Color": "#A14F22",
                    "DisplayName": "Acacia Stairs"
                },
                "minecraft:jungle_stairs": {
                    "Color": "#3E240C",
                    "DisplayName": "Dark Stairs"
                },
                "minecraft:crimson_stairs": {
                    "Color": "#5B243B",
                    "DisplayName": "Crimson Stairs"
                },
                "minecraft:warped_stairs": {
                    "Color": "#1B5B56",
                    "DisplayName": "Warped Stairs"
                },
                "minecraft:ice": {
                    "Color": "#8071A0F2",
                    "DisplayName": "Ice"
                },
                "minecraft:packed_ice": {
                    "Color": "#71A0F2",
                    "DisplayName": "Packed Ice"
                },
                "minecraft:blue_ice": {
                    "Color": "#5487DC",
                    "DisplayName": "Blue Ice"
                },
                "minecraft:snow_block": {
                    "Color": "#BFC8C8",
                    "DisplayName": "Snow Block"
                },
                "minecraft:pumpkin": {
                    "Color": "#B76D0C",
                    "DisplayName": "Pumpkin"
                },
                "minecraft:carved_pumpkin": {
                    "Color": "#A65C00",
                    "DisplayName": "Carved Pumpkin"
                },
                "minecraft:jack_o_lantern": {
                    "Color": "#C59000",
                    "DisplayName": "Jack O Lantern"
                },
                "minecraft:hay_block": {
                    "Color": "#AB8D02",
                    "DisplayName": "Hay Bale"
                },
                "minecraft:bone_block": {
                    "Color": "#B5B19D",
                    "DisplayName": "Bone Block"
                },
                "minecraft:melon": {
                    "Color": "#8E8D07",
                    "DisplayName": "Melon"
                },
                "minecraft:bookshelf": {
                    "Color": "#654625",
                    "DisplayName": "Bookshelf"
                },
                "minecraft:oak_sapling": {
                    "Color": "#607F0E",
                    "DisplayName": "Oak Sapling"
                },
                "minecraft:spruce_sapling": {
                    "Color": "#607F0E",
                    "DisplayName": "Spruce Sapling"
                },
                "minecraft:birch_sapling": {
                    "Color": "#607F0E",
                    "DisplayName": "Birch Sapling"
                },
                "minecraft:jungle_sapling": {
                    "Color": "#607F0E",
                    "DisplayName": "Jungle Sapling"
                },
                "minecraft:acacia_sapling": {
                    "Color": "#607F0E",
                    "DisplayName": "Acacia Sapling"
                },
                "minecraft:dark_oak_sapling": {
                    "Color": "#607F0E",
                    "DisplayName": "Dark Oak Sapling"
                },
                "minecraft:oak_leaves": {
                    "Color": "#337814",
                    "DisplayName": " Oak Leaves"
                },
                "minecraft:spruce_leaves": {
                    "Color": "#2C512C",
                    "DisplayName": " Spruce Leaves"
                },
                "minecraft:birch_leaves": {
                    "Color": "#52742D",
                    "DisplayName": " Birch Leaves"
                },
                "minecraft:jungle_leaves": {
                    "Color": "#2C9300",
                    "DisplayName": " Jungle Leaves"
                },
                "minecraft:acacia_leaves": {
                    "Color": "#268300",
                    "DisplayName": " Acacia Leaves"
                },
                "minecraft:dark_oak_leaves": {
                    "Color": "#1B5E00",
                    "DisplayName": " Dark Oak Leaves"
                },
                "minecraft:azalea_leaves": {
                    "Color": "#486117",
                    "DisplayName": " Azalea Leaves"
                },
                "minecraft:flowering_azalea_leaves": {
                    "Color": "#555C3C",
                    "DisplayName": "Flowering Azalea Leaves"
                },
                "minecraft:cobweb": {
                    "Color": "#DEDEDE",
                    "DisplayName": "Cobweb"
                },
                "minecraft:grass": {
                    "Color": "#81B24C",
                    "DisplayName": "Grass"
                },
                "minecraft:fern": {
                    "Color": "#557C2B",
                    "DisplayName": "Fern"
                },
                "minecraft:dead_bush": {
                    "Color": "#7E4E12",
                    "DisplayName": "Dead Bush"
                },
                "minecraft:seagrass": {
                    "Color": "#7E4E12",
                    "DisplayName": "Seagrass"
                },
                "minecraft:tall_seagrass": {
                    "Color": "#7E4E12",
                    "DisplayName": "Tall Seagrass"
                },
                "minecraft:sea_pickle": {
                    "Color": "#6B7129",
                    "DisplayName": "Sea Pickle"
                },
                "minecraft:dandelion": {
                    "Color": "#C6D000",
                    "DisplayName": "Dandelion"
                },
                "minecraft:poppy": {
                    "Color": "#BE0A00",
                    "DisplayName": "Poppy"
                },
                "minecraft:blue_orchid": {
                    "Color": "#098EDC",
                    "DisplayName": "Blue Orchid"
                },
                "minecraft:allium": {
                    "Color": "#9946DC",
                    "DisplayName": "Allium"
                },
                "minecraft:azure_bluet": {
                    "Color": "#B2B8C0",
                    "DisplayName": "Azure Bluet"
                },
                "minecraft:red_tulip": {
                    "Color": "#AF1800",
                    "DisplayName": "Red Tulip"
                },
                "minecraft:orange_tulip": {
                    "Color": "#C35305",
                    "DisplayName": "Orange Tulip"
                },
                "minecraft:white_tulip": {
                    "Color": "#B6B6B6",
                    "DisplayName": "White Tulip"
                },
                "minecraft:pink_tulip": {
                    "Color": "#BD91BD",
                    "DisplayName": "Pink Tulip"
                },
                "minecraft:cornflower": {
                    "Color": "#617FE5",
                    "DisplayName": "Cornflower"
                },
                "minecraft:lily_of_the_valley": {
                    "Color": "#E4E4E4",
                    "DisplayName": "Lily of The Valley"
                },
                "minecraft:wither_rose": {
                    "Color": "#23270F",
                    "DisplayName": "Wither Rose"
                },
                "minecraft:sunflower": {
                    "Color": "#DCC511",
                    "DisplayName": "Sunflower"
                },
                "minecraft:lilac": {
                    "Color": "#957899",
                    "DisplayName": "Lilac"
                },
                "minecraft:rose_bush": {
                    "Color": "#E10E00",
                    "DisplayName": "Rose Bush"
                },
                "minecraft:peony": {
                    "Color": "#B992CA",
                    "DisplayName": "Peony"
                },
                "minecraft:tall_grass": {
                    "Color": "#6FA535",
                    "DisplayName": "Tall Grash"
                },
                "minecraft:large_fern": {
                    "Color": "#6FA535",
                    "DisplayName": "Large Fern"
                },
                "minecraft:azalea": {
                    "Color": "#5F7628",
                    "DisplayName": "Azalea"
                },
                "minecraft:flowering_azalea": {
                    "Color": "#5F7628",
                    "DisplayName": "Flowering Azalea"
                },
                "minecraft:spore_blossom": {
                    "Color": "#5F7628",
                    "DisplayName": "Spore Blossom"
                },
                "minecraft:brown_mushroom": {
                    "Color": "#5F4533",
                    "DisplayName": "Brown Mushroom"
                },
                "minecraft:red_mushroom ": {
                    "Color": "#9B0D0B",
                    "DisplayName": "Red Mushroom"
                },
                "minecraft:crimson_fungus": {
                    "Color": "#8B2D19",
                    "DisplayName": "Crimson Fungus"
                },
                "minecraft:warped_fungus": {
                    "Color": "#007969",
                    "DisplayName": "Warped Fungus"
                },
                "minecraft:crimson_root": {
                    "Color": "#8B2D19",
                    "DisplayName": "Crimson Root"
                },
                "minecraft:warped_roots": {
                    "Color": "#007969",
                    "DisplayName": "Warped Root"
                },
                "minecraft:nether_sprouts": {
                    "Color": "#007969",
                    "DisplayName": "Nether Sprouts"
                },
                "minecraft:weeping_vines": {
                    "Color": "#8B2D19",
                    "DisplayName": "Weeping Vines"
                },
                "minecraft:twisting_vines": {
                    "Color": "#007969",
                    "DisplayName": "Twisting Vines"
                },
                "minecraft:vine": {
                    "Color": "#356600",
                    "DisplayName": "Vine"
                },
                "minecraft:glow_lichen": {
                    "Color": "#586A60",
                    "DisplayName": "Glow Lichen"
                },
                "minecraft:lily_pad": {
                    "Color": "#1A782A",
                    "DisplayName": "Lily Pad"
                },
                "minecraft:sugar_cane": {
                    "Color": "#698B45",
                    "DisplayName": "Sugar Cane"
                },
                "minecraft:kelp": {
                    "Color": "#508926",
                    "DisplayName": "Kelp"
                },
                "minecraft:moss_block": {
                    "Color": "#546827",
                    "DisplayName": "Moss Block"
                },
                "minecraft:hanging_roots": {
                    "Color": "#895C47",
                    "DisplayName": "Hanging Roots"
                },
                "minecraft:big_dripleaf": {
                    "Color": "#59781A",
                    "DisplayName": "Big Dripleaf"
                },
                "minecraft:small_dripleaf": {
                    "Color": "#59781A",
                    "DisplayName": "Small Dripleaf"
                },
                "minecraft:bamboo": {
                    "Color": "#607F0E",
                    "DisplayName": "Bamboo"
                },
                "minecraft:torch": {
                    "Color": "#EDED00",
                    "DisplayName": "Torch"
                },
                "minecraft:end_rod": {
                    "Color": "#CACACA",
                    "DisplayName": "End Rod"
                },
                "minecraft:chorus_plant": {
                    "Color": "#AF77E5",
                    "DisplayName": "Chorus Plant"
                },
                "minecraft:chorus_flower": {
                    "Color": "#AF77E5",
                    "DisplayName": "Chorus Flower"
                },
                "minecraft:chest": {
                    "Color": "#98671B",
                    "DisplayName": "Chest"
                },
                "minecraft:crafting_table": {
                    "Color": "#7B4D2B",
                    "DisplayName": "Crafting Table"
                },
                "minecraft:furnace": {
                    "Color": "#5E5E5F",
                    "DisplayName": "Furnace"
                },
                "minecraft:farmland": {
                    "Color": "#512B10",
                    "DisplayName": "Farmland"
                },
                "minecraft:dirt_path": {
                    "Color": "#7D642E",
                    "DisplayName": "Dirt Path"
                },
                "minecraft:ladder": {
                    "Color": "#7B4D2B",
                    "DisplayName": "Ladder"
                },
                "minecraft:snow": {
                    "Color": "#DFE9E9",
                    "DisplayName": "Snow"
                },
                "minecraft:cactus": {
                    "Color": "#085C13",
                    "DisplayName": "Cactus"
                },
                "minecraft:jukebox": {
                    "Color": "#7B4D2B",
                    "DisplayName": "Jukebox"
                },
                "minecraft:oak_fence": {
                    "Color": "#937840",
                    "DisplayName": "Oak Fence"
                },
                "minecraft:spruce_fence": {
                    "Color": "#654523",
                    "DisplayName": "Spruce Fence"
                },
                "minecraft:birch_fence": {
                    "Color": "#B09A5E",
                    "DisplayName": "Birch Fence"
                },
                "minecraft:jungle_fence": {
                    "Color": "#956440",
                    "DisplayName": "Jungle Fence"
                },
                "minecraft:acacia_fence": {
                    "Color": "#A14F22",
                    "DisplayName": "Acacia Fence"
                },
                "minecraft:dark_oak_fence": {
                    "Color": "#3E240C",
                    "DisplayName": "Dark Fence"
                },
                "minecraft:crimson_fence": {
                    "Color": "#5B243B",
                    "DisplayName": "Crimson Fence"
                },
                "minecraft:warped_fence": {
                    "Color": "#1B5B56",
                    "DisplayName": "Warped Fence"
                },
                "minecraft:infested_stone": {
                    "Color": "#737373",
                    "DisplayName": "Infested Stone"
                },
                "minecraft:infested_cobblestone": {
                    "Color": "#797979",
                    "DisplayName": "Infested Cobblestone"
                },
                "minecraft:infested_stone_bricks": {
                    "Color": "#606060",
                    "DisplayName": "Infested Stone Bricks"
                },
                "minecraft:infested_mossy_stone_bricks": {
                    "Color": "#4C6056",
                    "DisplayName": "Infested Mossy Stone Bricks"
                },
                "minecraft:infested_cracked_stone_bricks": {
                    "Color": "#606060",
                    "DisplayName": "Infested Cracked Stone Bricks"
                },
                "minecraft:infested_chiseled_stone_bricks": {
                    "Color": "#606060",
                    "DisplayName": "Infested Chiseled Stone Bricks"
                },
                "minecraft:infested_deepslate": {
                    "Color": "#535353",
                    "DisplayName": "Infested Deepslate"
                },
                "minecraft:brown_mushroom_block": {
                    "Color": "#5F4533",
                    "DisplayName": "Brown Mushroom Block"
                },
                "minecraft:red_mushroom_block": {
                    "Color": "#9B0D0B",
                    "DisplayName": "Red Mushroom Block"
                },
                "minecraft:mushroom_stem": {
                    "Color": "#C5BFB2",
                    "DisplayName": "Mushroom Stem"
                },
                "minecraft:iron_bars": {
                    "Color": "#828383",
                    "DisplayName": "Iron Bars"
                },
                "minecraft:chain": {
                    "Color": "#3D4352",
                    "DisplayName": "Chain"
                },
                "minecraft:enchanting_table": {
                    "Color": "#A02828",
                    "DisplayName": "Enchanting Table"
                },
                "minecraft:end_portal_frame": {
                    "Color": "#376559",
                    "DisplayName": "End Portal Frame"
                },
                "minecraft:ender_chest": {
                    "Color": "#253638",
                    "DisplayName": "Ender Chest"
                },
                "minecraft:cobblestone_wall": {
                    "Color": "#797979",
                    "DisplayName": "Cobblestone Wall"
                },
                "minecraft:mossy_cobblestone_wall": {
                    "Color": "#65796B",
                    "DisplayName": "Mossy Cobblestone Wall"
                },
                "minecraft:brick_wall": {
                    "Color": "#83402D",
                    "DisplayName": "Brick Wall"
                },
                "minecraft:stone_brick_wall": {
                    "Color": "#606060",
                    "DisplayName": "Stone Brick Wall"
                },
                "minecraft:mossy_stone_brick_wall": {
                    "Color": "#4C6056",
                    "DisplayName": "Mossy Stone Brick Wall"
                },
                "minecraft:granite_wall": {
                    "Color": "#8D5B48",
                    "DisplayName": "Granite Wall"
                },
                "minecraft:andesite_wall": {
                    "Color": "#656565",
                    "DisplayName": "Andesite Wall"
                },
                "minecraft:diorite_wall": {
                    "Color": "#A0A0A0",
                    "DisplayName": "Diorite Wall"
                },
                "minecraft:nether_brick_wall": {
                    "Color": "#2B1015",
                    "DisplayName": "Nether Brick Wall"
                },
                "minecraft:red_nether_brick_wall": {
                    "Color": "#410103",
                    "DisplayName": "Red Nether Brick Wall"
                },
                "minecraft:blackstone_wall": {
                    "Color": "#2A252C",
                    "DisplayName": "Blackstone Wall"
                },
                "minecraft:polished_blackstone_wall": {
                    "Color": "#2A252C",
                    "DisplayName": "Polished Blackstone Wall"
                },
                "minecraft:polished_blackstone_brick_wall": {
                    "Color": "#2A252C",
                    "DisplayName": "Polished Blackstone Brick Wall"
                },
                "minecraft:cobbled_deepslate_wall": {
                    "Color": "#535353",
                    "DisplayName": "Cobbled Deepslate Wall"
                },
                "minecraft:polished_deepslate_wall": {
                    "Color": "#535353",
                    "DisplayName": "Polished Deepslate Wall"
                },
                "minecraft:deepslate_brick_wall": {
                    "Color": "#4C4C4C",
                    "DisplayName": "Deepslate Brick Wall"
                },
                "minecraft:deepslate_tile_wall": {
                    "Color": "#2D2D2E",
                    "DisplayName": "Deepslate Tile Wall"
                },
                "minecraft:sandstone_wall": {
                    "Color": "#B5AD7B",
                    "DisplayName": "Sandstone Wall"
                },
                "minecraft:red_sandstone_wall": {
                    "Color": "#833906",
                    "DisplayName": "Red Sandstone Wall"
                },
                "minecraft:prismarine_wall": {
                    "Color": "#5DA28C",
                    "DisplayName": "Prismarine Wall"
                },
                "minecraft:end_stone_brick_wall": {
                    "Color": "#CBCD97",
                    "DisplayName": "End Stone Brick Wall"
                },
                "minecraft:anvil": {
                    "Color": "#3B3B3B",
                    "DisplayName": "Anvil"
                },
                "minecraft:chipped_anvil": {
                    "Color": "#3B3B3B",
                    "DisplayName": "Chipped Anvil"
                },
                "minecraft:damaged_anvil": {
                    "Color": "#3B3B3B",
                    "DisplayName": "Damaged Anvil"
                },
                "minecraft:glass_pane": {
                    "Color": "#80A6CED6",
                    "DisplayName": "Glass Pane"
                },
                "minecraft:white_stained_glass_pane": {
                    "Color": "#80E3E3E3",
                    "DisplayName": "White Stained Glass Pane"
                },
                "minecraft:orange_stained_glass_pane": {
                    "Color": "#80E35300",
                    "DisplayName": "Orange Stained Glass Pane"
                },
                "minecraft:magenta_stained_glass_pane": {
                    "Color": "#80BC26AF",
                    "DisplayName": "Magenta Stained Glass Pane"
                },
                "minecraft:light_blue_stained_glass_pane": {
                    "Color": "#801288D0",
                    "DisplayName": "Light Blue Stained Glass Pane"
                },
                "minecraft:yellow_stained_glass_pane": {
                    "Color": "#80E89E00",
                    "DisplayName": "Yellow Stained Glass Pane"
                },
                "minecraft:lime_stained_glass_pane": {
                    "Color": "#8058B600",
                    "DisplayName": "Lime Stained Glass Pane"
                },
                "minecraft:pink_stained_glass_pane": {
                    "Color": "#80D95687",
                    "DisplayName": "Pink Stained Glass Pane"
                },
                "minecraft:gray_stained_glass_pane": {
                    "Color": "#809B9B9B",
                    "DisplayName": "Gray Stained Glass Pane"
                },
                "minecraft:light_gray_stained_glass_pane": {
                    "Color": "#807C7C76",
                    "DisplayName": "Light Gray Stained Glass Pane"
                },
                "minecraft:cyan_stained_glass_pane": {
                    "Color": "#80048EA5",
                    "DisplayName": "Cyan Stained Glass Pane"
                },
                "minecraft:purple_stained_glass_pane": {
                    "Color": "#80701AB6",
                    "DisplayName": "Purple Stained Glass Pane"
                },
                "minecraft:blue_stained_glass_pane": {
                    "Color": "#802F32AF",
                    "DisplayName": "Blue Stained Glass Pane"
                },
                "minecraft:brown_stained_glass_pane": {
                    "Color": "#8077451E",
                    "DisplayName": "Brown Stained Glass Pane"
                },
                "minecraft:green_stained_glass_pane": {
                    "Color": "#80597323",
                    "DisplayName": "Green Stained Glass Pane"
                },
                "minecraft:red_stained_glass_pane": {
                    "Color": "#80AD1D1D",
                    "DisplayName": "Red Stained Glass Pane"
                },
                "minecraft:black_stained_glass_pane": {
                    "Color": "#80424242",
                    "DisplayName": "Black Stained Glass Pane"
                },
                "minecraft:shulker_box": {
                    "Color": "#845984",
                    "DisplayName": "Shulker Box"
                },
                "minecraft:white_shulker_box": {
                    "Color": "#ACB1B2",
                    "DisplayName": "White Shulker Box"
                },
                "minecraft:orange_shulker_box": {
                    "Color": "#D05000",
                    "DisplayName": "Orange Shulker Box"
                },
                "minecraft:magenta_shulker_box": {
                    "Color": "#99228F",
                    "DisplayName": "Magenta Shulker Box"
                },
                "minecraft:light_blue_shulker_box": {
                    "Color": "#1689B8",
                    "DisplayName": "Light Blue Shulker Box"
                },
                "minecraft:yellow_shulker_box": {
                    "Color": "#D29700",
                    "DisplayName": "Yellow Shulker Box"
                },
                "minecraft:lime_shulker_box": {
                    "Color": "#499300",
                    "DisplayName": "Lime Shulker Box"
                },
                "minecraft:pink_shulker_box": {
                    "Color": "#C75B7E",
                    "DisplayName": "Pink Shulker Box"
                },
                "minecraft:gray_shulker_box": {
                    "Color": "#2C2F33",
                    "DisplayName": "Gray Shulker Box"
                },
                "minecraft:light_gray_shulker_box": {
                    "Color": "#65655C",
                    "DisplayName": "Light Gray Shulker Box"
                },
                "minecraft:cyan_shulker_box": {
                    "Color": "#016775",
                    "DisplayName": "Cyan Shulker Box"
                },
                "minecraft:purple_shulker_box": {
                    "Color": "#754A75",
                    "DisplayName": "Purple Shulker Box"
                },
                "minecraft:blue_shulker_box": {
                    "Color": "#202281",
                    "DisplayName": "Blue Shulker Box"
                },
                "minecraft:brown_shulker_box": {
                    "Color": "#5C3315",
                    "DisplayName": "Brown Shulker Box"
                },
                "minecraft:green_shulker_box": {
                    "Color": "#3D530E",
                    "DisplayName": "Green Shulker Box"
                },
                "minecraft:red_shulker_box": {
                    "Color": "#811312",
                    "DisplayName": "Red Shulker Box"
                },
                "minecraft:black_shulker_box": {
                    "Color": "#141418",
                    "DisplayName": "Black Shulker Box"
                },
                "minecraft:white_glazed_terracotta": {
                    "Color": "#C7C7C7",
                    "DisplayName": "White Glazed Terracotta "
                },
                "minecraft:orange_glazed_terracotta": {
                    "Color": "#C74800",
                    "DisplayName": "Orange Glazed Terracotta "
                },
                "minecraft:magenta_glazed_terracotta": {
                    "Color": "#951C8B",
                    "DisplayName": "Magenta Glazed Terracotta "
                },
                "minecraft:light_blue_glazed_terracotta": {
                    "Color": "#0D72B0",
                    "DisplayName": "Light Blue Glazed Terracotta "
                },
                "minecraft:yellow_glazed_terracotta": {
                    "Color": "#CD8B00",
                    "DisplayName": "Yellow Glazed Terracotta "
                },
                "minecraft:lime_glazed_terracotta": {
                    "Color": "#438E00",
                    "DisplayName": "Lime Glazed Terracotta "
                },
                "minecraft:pink_glazed_terracotta": {
                    "Color": "#BA4973",
                    "DisplayName": "Pink Glazed Terracotta "
                },
                "minecraft:gray_glazed_terracotta": {
                    "Color": "#323232",
                    "DisplayName": "Gray Glazed Terracotta "
                },
                "minecraft:light_gray_glazed_terracotta": {
                    "Color": "#646464",
                    "DisplayName": "Light Gray Glazed Terracotta "
                },
                "minecraft:cyan_glazed_terracotta": {
                    "Color": "#026475",
                    "DisplayName": "Cyan Glazed Terracotta "
                },
                "minecraft:purple_glazed_terracotta": {
                    "Color": "#56128E",
                    "DisplayName": "Purple Glazed Terracotta "
                },
                "minecraft:blue_glazed_terracotta": {
                    "Color": "#212383",
                    "DisplayName": "Blue Glazed Terracotta "
                },
                "minecraft:brown_glazed_terracotta": {
                    "Color": "#522E12",
                    "DisplayName": "Brown Glazed Terracotta "
                },
                "minecraft:green_glazed_terracotta": {
                    "Color": "#394B14",
                    "DisplayName": "Green Glazed Terracotta "
                },
                "minecraft:red_glazed_terracotta": {
                    "Color": "#801313",
                    "DisplayName": "Red Glazed Terracotta "
                },
                "minecraft:black_glazed_terracotta": {
                    "Color": "#0D0D0D",
                    "DisplayName": "Black Glazed Terracotta "
                },
                "minecraft:tube_coral": {
                    "Color": "#2C51C4",
                    "DisplayName": "Tube Coral"
                },
                "minecraft:brain_coral": {
                    "Color": "#AD4280",
                    "DisplayName": "Brain Coral"
                },
                "minecraft:bubble_coral": {
                    "Color": "#940C90",
                    "DisplayName": "Bubble Coral"
                },
                "minecraft:fire_coral": {
                    "Color": "#9B171E",
                    "DisplayName": "Fire Coral"
                },
                "minecraft:horn_coral": {
                    "Color": "#AD991B",
                    "DisplayName": "Horn Coral"
                },
                "minecraft:dead_tube_coral": {
                    "Color": "#6B645F",
                    "DisplayName": "Dead Tube Coral"
                },
                "minecraft:dead_brain_coral": {
                    "Color": "#6B645F",
                    "DisplayName": "Dead Brain Coral"
                },
                "minecraft:dead_bubble_coral": {
                    "Color": "#6B645F",
                    "DisplayName": "Dead Bubble Coral"
                },
                "minecraft:dead_fire_coral": {
                    "Color": "#6B645F",
                    "DisplayName": "Dead Fire Coral"
                },
                "minecraft:dead_horn_coral": {
                    "Color": "#6B645F",
                    "DisplayName": "Dead Horn Coral"
                },
                "minecraft:tube_coral_fan": {
                    "Color": "#2C51C4",
                    "DisplayName": "Tube Coral Fan"
                },
                "minecraft:brain_coral_fan": {
                    "Color": "#AD4280",
                    "DisplayName": "Brain Coral Fan"
                },
                "minecraft:bubble_coral_fan": {
                    "Color": "#940C90",
                    "DisplayName": "Bubble Coral Fan"
                },
                "minecraft:fire_coral_fan": {
                    "Color": "#9B171E",
                    "DisplayName": "Fire Coral Fan"
                },
                "minecraft:horn_coral_fan": {
                    "Color": "#AD991B",
                    "DisplayName": "Horn Coral Fan"
                },
                "minecraft:dead_tube_coral_fan": {
                    "Color": "#6B645F",
                    "DisplayName": "Dead Tube Coral Fan"
                },
                "minecraft:dead_brain_coral_fan": {
                    "Color": "#6B645F",
                    "DisplayName": "Dead Brain Coral Fan"
                },
                "minecraft:dead_bubble_coral_fan": {
                    "Color": "#6B645F",
                    "DisplayName": "Dead Bubble Coral Fan"
                },
                "minecraft:dead_fire_coral_fan": {
                    "Color": "#6B645F",
                    "DisplayName": "Dead Fire Coral Fan"
                },
                "minecraft:dead_horn_coral_fan": {
                    "Color": "#6B645F",
                    "DisplayName": "Dead Horn Coral Fan"
                },
                "minecraft:oak_sign": {
                    "Color": "#937840",
                    "DisplayName": "Oak Sign"
                },
                "minecraft:spruce_sign": {
                    "Color": "#654523",
                    "DisplayName": "Spruce Sign"
                },
                "minecraft:birch_sign": {
                    "Color": "#B09A5E",
                    "DisplayName": "Birch Sign"
                },
                "minecraft:jungle_sign": {
                    "Color": "#956440",
                    "DisplayName": "Jungle Sign"
                },
                "minecraft:acacia_sign": {
                    "Color": "#A14F22",
                    "DisplayName": "Acacia Sign"
                },
                "minecraft:dark_oak_sign": {
                    "Color": "#3E240C",
                    "DisplayName": "Dark Oak Sign"
                },
                "minecraft:crimson_sign": {
                    "Color": "#5B243B",
                    "DisplayName": "Crimson Sign"
                },
                "minecraft:warped_sign": {
                    "Color": "#1B5B56",
                    "DisplayName": "Warped Sign"
                },
                "minecraft:white_bed": {
                    "Color": "#E3E3E3",
                    "DisplayName": "White Bed"
                },
                "minecraft:orange_bed": {
                    "Color": "#E35300",
                    "DisplayName": "Orange Bed"
                },
                "minecraft:magenta_bed": {
                    "Color": "#BC26AF",
                    "DisplayName": "Magenta Bed"
                },
                "minecraft:light_blue_bed": {
                    "Color": "#1288D0",
                    "DisplayName": "Light Blue Bed"
                },
                "minecraft:yellow_bed": {
                    "Color": "#E89E00",
                    "DisplayName": "Yellow Bed"
                },
                "minecraft:lime_bed": {
                    "Color": "#58B600",
                    "DisplayName": "Lime Bed"
                },
                "minecraft:pink_bed": {
                    "Color": "#D95687",
                    "DisplayName": "Pink Bed"
                },
                "minecraft:gray_bed": {
                    "Color": "#9B9B9B",
                    "DisplayName": "Gray Bed"
                },
                "minecraft:light_gray_bed": {
                    "Color": "#7C7C76",
                    "DisplayName": "Light Gray Bed"
                },
                "minecraft:cyan_bed": {
                    "Color": "#048EA5",
                    "DisplayName": "Cyan Bed"
                },
                "minecraft:purple_bed": {
                    "Color": "#701AB6",
                    "DisplayName": "Purple Bed"
                },
                "minecraft:blue_bed": {
                    "Color": "#2F32AF",
                    "DisplayName": "Blue Bed"
                },
                "minecraft:brown_bed": {
                    "Color": "#77451E",
                    "DisplayName": "Brown Bed"
                },
                "minecraft:green_bed": {
                    "Color": "#597323",
                    "DisplayName": "Green Bed"
                },
                "minecraft:red_bed": {
                    "Color": "#AD1D1D",
                    "DisplayName": "Red Bed"
                },
                "minecraft:black_bed": {
                    "Color": "#424242",
                    "DisplayName": "Black Bed"
                },
                "minecraft:scaffolding": {
                    "Color": "#A9834A",
                    "DisplayName": "Scaffolding"
                },
                "minecraft:flower_pot": {
                    "Color": "#743F31",
                    "DisplayName": "Flower Pot"
                },
                "minecraft:skeleton_head": {
                    "Color": "#A1A1A1",
                    "DisplayName": "Skeleton Head"
                },
                "minecraft:wither_skeleton_head": {
                    "Color": "#A1A1A1",
                    "DisplayName": "Wither Skeleton Head"
                },
                "minecraft:player_head": {
                    "Color": "#A1A1A1",
                    "DisplayName": "Player Head"
                },
                "minecraft:zombie_head": {
                    "Color": "#A1A1A1",
                    "DisplayName": "Zombie Head"
                },
                "minecraft:creeper_head": {
                    "Color": "#A1A1A1",
                    "DisplayName": "Creeper Head"
                },
                "minecraft:dragon_head": {
                    "Color": "#A1A1A1",
                    "DisplayName": "Dragon Head"
                },
                "minecraft:white_banner": {
                    "Color": "#E3E3E3",
                    "DisplayName": "White Banner"
                },
                "minecraft:orange_banner": {
                    "Color": "#E35300",
                    "DisplayName": "Orange Banner"
                },
                "minecraft:magenta_banner": {
                    "Color": "#BC26AF",
                    "DisplayName": "Magenta Banner"
                },
                "minecraft:light_blue_banner": {
                    "Color": "#1288D0",
                    "DisplayName": "Light Blue Banner"
                },
                "minecraft:yellow_banner": {
                    "Color": "#E89E00",
                    "DisplayName": "Yellow Banner"
                },
                "minecraft:lime_banner": {
                    "Color": "#58B600",
                    "DisplayName": "Lime Banner"
                },
                "minecraft:pink_banner": {
                    "Color": "#D95687",
                    "DisplayName": "Pink Banner"
                },
                "minecraft:gray_banner": {
                    "Color": "#9B9B9B",
                    "DisplayName": "Gray Banner"
                },
                "minecraft:light_gray_banner": {
                    "Color": "#7C7C76",
                    "DisplayName": "Light Gray Banner"
                },
                "minecraft:cyan_banner": {
                    "Color": "#048EA5",
                    "DisplayName": "Cyan Banner"
                },
                "minecraft:purple_banner": {
                    "Color": "#701AB6",
                    "DisplayName": "Purple Banner"
                },
                "minecraft:blue_banner": {
                    "Color": "#2F32AF",
                    "DisplayName": "Blue Banner"
                },
                "minecraft:brown_banner": {
                    "Color": "#77451E",
                    "DisplayName": "Brown Banner"
                },
                "minecraft:green_banner": {
                    "Color": "#597323",
                    "DisplayName": "Green Banner"
                },
                "minecraft:red_banner": {
                    "Color": "#AD1D1D",
                    "DisplayName": "Red Banner"
                },
                "minecraft:black_banner": {
                    "Color": "#424242",
                    "DisplayName": "Black Banner"
                },
                "minecraft:loom": {
                    "Color": "#907962",
                    "DisplayName": "Loom"
                },
                "minecraft:composter": {
                    "Color": "#663B16",
                    "DisplayName": "Composter"
                },
                "minecraft:barrel": {
                    "Color": "#704E25",
                    "DisplayName": "Barrel"
                },
                "minecraft:smoker": {
                    "Color": "#494746",
                    "DisplayName": "Smoker"
                },
                "minecraft:blast_furnace": {
                    "Color": "#424142",
                    "DisplayName": "Blast Furnace"
                },
                "minecraft:cartography_table": {
                    "Color": "#695D55",
                    "DisplayName": "Cartography Table"
                },
                "minecraft:fletching_table": {
                    "Color": "#A39267",
                    "DisplayName": "Fletching Table"
                },
                "minecraft:grindstone": {
                    "Color": "#717171",
                    "DisplayName": "Grindstone"
                },
                "minecraft:smithing_table": {
                    "Color": "#2E2F3C",
                    "DisplayName": "Smithing Table"
                },
                "minecraft:stonecutter": {
                    "Color": "#64605C",
                    "DisplayName": "Stonecutter"
                },
                "minecraft:bell": {
                    "Color": "#BB932B",
                    "DisplayName": "Bell"
                },
                "minecraft:lantern": {
                    "Color": "#E1A153",
                    "DisplayName": "Lantern"
                },
                "minecraft:soul_lantern": {
                    "Color": "#97C3C5",
                    "DisplayName": "Soul Lantern"
                },
                "minecraft:campfire": {
                    "Color": "#D3A556",
                    "DisplayName": "Campfire"
                },
                "minecraft:soul_campfire": {
                    "Color": "#23BEC4",
                    "DisplayName": "Soul Campfire"
                },
                "minecraft:shroomlight": {
                    "Color": "#CF793B",
                    "DisplayName": "Shroomlight"
                },
                "minecraft:bee_nest": {
                    "Color": "#AF8A2E",
                    "DisplayName": "Bee Nest"
                },
                "minecraft:beehive": {
                    "Color": "#96743C",
                    "DisplayName": "Beehive"
                },
                "minecraft:honeycomb_block": {
                    "Color": "#C47913",
                    "DisplayName": "Honeycomb Block"
                },
                "minecraft:lodestone": {
                    "Color": "#636466",
                    "DisplayName": "Lodestone"
                },
                "minecraft:respawn_anchor": {
                    "Color": "#5302B8",
                    "DisplayName": "Respawn Anchor"
                },
                "minecraft:beacon": {
                    "Color": "#85D4BB",
                    "DisplayName": "Beacon"
                },
                "minecraft:water": {
                    "Color": "#22417F",
                    "DisplayName": "Water"
                },
                "minecraft:bubble_column": {
                    "Color": "#22417F",
                    "DisplayName": "Bubble Column"
                },
                "minecraft:lava": {
                    "Color": "#CC4600",
                    "DisplayName": "Lava"
                }
            }
        }
        
        """;

        return JsonSerializer.Deserialize<ViewportDefinition>(input)!;
    }
}

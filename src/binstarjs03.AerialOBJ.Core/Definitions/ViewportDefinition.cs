using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace binstarjs03.AerialOBJ.Core.Definitions;

[JsonConverter(typeof(ViewportDefinitionConverter))]
public class ViewportDefinition : IRootDefinition
{
    static ViewportDefinition()
    {
        DefaultDefinition = GetDefaultDefinition();
    }

    public static ViewportDefinition DefaultDefinition { get; }

    public required string Name { get; set; }
    public required int FormatVersion { get; set; }
    public required string MinecraftVersion { get; set; }
    public string? OriginalFilename { get; set; }
    public bool IsDefault { get; private set; }

    public required string AirBlockName { get; set; }
    public required ViewportBlockDefinition MissingBlockDefinition { get; set; }

    [JsonConverter(typeof(ViewportBlockDefinitionsConverter))]
    public required Dictionary<string, ViewportBlockDefinition> BlockDefinitions { get; set; }

    private static ViewportDefinition GetDefaultDefinition()
    {
        string input = $$"""
        {
            "Name": "Default Viewport Definition",
            "Kind": "{{DefinitionKinds.Viewport}}",
            "FormatVersion": 1,
            "MinecraftVersion": "1.18",
            "AirBlockName": "minecraft:air",
            "MissingBlockDefinition": {
                "Name": "",
                "Color": "#FF00CC",
                "DisplayName": "Unknown (Missing Definition)"
            },
            "BlockDefinitions": [
                {
                    "Name": "minecraft:air",
                    "Color": "#000000",
                    "Alpha": 0,
                    "IsSolid": false,
                    "IsExcluded": true
                },
                {
                    "Name": "minecraft:cave_air",
                    "Color": "#000000",
                    "Alpha": 0,
                    "DisplayName": "Air (Cave)",
                    "IsSolid": false,
                    "IsExcluded": true
                },
                {
                    "Name": "minecraft:stone",
                    "Color": "#737373"
                },
                {
                    "Name": "minecraft:stone_bricks",
                    "Color": "#606060"
                },
                {
                    "Name": "minecraft:mossy_stone_bricks",
                    "Color": "#4C6056"
                },
                {
                    "Name": "minecraft:cracked_stone_bricks",
                    "Color": "#606060"
                },
                {
                    "Name": "minecraft:chiseled_stone_bricks",
                    "Color": "#606060"
                },
                {
                    "Name": "minecraft:smooth_stone",
                    "Color": "#808080"
                },
                {
                    "Name": "minecraft:cobblestone",
                    "Color": "#808080"
                },
                {
                    "Name": "minecraft:mossy_cobblestone",
                    "Color": "#808080"
                },
                {
                    "Name": "minecraft:granite",
                    "Color": "#8D5B48"
                },
                {
                    "Name": "minecraft:andesite",
                    "Color": "#656565"
                },
                {
                    "Name": "minecraft:diorite",
                    "Color": "#A0A0A0"
                },
                {
                    "Name": "minecraft:polished_granite",
                    "Color": "#8D5B48"
                },
                {
                    "Name": "minecraft:polished_andesite",
                    "Color": "#656565"
                },
                {
                    "Name": "minecraft:polished_diorite",
                    "Color": "#A0A0A0"
                },
                {
                    "Name": "minecraft:bedrock",
                    "Color": "#444444"
                },
                {
                    "Name": "minecraft:calcite",
                    "Color": "#B3B3B3"
                },
                {
                    "Name": "minecraft:tuff",
                    "Color": "#585852"
                },
                {
                    "Name": "minecraft:dripstone_block",
                    "Color": "#705646"
                },
                {
                    "Name": "minecraft:gravel",
                    "Color": "#6B6161"
                },
                {
                    "Name": "minecraft:clay",
                    "Color": "#808592"
                },
                {
                    "Name": "minecraft:bricks",
                    "Color": "#83402D"
                },
                {
                    "Name": "minecraft:obsidian",
                    "Color": "#151123"
                },
                {
                    "Name": "minecraft:crying_obsidian",
                    "Color": "#2A0954"
                },
                {
                    "Name": "minecraft:deepslate",
                    "Color": "#535353"
                },
                {
                    "Name": "minecraft:cobbled_deepslate",
                    "Color": "#535353"
                },
                {
                    "Name": "minecraft:polished_deepslate",
                    "Color": "#535353"
                },
                {
                    "Name": "minecraft:deepslate_bricks",
                    "Color": "#4C4C4C"
                },
                {
                    "Name": "minecraft:cracked_deepslate_bricks",
                    "Color": "#4C4C4C"
                },
                {
                    "Name": "minecraft:deepslate_tiles",
                    "Color": "#2D2D2D"
                },
                {
                    "Name": "minecraft:cracked_deepslate_tiles",
                    "Color": "#2D2D2D"
                },
                {
                    "Name": "minecraft:chiseled_deepslate_tiles",
                    "Color": "#2D2D2D"
                },
                {
                    "Name": "minecraft:amethyst_block",
                    "Color": "#6E4F9C"
                },
                {
                    "Name": "minecraft:budding_amethyst",
                    "Color": "#6E4F9C"
                },
                {
                    "Name": "minecraft:netherrack",
                    "Color": "#6D3533"
                },
                {
                    "Name": "minecraft:soul_sand",
                    "Color": "#473326"
                },
                {
                    "Name": "minecraft:soul_soil",
                    "Color": "#473326"
                },
                {
                    "Name": "minecraft:crimson_nylium",
                    "Color": "#771919"
                },
                {
                    "Name": "minecraft:warped_nylium",
                    "Color": "#206154"
                },
                {
                    "Name": "minecraft:magma_block",
                    "Color": "#893A09"
                },
                {
                    "Name": "minecraft:glowstone",
                    "Color": "#CDA870"
                },
                {
                    "Name": "minecraft:basalt",
                    "Color": "#505050"
                },
                {
                    "Name": "minecraft:polished_basalt",
                    "Color": "#525252"
                },
                {
                    "Name": "minecraft:smooth_basalt",
                    "Color": "#3B3B3B"
                },
                {
                    "Name": "minecraft:nether_bricks",
                    "Color": "#2B1010"
                },
                {
                    "Name": "minecraft:cracked_nether_bricks",
                    "Color": "#2B1010"
                },
                {
                    "Name": "minecraft:chiseled_nether_bricks",
                    "Color": "#2B1010"
                },
                {
                    "Name": "minecraft:red_nether_bricks",
                    "Color": "#410101"
                },
                {
                    "Name": "minecraft:blackstone",
                    "Color": "#2A252C"
                },
                {
                    "Name": "minecraft:giled_blackstone",
                    "Color": "#2A252C"
                },
                {
                    "Name": "minecraft:polished_blackstone",
                    "Color": "#2A252C"
                },
                {
                    "Name": "minecraft:chiseled_polished_blackstone",
                    "Color": "#3E3641"
                },
                {
                    "Name": "minecraft:polished_blackstone_bricks",
                    "Color": "#3E3641"
                },
                {
                    "Name": "minecraft:cracked_polished_blackstone_bricks",
                    "Color": "#3E3641"
                },
                {
                    "Name": "minecraft:end_stone",
                    "Color": "#CBCD97"
                },
                {
                    "Name": "minecraft:end_stone_bricks",
                    "Color": "#CBCD97"
                },
                {
                    "Name": "minecraft:purpur_block",
                    "Color": "#8A5E8A"
                },
                {
                    "Name": "minecraft:purpur_pillar",
                    "Color": "#8A5E8A"
                },
                {
                    "Name": "minecraft:quartz_block",
                    "Color": "#D1CEC8"
                },
                {
                    "Name": "minecraft:quartz_bricks",
                    "Color": "#B3B0AA"
                },
                {
                    "Name": "minecraft:quartz_pillar",
                    "Color": "#B3B0AA"
                },
                {
                    "Name": "minecraft:chiseled_quartz_block",
                    "Color": "#B3B0AA"
                },
                {
                    "Name": "minecraft:smooth_quartz",
                    "Color": "#D1CEC8"
                },
                {
                    "Name": "minecraft:sand",
                    "Color": "#CEC298"
                },
                {
                    "Name": "minecraft:sandstone",
                    "Color": "#B5AD7B"
                },
                {
                    "Name": "minecraft:smooth_sandstone",
                    "Color": "#B5AD7B"
                },
                {
                    "Name": "minecraft:cut_sandstone",
                    "Color": "#B5AD7B"
                },
                {
                    "Name": "minecraft:chiseled_sandstone",
                    "Color": "#B5AD7B"
                },
                {
                    "Name": "minecraft:red_sand",
                    "Color": "#933F0B"
                },
                {
                    "Name": "minecraft:red_sandstone",
                    "Color": "#833906"
                },
                {
                    "Name": "minecraft:smooth_red_sandstone",
                    "Color": "#833906"
                },
                {
                    "Name": "minecraft:cut_red_sandstone",
                    "Color": "#833906"
                },
                {
                    "Name": "minecraft:chiseled_red_sandstone",
                    "Color": "#833906"
                },
                {
                    "Name": "minecraft:grass_block",
                    "Color": "#5D923A"
                },
                {
                    "Name": "minecraft:dirt",
                    "Color": "#845B3C"
                },
                {
                    "Name": "minecraft:coarse_dirt",
                    "Color": "#69462B"
                },
                {
                    "Name": "minecraft:rooted_dirt",
                    "Color": "#69462B"
                },
                {
                    "Name": "minecraft:podzol",
                    "Color": "#51330A"
                },
                {
                    "Name": "minecraft:mycelium",
                    "Color": "#544955"
                },
                {
                    "Name": "minecraft:prismarine",
                    "Color": "#5DA28C"
                },
                {
                    "Name": "minecraft:prismarine_bricks",
                    "Color": "#78B5A6"
                },
                {
                    "Name": "minecraft:dark_prismarine",
                    "Color": "#244E3D"
                },
                {
                    "Name": "minecraft:sea_lantern",
                    "Color": "#A7AFA7"
                },
                {
                    "Name": "minecraft:sponge",
                    "Color": "#AAAB27"
                },
                {
                    "Name": "minecraft:wet_sponge",
                    "Color": "#7D7C0D"
                },
                {
                    "Name": "minecraft:dried_kelp_block",
                    "Color": "#333726"
                },
                {
                    "Name": "minecraft:coal_ore",
                    "Color": "#303030"
                },
                {
                    "Name": "minecraft:iron_ore",
                    "Color": "#9B785F"
                },
                {
                    "Name": "minecraft:copper_ore",
                    "Color": "#7B542D"
                },
                {
                    "Name": "minecraft:gold_ore",
                    "Color": "#CFC11E"
                },
                {
                    "Name": "minecraft:redstone_ore",
                    "Color": "#860000"
                },
                {
                    "Name": "minecraft:emerald_ore",
                    "Color": "#617265"
                },
                {
                    "Name": "minecraft:lapis_ore",
                    "Color": "#092D75",
                    "DisplayName": "Lapis Lazuli Ore"
                },
                {
                    "Name": "minecraft:diamond_ore",
                    "Color": "#36C5CE"
                },
                {
                    "Name": "minecraft:deepslate_coal_ore",
                    "Color": "#202020"
                },
                {
                    "Name": "minecraft:deepslate_iron_ore",
                    "Color": "#735846"
                },
                {
                    "Name": "minecraft:deepslate_copper_ore",
                    "Color": "#53381E"
                },
                {
                    "Name": "minecraft:deepslate_gold_ore",
                    "Color": "#A79B18"
                },
                {
                    "Name": "minecraft:deepslate_redstone_ore",
                    "Color": "#653E3F"
                },
                {
                    "Name": "minecraft:deepslate_emerald_ore",
                    "Color": "#3F4A41"
                },
                {
                    "Name": "minecraft:deepslate_lapis_ore",
                    "Color": "#051D4D",
                    "DisplayName": "Deepslate Lapis Lazuli Ore"
                },
                {
                    "Name": "minecraft:deepslate_diamond_ore",
                    "Color": "#2BA0A6"
                },
                {
                    "Name": "minecraft:raw_iron_block",
                    "Color": "#A2836A",
                    "DisplayName": "Block of Raw Iron"
                },
                {
                    "Name": "minecraft:raw_copper_block",
                    "Color": "#8A543A",
                    "DisplayName": "Block of Raw Copper"
                },
                {
                    "Name": "minecraft:raw_gold_block",
                    "Color": "#BC8C1A",
                    "DisplayName": "Block of Raw Gold"
                },
                {
                    "Name": "minecraft:coal_block",
                    "Color": "#171717",
                    "DisplayName": "Block of Coal"
                },
                {
                    "Name": "minecraft:iron_block",
                    "Color": "#BEBEBE",
                    "DisplayName": "Block of Iron"
                },
                {
                    "Name": "minecraft:copper_block",
                    "Color": "#A75237",
                    "DisplayName": "Block of Copper"
                },
                {
                    "Name": "minecraft:gold_block",
                    "Color": "#D0C920",
                    "DisplayName": "Block of Gold"
                },
                {
                    "Name": "minecraft:redstone_block",
                    "Color": "#9A1000",
                    "DisplayName": "Block of Redstone"
                },
                {
                    "Name": "minecraft:emerald_block",
                    "Color": "#32B657",
                    "DisplayName": "Block of Emerald"
                },
                {
                    "Name": "minecraft:lapis_block",
                    "Color": "#0B3EAB",
                    "DisplayName": "Block of Lapis Lazuli"
                },
                {
                    "Name": "minecraft:diamond_block",
                    "Color": "#57BBB7",
                    "DisplayName": "Block of Diamond"
                },
                {
                    "Name": "minecraft:netherite_block",
                    "Color": "#363234",
                    "DisplayName": "Block of Netherite"
                },
                {
                    "Name": "minecraft:nether_gold_ore",
                    "Color": "#78482B"
                },
                {
                    "Name": "minecraft:nether_quartz_ore",
                    "Color": "#664743"
                },
                {
                    "Name": "minecraft:ancient_debris",
                    "Color": "#553E38"
                },
                {
                    "Name": "minecraft:exposed_copper",
                    "Color": "#9A7560"
                },
                {
                    "Name": "minecraft:weathered_copper",
                    "Color": "#547853"
                },
                {
                    "Name": "minecraft:oxidized_copper",
                    "Color": "#38896B"
                },
                {
                    "Name": "minecraft:cut_copper",
                    "Color": "#A75237"
                },
                {
                    "Name": "minecraft:exposed_cut_copper",
                    "Color": "#9A7560"
                },
                {
                    "Name": "minecraft:weathered_cut_copper",
                    "Color": "#547853"
                },
                {
                    "Name": "minecraft:oxidized_cut_copper",
                    "Color": "#38896B"
                },
                {
                    "Name": "minecraft:waxed_copper",
                    "Color": "#A75237",
                    "DisplayName": "Waxed Block of Copper"
                },
                {
                    "Name": "minecraft:waxed_exposed_copper",
                    "Color": "#9A7560"
                },
                {
                    "Name": "minecraft:waxed_weathered_copper",
                    "Color": "#547853"
                },
                {
                    "Name": "minecraft:waxed_oxidized_copper",
                    "Color": "#38896B"
                },
                {
                    "Name": "minecraft:waxed_cut_copper",
                    "Color": "#A75237"
                },
                {
                    "Name": "minecraft:waxed_exposed_cut_copper",
                    "Color": "#9A7560"
                },
                {
                    "Name": "minecraft:waxed_weathered_cut_copper",
                    "Color": "#547853"
                },
                {
                    "Name": "minecraft:waxed_oxidized_cut_copper",
                    "Color": "#38896B"
                },
                {
                    "Name": "minecraft:dead_tube_coral_block",
                    "Color": "#6B645F"
                },
                {
                    "Name": "minecraft:dead_brain_coral_block",
                    "Color": "#6B645F"
                },
                {
                    "Name": "minecraft:dead_bubble_coral_block",
                    "Color": "#6B645F"
                },
                {
                    "Name": "minecraft:dead_fire_coral_block",
                    "Color": "#6B645F"
                },
                {
                    "Name": "minecraft:dead_horn_coral_block",
                    "Color": "#6B645F"
                },
                {
                    "Name": "minecraft:tube_coral_block",
                    "Color": "#2C51C4"
                },
                {
                    "Name": "minecraft:brain_coral_block",
                    "Color": "#AD4280"
                },
                {
                    "Name": "minecraft:bubble_coral_block",
                    "Color": "#940C90"
                },
                {
                    "Name": "minecraft:fire_coral_block",
                    "Color": "#9B171E"
                },
                {
                    "Name": "minecraft:horn_coral_block",
                    "Color": "#AD991B"
                },
                {
                    "Name": "minecraft:white_concrete",
                    "Color": "#C7C7C7"
                },
                {
                    "Name": "minecraft:orange_concrete",
                    "Color": "#C74800"
                },
                {
                    "Name": "minecraft:magenta_concrete",
                    "Color": "#951C8B"
                },
                {
                    "Name": "minecraft:light_blue_concrete",
                    "Color": "#0D72B0"
                },
                {
                    "Name": "minecraft:yellow_concrete",
                    "Color": "#CD8B00"
                },
                {
                    "Name": "minecraft:lime_concrete",
                    "Color": "#438E00"
                },
                {
                    "Name": "minecraft:pink_concrete",
                    "Color": "#BA4973"
                },
                {
                    "Name": "minecraft:gray_concrete",
                    "Color": "#323232"
                },
                {
                    "Name": "minecraft:light_gray_concrete",
                    "Color": "#646464"
                },
                {
                    "Name": "minecraft:cyan_concrete",
                    "Color": "#026475"
                },
                {
                    "Name": "minecraft:purple_concrete",
                    "Color": "#56128E"
                },
                {
                    "Name": "minecraft:blue_concrete",
                    "Color": "#212383"
                },
                {
                    "Name": "minecraft:brown_concrete",
                    "Color": "#522E12"
                },
                {
                    "Name": "minecraft:green_concrete",
                    "Color": "#394B14"
                },
                {
                    "Name": "minecraft:red_concrete",
                    "Color": "#801313"
                },
                {
                    "Name": "minecraft:black_concrete",
                    "Color": "#0D0D0D"
                },
                {
                    "Name": "minecraft:white_concrete_powder",
                    "Color": "#E3E3E3"
                },
                {
                    "Name": "minecraft:orange_concrete_powder",
                    "Color": "#E35300"
                },
                {
                    "Name": "minecraft:magenta_concrete_powder",
                    "Color": "#BC26AF"
                },
                {
                    "Name": "minecraft:light_blue_concrete_powder",
                    "Color": "#1288D0"
                },
                {
                    "Name": "minecraft:yellow_concrete_powder",
                    "Color": "#E89E00"
                },
                {
                    "Name": "minecraft:lime_concrete_powder",
                    "Color": "#58B600"
                },
                {
                    "Name": "minecraft:pink_concrete_powder",
                    "Color": "#D95687"
                },
                {
                    "Name": "minecraft:gray_concrete_powder",
                    "Color": "#9B9B9B"
                },
                {
                    "Name": "minecraft:light_gray_concrete_powder",
                    "Color": "#7C7C76"
                },
                {
                    "Name": "minecraft:cyan_concrete_powder",
                    "Color": "#048EA5"
                },
                {
                    "Name": "minecraft:purple_concrete_powder",
                    "Color": "#701AB6"
                },
                {
                    "Name": "minecraft:blue_concrete_powder",
                    "Color": "#2F32AF"
                },
                {
                    "Name": "minecraft:brown_concrete_powder",
                    "Color": "#77451E"
                },
                {
                    "Name": "minecraft:green_concrete_powder",
                    "Color": "#597323"
                },
                {
                    "Name": "minecraft:red_concrete_powder",
                    "Color": "#AD1D1D"
                },
                {
                    "Name": "minecraft:black_concrete_powder",
                    "Color": "#424242"
                },
                {
                    "Name": "minecraft:terracotta",
                    "Color": "#8D533A"
                },
                {
                    "Name": "minecraft:white_terracotta",
                    "Color": "#A98979"
                },
                {
                    "Name": "minecraft:orange_terracotta",
                    "Color": "#893C0D"
                },
                {
                    "Name": "minecraft:magenta_terracotta",
                    "Color": "#7D3F54"
                },
                {
                    "Name": "minecraft:yellow_terracotta",
                    "Color": "#9B6504"
                },
                {
                    "Name": "minecraft:lime_terracotta",
                    "Color": "#4F5D1B"
                },
                {
                    "Name": "minecraft:pink_terracotta",
                    "Color": "#893637"
                },
                {
                    "Name": "minecraft:gray_terracotta",
                    "Color": "#2D1D18"
                },
                {
                    "Name": "minecraft:light_gray_terracotta",
                    "Color": "#6E5047"
                },
                {
                    "Name": "minecraft:cyan_terracotta",
                    "Color": "#414546"
                },
                {
                    "Name": "minecraft:purple_terracotta",
                    "Color": "#623142"
                },
                {
                    "Name": "minecraft:blue_terracotta",
                    "Color": "#3A2B4B"
                },
                {
                    "Name": "minecraft:brown_terracotta",
                    "Color": "#3F2416"
                },
                {
                    "Name": "minecraft:green_terracotta",
                    "Color": "#394018"
                },
                {
                    "Name": "minecraft:red_terracotta",
                    "Color": "#7B291B"
                },
                {
                    "Name": "minecraft:black_terracotta",
                    "Color": "#1C0D08"
                },
                {
                    "Name": "minecraft:white_wool",
                    "Color": "#E3E3E3"
                },
                {
                    "Name": "minecraft:orange_wool",
                    "Color": "#E35300"
                },
                {
                    "Name": "minecraft:magenta_wool",
                    "Color": "#BC26AF"
                },
                {
                    "Name": "minecraft:light_blue_wool",
                    "Color": "#1288D0"
                },
                {
                    "Name": "minecraft:yellow_wool",
                    "Color": "#E89E00"
                },
                {
                    "Name": "minecraft:lime_wool",
                    "Color": "#58B600"
                },
                {
                    "Name": "minecraft:pink_wool",
                    "Color": "#D95687"
                },
                {
                    "Name": "minecraft:gray_wool",
                    "Color": "#9B9B9B"
                },
                {
                    "Name": "minecraft:light_gray_wool",
                    "Color": "#7C7C76"
                },
                {
                    "Name": "minecraft:cyan_wool",
                    "Color": "#048EA5"
                },
                {
                    "Name": "minecraft:purple_wool",
                    "Color": "#701AB6"
                },
                {
                    "Name": "minecraft:blue_wool",
                    "Color": "#2F32AF"
                },
                {
                    "Name": "minecraft:brown_wool",
                    "Color": "#77451E"
                },
                {
                    "Name": "minecraft:green_wool",
                    "Color": "#597323"
                },
                {
                    "Name": "minecraft:red_wool",
                    "Color": "#AD1D1D"
                },
                {
                    "Name": "minecraft:black_wool",
                    "Color": "#424242"
                },
                {
                    "Name": "minecraft:glass",
                    "Color": "#A6CED6",
                    "Alpha": 192
                },
                {
                    "Name": "minecraft:tinted_glass",
                    "Color": "#252523",
                    "Alpha": 192
                },
                {
                    "Name": "minecraft:white_stained_glass",
                    "Color": "#E3E3E3",
                    "Alpha": 192
                },
                {
                    "Name": "minecraft:orange_stained_glass",
                    "Color": "#E35300",
                    "Alpha": 192
                },
                {
                    "Name": "minecraft:magenta_stained_glass",
                    "Color": "#BC26AF",
                    "Alpha": 192
                },
                {
                    "Name": "minecraft:light_blue_stained_glass",
                    "Color": "#1288D0",
                    "Alpha": 192
                },
                {
                    "Name": "minecraft:yellow_stained_glass",
                    "Color": "#E89E00",
                    "Alpha": 192
                },
                {
                    "Name": "minecraft:lime_stained_glass",
                    "Color": "#58B600",
                    "Alpha": 192
                },
                {
                    "Name": "minecraft:pink_stained_glass",
                    "Color": "#D95687",
                    "Alpha": 192
                },
                {
                    "Name": "minecraft:gray_stained_glass",
                    "Color": "#9B9B9B",
                    "Alpha": 192
                },
                {
                    "Name": "minecraft:light_gray_stained_glass",
                    "Color": "#7C7C76",
                    "Alpha": 192
                },
                {
                    "Name": "minecraft:cyan_stained_glass",
                    "Color": "#048EA5",
                    "Alpha": 192
                },
                {
                    "Name": "minecraft:purple_stained_glass",
                    "Color": "#701AB6",
                    "Alpha": 192
                },
                {
                    "Name": "minecraft:blue_stained_glass",
                    "Color": "#2F32AF",
                    "Alpha": 192
                },
                {
                    "Name": "minecraft:brown_stained_glass",
                    "Color": "#77451E",
                    "Alpha": 192
                },
                {
                    "Name": "minecraft:green_stained_glass",
                    "Color": "#597323",
                    "Alpha": 192
                },
                {
                    "Name": "minecraft:red_stained_glass",
                    "Color": "#AD1D1D",
                    "Alpha": 192
                },
                {
                    "Name": "minecraft:black_stained_glass",
                    "Color": "#424242",
                    "Alpha": 192
                },
                {
                    "Name": "minecraft:glass_pane",
                    "Color": "#A6CED6",
                    "Alpha": 192
                },
                {
                    "Name": "minecraft:white_stained_glass_pane",
                    "Color": "#E3E3E3",
                    "Alpha": 192
                },
                {
                    "Name": "minecraft:orange_stained_glass_pane",
                    "Color": "#E35300",
                    "Alpha": 192
                },
                {
                    "Name": "minecraft:magenta_stained_glass_pane",
                    "Color": "#BC26AF",
                    "Alpha": 192
                },
                {
                    "Name": "minecraft:light_blue_stained_glass_pane",
                    "Color": "#1288D0",
                    "Alpha": 192
                },
                {
                    "Name": "minecraft:yellow_stained_glass_pane",
                    "Color": "#E89E00",
                    "Alpha": 192
                },
                {
                    "Name": "minecraft:lime_stained_glass_pane",
                    "Color": "#58B600",
                    "Alpha": 192
                },
                {
                    "Name": "minecraft:pink_stained_glass_pane",
                    "Color": "#D95687",
                    "Alpha": 192
                },
                {
                    "Name": "minecraft:gray_stained_glass_pane",
                    "Color": "#9B9B9B",
                    "Alpha": 192
                },
                {
                    "Name": "minecraft:light_gray_stained_glass_pane",
                    "Color": "#7C7C76",
                    "Alpha": 192
                },
                {
                    "Name": "minecraft:cyan_stained_glass_pane",
                    "Color": "#048EA5",
                    "Alpha": 192
                },
                {
                    "Name": "minecraft:purple_stained_glass_pane",
                    "Color": "#701AB6",
                    "Alpha": 192
                },
                {
                    "Name": "minecraft:blue_stained_glass_pane",
                    "Color": "#2F32AF",
                    "Alpha": 192
                },
                {
                    "Name": "minecraft:brown_stained_glass_pane",
                    "Color": "#77451E",
                    "Alpha": 192
                },
                {
                    "Name": "minecraft:green_stained_glass_pane",
                    "Color": "#597323",
                    "Alpha": 192
                },
                {
                    "Name": "minecraft:red_stained_glass_pane",
                    "Color": "#AD1D1D",
                    "Alpha": 192
                },
                {
                    "Name": "minecraft:black_stained_glass_pane",
                    "Color": "#424242",
                    "Alpha": 192
                },
                {
                    "Name": "minecraft:oak_planks",
                    "Color": "#937840"
                },
                {
                    "Name": "minecraft:spruce_planks",
                    "Color": "#654523"
                },
                {
                    "Name": "minecraft:birch_planks",
                    "Color": "#B09A5E"
                },
                {
                    "Name": "minecraft:jungle_planks",
                    "Color": "#956440"
                },
                {
                    "Name": "minecraft:acacia_planks",
                    "Color": "#A14F22"
                },
                {
                    "Name": "minecraft:dark_oak_planks",
                    "Color": "#3E240C"
                },
                {
                    "Name": "minecraft:crimson_planks",
                    "Color": "#5B243B"
                },
                {
                    "Name": "minecraft:warped_planks",
                    "Color": "#1B5B56"
                },
                {
                    "Name": "minecraft:oak_log",
                    "Color": "#634D2D"
                },
                {
                    "Name": "minecraft:spruce_log",
                    "Color": "#231200"
                },
                {
                    "Name": "minecraft:birch_log",
                    "Color": "#B4BAB1"
                },
                {
                    "Name": "minecraft:jungle_log",
                    "Color": "#4A360B"
                },
                {
                    "Name": "minecraft:acacia_log",
                    "Color": "#4A360B"
                },
                {
                    "Name": "minecraft:dark_oak_log",
                    "Color": "#2B1F0D"
                },
                {
                    "Name": "minecraft:crimson_stem",
                    "Color": "#6B2943"
                },
                {
                    "Name": "minecraft:warped_stem",
                    "Color": "#1F6D69"
                },
                {
                    "Name": "minecraft:stripped_oak_log",
                    "Color": "#937840"
                },
                {
                    "Name": "minecraft:stripped_spruce_log",
                    "Color": "#231200"
                },
                {
                    "Name": "minecraft:stripped_birch_log",
                    "Color": "#B4BAB1"
                },
                {
                    "Name": "minecraft:stripped_jungle_log",
                    "Color": "#4A360B"
                },
                {
                    "Name": "minecraft:stripped_acacia_log",
                    "Color": "#A14F22"
                },
                {
                    "Name": "minecraft:stripped_dark_oak_log",
                    "Color": "#2B1F0D"
                },
                {
                    "Name": "minecraft:stripped_crimson_stem",
                    "Color": "#6B2943"
                },
                {
                    "Name": "minecraft:stripped_warped_stem",
                    "Color": "#1F6D69"
                },
                {
                    "Name": "minecraft:stripped_oak_wood",
                    "Color": "#937840"
                },
                {
                    "Name": "minecraft:stripped_spruce_wood",
                    "Color": "#231200"
                },
                {
                    "Name": "minecraft:stripped_birch_wood",
                    "Color": "#B4BAB1"
                },
                {
                    "Name": "minecraft:stripped_jungle_wood",
                    "Color": "#4A360B"
                },
                {
                    "Name": "minecraft:stripped_acacia_wood",
                    "Color": "#A14F22"
                },
                {
                    "Name": "minecraft:stripped_dark_oak_wood",
                    "Color": "#2B1F0D"
                },
                {
                    "Name": "minecraft:stripped_crimson_hyphae",
                    "Color": "#6B2943"
                },
                {
                    "Name": "minecraft:stripped_warped_hyphae",
                    "Color": "#1F6D69"
                },
                {
                    "Name": "minecraft:oak_wood",
                    "Color": "#634D2D"
                },
                {
                    "Name": "minecraft:spruce_wood",
                    "Color": "#231200"
                },
                {
                    "Name": "minecraft:birch_wood",
                    "Color": "#B4BAB1"
                },
                {
                    "Name": "minecraft:jungle_wood",
                    "Color": "#4A360B"
                },
                {
                    "Name": "minecraft:acacia_wood",
                    "Color": "#4A360B"
                },
                {
                    "Name": "minecraft:dark_oak_wood",
                    "Color": "#2B1F0D"
                },
                {
                    "Name": "minecraft:crimson_hyphae",
                    "Color": "#6B2943"
                },
                {
                    "Name": "minecraft:warped_hyphae",
                    "Color": "#1F6D69"
                },
                {
                    "Name": "minecraft:nether_wart_block",
                    "Color": "#74090A"
                },
                {
                    "Name": "minecraft:warped_wart_block",
                    "Color": "#046767"
                },
                {
                    "Name": "minecraft:stone_slab",
                    "Color": "#737373"
                },
                {
                    "Name": "minecraft:smooth_stone_slab",
                    "Color": "#808080"
                },
                {
                    "Name": "minecraft:stone_brick_slab",
                    "Color": "#606060"
                },
                {
                    "Name": "minecraft:mossy_stone_brick_slab",
                    "Color": "#4C6056"
                },
                {
                    "Name": "minecraft:cobblestone_slab",
                    "Color": "#797979"
                },
                {
                    "Name": "minecraft:granite_slab",
                    "Color": "#8D5B48"
                },
                {
                    "Name": "minecraft:diorite_slab",
                    "Color": "#7B7B7E"
                },
                {
                    "Name": "minecraft:andesite_slab",
                    "Color": "#656569"
                },
                {
                    "Name": "minecraft:polished_granite_slab",
                    "Color": "#8D5B48"
                },
                {
                    "Name": "minecraft:polished_diorite_slab",
                    "Color": "#656565"
                },
                {
                    "Name": "minecraft:polished_andesite_slab",
                    "Color": "#A0A0A0"
                },
                {
                    "Name": "minecraft:sandstone_slab",
                    "Color": "#B5AD7B"
                },
                {
                    "Name": "minecraft:smooth_sandstone_slab",
                    "Color": "#B5AD7B"
                },
                {
                    "Name": "minecraft:cut_sandstone_slab",
                    "Color": "#B5AD7B"
                },
                {
                    "Name": "minecraft:red_sandstone_slab",
                    "Color": "#833906"
                },
                {
                    "Name": "minecraft:smooth_red_sandstone_slab",
                    "Color": "#833906"
                },
                {
                    "Name": "minecraft:cut_red_sandstone_slab",
                    "Color": "#833906"
                },
                {
                    "Name": "minecraft:brick_slab",
                    "Color": "#83402D"
                },
                {
                    "Name": "minecraft:nether_brick_slab",
                    "Color": "#2B1015"
                },
                {
                    "Name": "minecraft:red_nether_brick_slab",
                    "Color": "#410103"
                },
                {
                    "Name": "minecraft:quartz_slab",
                    "Color": "#B3B0AA"
                },
                {
                    "Name": "minecraft:smooth_quartz_slab",
                    "Color": "#B3B0AA"
                },
                {
                    "Name": "minecraft:prismarine_slab",
                    "Color": "#5BA296"
                },
                {
                    "Name": "minecraft:prismarine_brick_slab",
                    "Color": "#4B8778"
                },
                {
                    "Name": "minecraft:dark_prismarine_slab",
                    "Color": "#244E3D"
                },
                {
                    "Name": "minecraft:cobbled_deepslate_slab",
                    "Color": "#3C3C3C"
                },
                {
                    "Name": "minecraft:polished_deepslate_slab",
                    "Color": "#3C3C3C"
                },
                {
                    "Name": "minecraft:deepslate_brick_slab",
                    "Color": "#3C3C3C"
                },
                {
                    "Name": "minecraft:deepslate_tile_slab",
                    "Color": "#3C3C3C"
                },
                {
                    "Name": "minecraft:end_stone_brick_slab",
                    "Color": "#CBCD97"
                },
                {
                    "Name": "minecraft:purpur_slab",
                    "Color": "#8A5E8A"
                },
                {
                    "Name": "minecraft:blackstone_slab",
                    "Color": "#241F26"
                },
                {
                    "Name": "minecraft:polished_blackstone_slab",
                    "Color": "#241F26"
                },
                {
                    "Name": "minecraft:polished_blackstone_brick_slab",
                    "Color": "#241F26"
                },
                {
                    "Name": "minecraft:cut_copper_slab",
                    "Color": "#A75237"
                },
                {
                    "Name": "minecraft:exposed_cut_copper_slab",
                    "Color": "#9A7560"
                },
                {
                    "Name": "minecraft:weathered_cut_copper_slab",
                    "Color": "#547853"
                },
                {
                    "Name": "minecraft:oxidized_cut_copper_slab",
                    "Color": "#38896B"
                },
                {
                    "Name": "minecraft:waxed_cut_copper_slab",
                    "Color": "#A75237"
                },
                {
                    "Name": "minecraft:waxed_exposed_cut_copper_slab",
                    "Color": "#9A7560"
                },
                {
                    "Name": "minecraft:waxed_weathered_cut_copper_slab",
                    "Color": "#547853"
                },
                {
                    "Name": "minecraft:waxed_oxidized_cut_copper_slab",
                    "Color": "#38896B"
                },
                {
                    "Name": "minecraft:petrified_oak_slab",
                    "Color": "#937840"
                },
                {
                    "Name": "minecraft:oak_slab",
                    "Color": "#937840"
                },
                {
                    "Name": "minecraft:spruce_slab",
                    "Color": "#654523"
                },
                {
                    "Name": "minecraft:birch_slab",
                    "Color": "#B09A5E"
                },
                {
                    "Name": "minecraft:jungle_slab",
                    "Color": "#956440"
                },
                {
                    "Name": "minecraft:acacia_slab",
                    "Color": "#A14F22"
                },
                {
                    "Name": "minecraft:dark_oak_slab",
                    "Color": "#3E240C"
                },
                {
                    "Name": "minecraft:crimson_slab",
                    "Color": "#5B243B"
                },
                {
                    "Name": "minecraft:warped_slab",
                    "Color": "#1B5B56"
                },
                {
                    "Name": "minecraft:stone_stairs",
                    "Color": "#737373"
                },
                {
                    "Name": "minecraft:stone_brick_stairs",
                    "Color": "#606060"
                },
                {
                    "Name": "minecraft:mossy_stone_brick_stairs",
                    "Color": "#4C6056"
                },
                {
                    "Name": "minecraft:cobblestone_stairs",
                    "Color": "#797979"
                },
                {
                    "Name": "minecraft:mossy_cobblestone_stairs",
                    "Color": "#65796B"
                },
                {
                    "Name": "minecraft:granite_stairs",
                    "Color": "#8D5B48"
                },
                {
                    "Name": "minecraft:andesite_stairs",
                    "Color": "#656565"
                },
                {
                    "Name": "minecraft:diorite_stairs",
                    "Color": "#A0A0A0"
                },
                {
                    "Name": "minecraft:polished_granite_stairs",
                    "Color": "#8D5B48"
                },
                {
                    "Name": "minecraft:polished_andesite_stairs",
                    "Color": "#656565"
                },
                {
                    "Name": "minecraft:polished_diorite_stairs",
                    "Color": "#A0A0A0"
                },
                {
                    "Name": "minecraft:cobbled_deepslate_stairs",
                    "Color": "#535353"
                },
                {
                    "Name": "minecraft:polished_deepslate_stairs",
                    "Color": "#535353"
                },
                {
                    "Name": "minecraft:deepslate_brick_stairs",
                    "Color": "#4C4C4C"
                },
                {
                    "Name": "minecraft:deepslate_tile_stairs",
                    "Color": "#2D2D2E"
                },
                {
                    "Name": "minecraft:brick_stairs",
                    "Color": "#83402D"
                },
                {
                    "Name": "minecraft:sandstone_stairs",
                    "Color": "#B5AD7B"
                },
                {
                    "Name": "minecraft:smooth_sandstone_stairs",
                    "Color": "#B5AD7B"
                },
                {
                    "Name": "minecraft:red_sandstone_stairs",
                    "Color": "#833906"
                },
                {
                    "Name": "minecraft:smooth_red_sandstone_stairs",
                    "Color": "#833906"
                },
                {
                    "Name": "minecraft:nether_brick_stairs",
                    "Color": "#2B1015"
                },
                {
                    "Name": "minecraft:red_nether_brick_stairs",
                    "Color": "#410103"
                },
                {
                    "Name": "minecraft:quartz_stairs",
                    "Color": "#B3B0AA"
                },
                {
                    "Name": "minecraft:smooth_quartz_stairs",
                    "Color": "#D1CEC8"
                },
                {
                    "Name": "minecraft:blackstone_stairs",
                    "Color": "#2A252C"
                },
                {
                    "Name": "minecraft:polished_blackstone_stairs",
                    "Color": "#2A252C"
                },
                {
                    "Name": "minecraft:polished_blackstone_brick_stairs",
                    "Color": "#3E3641"
                },
                {
                    "Name": "minecraft:end_stone_brick_stairs",
                    "Color": "#CBCD97"
                },
                {
                    "Name": "minecraft:purpur_stairs",
                    "Color": "#8A5E8A"
                },
                {
                    "Name": "minecraft:prismarine_stairs",
                    "Color": "#5DA28C"
                },
                {
                    "Name": "minecraft:prismarine_brick_stairs",
                    "Color": "#78B5A6"
                },
                {
                    "Name": "minecraft:dark_prismarine_stairs",
                    "Color": "#244E3D"
                },
                {
                    "Name": "minecraft:cut_copper_stairs",
                    "Color": "#A75237"
                },
                {
                    "Name": "minecraft:exposed_cut_copper_stairs",
                    "Color": "#9A7560"
                },
                {
                    "Name": "minecraft:weathered_cut_copper_stairs",
                    "Color": "#547853"
                },
                {
                    "Name": "minecraft:oxidized_cut_copper_stairs",
                    "Color": "#38896B"
                },
                {
                    "Name": "minecraft:waxed_cut_copper_stairs",
                    "Color": "#A75237"
                },
                {
                    "Name": "minecraft:waxed_exposed_cut_copper_stairs",
                    "Color": "#9A7560"
                },
                {
                    "Name": "minecraft:waxed_weathered_cut_copper_stairs",
                    "Color": "#547853"
                },
                {
                    "Name": "minecraft:waxed_oxidized_cut_copper_stairs",
                    "Color": "#38896B"
                },
                {
                    "Name": "minecraft:oak_stairs",
                    "Color": "#937840"
                },
                {
                    "Name": "minecraft:acacia_stairs",
                    "Color": "#654523"
                },
                {
                    "Name": "minecraft:dark_oak_stairs",
                    "Color": "#B09A5E"
                },
                {
                    "Name": "minecraft:spruce_stairs",
                    "Color": "#956440"
                },
                {
                    "Name": "minecraft:birch_stairs",
                    "Color": "#A14F22"
                },
                {
                    "Name": "minecraft:jungle_stairs",
                    "Color": "#3E240C"
                },
                {
                    "Name": "minecraft:crimson_stairs",
                    "Color": "#5B243B"
                },
                {
                    "Name": "minecraft:warped_stairs",
                    "Color": "#1B5B56"
                },
                {
                    "Name": "minecraft:ice",
                    "Color": "#71A0F2",
                    "Alpha": 192
                },
                {
                    "Name": "minecraft:packed_ice",
                    "Color": "#71A0F2"
                },
                {
                    "Name": "minecraft:blue_ice",
                    "Color": "#5487DC"
                },
                {
                    "Name": "minecraft:snow_block",
                    "Color": "#BFC8C8"
                },
                {
                    "Name": "minecraft:pumpkin",
                    "Color": "#B76D0C"
                },
                {
                    "Name": "minecraft:carved_pumpkin",
                    "Color": "#A65C00"
                },
                {
                    "Name": "minecraft:jack_o_lantern",
                    "Color": "#C59000"
                },
                {
                    "Name": "minecraft:hay_block",
                    "Color": "#AB8D02"
                },
                {
                    "Name": "minecraft:bone_block",
                    "Color": "#B5B19D"
                },
                {
                    "Name": "minecraft:melon",
                    "Color": "#8E8D07"
                },
                {
                    "Name": "minecraft:bookshelf",
                    "Color": "#654625"
                },
                {
                    "Name": "minecraft:oak_sapling",
                    "Color": "#607F0E",
                    "IsSolid": false
                },
                {
                    "Name": "minecraft:spruce_sapling",
                    "Color": "#607F0E",
                    "IsSolid": false
                },
                {
                    "Name": "minecraft:birch_sapling",
                    "Color": "#607F0E",
                    "IsSolid": false
                },
                {
                    "Name": "minecraft:jungle_sapling",
                    "Color": "#607F0E",
                    "IsSolid": false
                },
                {
                    "Name": "minecraft:acacia_sapling",
                    "Color": "#607F0E",
                    "IsSolid": false
                },
                {
                    "Name": "minecraft:dark_oak_sapling",
                    "Color": "#607F0E",
                    "IsSolid": false
                },
                {
                    "Name": "minecraft:oak_leaves",
                    "Color": "#337814"
                },
                {
                    "Name": "minecraft:spruce_leaves",
                    "Color": "#2C512C"
                },
                {
                    "Name": "minecraft:birch_leaves",
                    "Color": "#52742D"
                },
                {
                    "Name": "minecraft:jungle_leaves",
                    "Color": "#2C9300"
                },
                {
                    "Name": "minecraft:acacia_leaves",
                    "Color": "#268300"
                },
                {
                    "Name": "minecraft:dark_oak_leaves",
                    "Color": "#1B5E00"
                },
                {
                    "Name": "minecraft:azalea_leaves",
                    "Color": "#486117"
                },
                {
                    "Name": "minecraft:flowering_azalea_leaves",
                    "Color": "#555C3C"
                },
                {
                    "Name": "minecraft:cobweb",
                    "Color": "#DEDEDE",
                    "IsSolid": false
                },
                {
                    "Name": "minecraft:grass",
                    "Color": "#5D923A",
                    "IsSolid": false
                },
                {
                    "Name": "minecraft:fern",
                    "Color": "#557C2B",
                    "IsSolid": false
                },
                {
                    "Name": "minecraft:dead_bush",
                    "Color": "#7E4E12",
                    "IsSolid": false
                },
                {
                    "Name": "minecraft:seagrass",
                    "Color": "#8A985D",
                    "IsSolid": false
                },
                {
                    "Name": "minecraft:tall_seagrass",
                    "Color": "#8A985D",
                    "IsSolid": false
                },
                {
                    "Name": "minecraft:sea_pickle",
                    "Color": "#6B7129",
                    "IsSolid": false
                },
                {
                    "Name": "minecraft:dandelion",
                    "Color": "#C6D000",
                    "IsSolid": false
                },
                {
                    "Name": "minecraft:poppy",
                    "Color": "#BE0A00",
                    "IsSolid": false
                },
                {
                    "Name": "minecraft:blue_orchid",
                    "Color": "#098EDC",
                    "IsSolid": false
                },
                {
                    "Name": "minecraft:allium",
                    "Color": "#9946DC",
                    "IsSolid": false
                },
                {
                    "Name": "minecraft:azure_bluet",
                    "Color": "#B2B8C0",
                    "IsSolid": false
                },
                {
                    "Name": "minecraft:red_tulip",
                    "Color": "#AF1800",
                    "IsSolid": false
                },
                {
                    "Name": "minecraft:orange_tulip",
                    "Color": "#C35305",
                    "IsSolid": false
                },
                {
                    "Name": "minecraft:white_tulip",
                    "Color": "#B6B6B6",
                    "IsSolid": false
                },
                {
                    "Name": "minecraft:pink_tulip",
                    "Color": "#BD91BD",
                    "IsSolid": false
                },
                {
                    "Name": "minecraft:cornflower",
                    "Color": "#617FE5",
                    "IsSolid": false
                },
                {
                    "Name": "minecraft:lily_of_the_valley",
                    "Color": "#E4E4E4",
                    "IsSolid": false
                },
                {
                    "Name": "minecraft:wither_rose",
                    "Color": "#23270F",
                    "IsSolid": false
                },
                {
                    "Name": "minecraft:sunflower",
                    "Color": "#DCC511",
                    "IsSolid": false
                },
                {
                    "Name": "minecraft:lilac",
                    "Color": "#957899",
                    "IsSolid": false
                },
                {
                    "Name": "minecraft:rose_bush",
                    "Color": "#E10E00",
                    "IsSolid": false
                },
                {
                    "Name": "minecraft:peony",
                    "Color": "#B992CA",
                    "IsSolid": false
                },
                {
                    "Name": "minecraft:tall_grass",
                    "Color": "#5D923A",
                    "IsSolid": false
                },
                {
                    "Name": "minecraft:large_fern",
                    "Color": "#6FA535",
                    "IsSolid": false
                },
                {
                    "Name": "minecraft:azalea",
                    "Color": "#5F7628"
                },
                {
                    "Name": "minecraft:flowering_azalea",
                    "Color": "#5F7628"
                },
                {
                    "Name": "minecraft:spore_blossom",
                    "Color": "#5F7628"
                },
                {
                    "Name": "minecraft:brown_mushroom",
                    "Color": "#5F4533",
                    "IsSolid": false
                },
                {
                    "Name": "minecraft:red_mushroom",
                    "Color": "#9B0D0B",
                    "IsSolid": false
                },
                {
                    "Name": "minecraft:crimson_fungus",
                    "Color": "#8B2D19",
                    "IsSolid": false
                },
                {
                    "Name": "minecraft:warped_fungus",
                    "Color": "#007969",
                    "IsSolid": false
                },
                {
                    "Name": "minecraft:crimson_root",
                    "Color": "#8B2D19",
                    "IsSolid": false
                },
                {
                    "Name": "minecraft:warped_roots",
                    "Color": "#007969",
                    "IsSolid": false
                },
                {
                    "Name": "minecraft:nether_sprouts",
                    "Color": "#007969",
                    "IsSolid": false
                },
                {
                    "Name": "minecraft:weeping_vines",
                    "Color": "#8B2D19"
                },
                {
                    "Name": "minecraft:twisting_vines",
                    "Color": "#007969"
                },
                {
                    "Name": "minecraft:vine",
                    "Color": "#356600",
                    "IsSolid": false
                },
                {
                    "Name": "minecraft:glow_lichen",
                    "Color": "#586A60",
                    "IsSolid": false
                },
                {
                    "Name": "minecraft:lily_pad",
                    "Color": "#1A782A",
                    "IsSolid": false
                },
                {
                    "Name": "minecraft:sugar_cane",
                    "Color": "#698B45"
                },
                {
                    "Name": "minecraft:kelp",
                    "Color": "#508926"
                },
                {
                    "Name": "minecraft:moss_block",
                    "Color": "#546827"
                },
                {
                    "Name": "minecraft:hanging_roots",
                    "Color": "#895C47"
                },
                {
                    "Name": "minecraft:big_dripleaf",
                    "Color": "#59781A"
                },
                {
                    "Name": "minecraft:small_dripleaf",
                    "Color": "#59781A"
                },
                {
                    "Name": "minecraft:bamboo",
                    "Color": "#607F0E"
                },
                {
                    "Name": "minecraft:torch",
                    "Color": "#EDED00",
                    "IsSolid": false
                },
                {
                    "Name": "minecraft:end_rod",
                    "Color": "#CACACA"
                },
                {
                    "Name": "minecraft:chorus_plant",
                    "Color": "#AF77E5"
                },
                {
                    "Name": "minecraft:chorus_flower",
                    "Color": "#AF77E5"
                },
                {
                    "Name": "minecraft:chest",
                    "Color": "#98671B"
                },
                {
                    "Name": "minecraft:crafting_table",
                    "Color": "#7B4D2B"
                },
                {
                    "Name": "minecraft:furnace",
                    "Color": "#5E5E5F"
                },
                {
                    "Name": "minecraft:farmland",
                    "Color": "#512B10"
                },
                {
                    "Name": "minecraft:dirt_path",
                    "Color": "#7D642E"
                },
                {
                    "Name": "minecraft:ladder",
                    "Color": "#7B4D2B"
                },
                {
                    "Name": "minecraft:snow",
                    "Color": "#DFE9E9"
                },
                {
                    "Name": "minecraft:cactus",
                    "Color": "#085C13"
                },
                {
                    "Name": "minecraft:jukebox",
                    "Color": "#7B4D2B"
                },
                {
                    "Name": "minecraft:oak_fence",
                    "Color": "#937840"
                },
                {
                    "Name": "minecraft:spruce_fence",
                    "Color": "#654523"
                },
                {
                    "Name": "minecraft:birch_fence",
                    "Color": "#B09A5E"
                },
                {
                    "Name": "minecraft:jungle_fence",
                    "Color": "#956440"
                },
                {
                    "Name": "minecraft:acacia_fence",
                    "Color": "#A14F22"
                },
                {
                    "Name": "minecraft:dark_oak_fence",
                    "Color": "#3E240C"
                },
                {
                    "Name": "minecraft:crimson_fence",
                    "Color": "#5B243B"
                },
                {
                    "Name": "minecraft:warped_fence",
                    "Color": "#1B5B56"
                },
                {
                    "Name": "minecraft:infested_stone",
                    "Color": "#737373"
                },
                {
                    "Name": "minecraft:infested_cobblestone",
                    "Color": "#797979"
                },
                {
                    "Name": "minecraft:infested_stone_bricks",
                    "Color": "#606060"
                },
                {
                    "Name": "minecraft:infested_mossy_stone_bricks",
                    "Color": "#4C6056"
                },
                {
                    "Name": "minecraft:infested_cracked_stone_bricks",
                    "Color": "#606060"
                },
                {
                    "Name": "minecraft:infested_chiseled_stone_bricks",
                    "Color": "#606060"
                },
                {
                    "Name": "minecraft:infested_deepslate",
                    "Color": "#535353"
                },
                {
                    "Name": "minecraft:brown_mushroom_block",
                    "Color": "#5F4533"
                },
                {
                    "Name": "minecraft:red_mushroom_block",
                    "Color": "#9B0D0B"
                },
                {
                    "Name": "minecraft:mushroom_stem",
                    "Color": "#C5BFB2"
                },
                {
                    "Name": "minecraft:iron_bars",
                    "Color": "#828383"
                },
                {
                    "Name": "minecraft:chain",
                    "Color": "#3D4352"
                },
                {
                    "Name": "minecraft:enchanting_table",
                    "Color": "#A02828"
                },
                {
                    "Name": "minecraft:end_portal_frame",
                    "Color": "#376559"
                },
                {
                    "Name": "minecraft:ender_chest",
                    "Color": "#253638"
                },
                {
                    "Name": "minecraft:cobblestone_wall",
                    "Color": "#797979"
                },
                {
                    "Name": "minecraft:mossy_cobblestone_wall",
                    "Color": "#65796B"
                },
                {
                    "Name": "minecraft:brick_wall",
                    "Color": "#83402D"
                },
                {
                    "Name": "minecraft:stone_brick_wall",
                    "Color": "#606060"
                },
                {
                    "Name": "minecraft:mossy_stone_brick_wall",
                    "Color": "#4C6056"
                },
                {
                    "Name": "minecraft:granite_wall",
                    "Color": "#8D5B48"
                },
                {
                    "Name": "minecraft:andesite_wall",
                    "Color": "#656565"
                },
                {
                    "Name": "minecraft:diorite_wall",
                    "Color": "#A0A0A0"
                },
                {
                    "Name": "minecraft:nether_brick_wall",
                    "Color": "#2B1015"
                },
                {
                    "Name": "minecraft:red_nether_brick_wall",
                    "Color": "#410103"
                },
                {
                    "Name": "minecraft:blackstone_wall",
                    "Color": "#2A252C"
                },
                {
                    "Name": "minecraft:polished_blackstone_wall",
                    "Color": "#2A252C"
                },
                {
                    "Name": "minecraft:polished_blackstone_brick_wall",
                    "Color": "#2A252C"
                },
                {
                    "Name": "minecraft:cobbled_deepslate_wall",
                    "Color": "#535353"
                },
                {
                    "Name": "minecraft:polished_deepslate_wall",
                    "Color": "#535353"
                },
                {
                    "Name": "minecraft:deepslate_brick_wall",
                    "Color": "#4C4C4C"
                },
                {
                    "Name": "minecraft:deepslate_tile_wall",
                    "Color": "#2D2D2E"
                },
                {
                    "Name": "minecraft:sandstone_wall",
                    "Color": "#B5AD7B"
                },
                {
                    "Name": "minecraft:red_sandstone_wall",
                    "Color": "#833906"
                },
                {
                    "Name": "minecraft:prismarine_wall",
                    "Color": "#5DA28C"
                },
                {
                    "Name": "minecraft:end_stone_brick_wall",
                    "Color": "#CBCD97"
                },
                {
                    "Name": "minecraft:anvil",
                    "Color": "#3B3B3B"
                },
                {
                    "Name": "minecraft:chipped_anvil",
                    "Color": "#3B3B3B"
                },
                {
                    "Name": "minecraft:damaged_anvil",
                    "Color": "#3B3B3B"
                },
                {
                    "Name": "minecraft:shulker_box",
                    "Color": "#845984"
                },
                {
                    "Name": "minecraft:white_shulker_box",
                    "Color": "#ACB1B2"
                },
                {
                    "Name": "minecraft:orange_shulker_box",
                    "Color": "#D05000"
                },
                {
                    "Name": "minecraft:magenta_shulker_box",
                    "Color": "#99228F"
                },
                {
                    "Name": "minecraft:light_blue_shulker_box",
                    "Color": "#1689B8"
                },
                {
                    "Name": "minecraft:yellow_shulker_box",
                    "Color": "#D29700"
                },
                {
                    "Name": "minecraft:lime_shulker_box",
                    "Color": "#499300"
                },
                {
                    "Name": "minecraft:pink_shulker_box",
                    "Color": "#C75B7E"
                },
                {
                    "Name": "minecraft:gray_shulker_box",
                    "Color": "#2C2F33"
                },
                {
                    "Name": "minecraft:light_gray_shulker_box",
                    "Color": "#65655C"
                },
                {
                    "Name": "minecraft:cyan_shulker_box",
                    "Color": "#016775"
                },
                {
                    "Name": "minecraft:purple_shulker_box",
                    "Color": "#754A75"
                },
                {
                    "Name": "minecraft:blue_shulker_box",
                    "Color": "#202281"
                },
                {
                    "Name": "minecraft:brown_shulker_box",
                    "Color": "#5C3315"
                },
                {
                    "Name": "minecraft:green_shulker_box",
                    "Color": "#3D530E"
                },
                {
                    "Name": "minecraft:red_shulker_box",
                    "Color": "#811312"
                },
                {
                    "Name": "minecraft:black_shulker_box",
                    "Color": "#141418"
                },
                {
                    "Name": "minecraft:white_glazed_terracotta",
                    "Color": "#C7C7C7"
                },
                {
                    "Name": "minecraft:orange_glazed_terracotta",
                    "Color": "#C74800"
                },
                {
                    "Name": "minecraft:magenta_glazed_terracotta",
                    "Color": "#951C8B"
                },
                {
                    "Name": "minecraft:light_blue_glazed_terracotta",
                    "Color": "#0D72B0"
                },
                {
                    "Name": "minecraft:yellow_glazed_terracotta",
                    "Color": "#CD8B00"
                },
                {
                    "Name": "minecraft:lime_glazed_terracotta",
                    "Color": "#438E00"
                },
                {
                    "Name": "minecraft:pink_glazed_terracotta",
                    "Color": "#BA4973"
                },
                {
                    "Name": "minecraft:gray_glazed_terracotta",
                    "Color": "#323232"
                },
                {
                    "Name": "minecraft:light_gray_glazed_terracotta",
                    "Color": "#646464"
                },
                {
                    "Name": "minecraft:cyan_glazed_terracotta",
                    "Color": "#026475"
                },
                {
                    "Name": "minecraft:purple_glazed_terracotta",
                    "Color": "#56128E"
                },
                {
                    "Name": "minecraft:blue_glazed_terracotta",
                    "Color": "#212383"
                },
                {
                    "Name": "minecraft:brown_glazed_terracotta",
                    "Color": "#522E12"
                },
                {
                    "Name": "minecraft:green_glazed_terracotta",
                    "Color": "#394B14"
                },
                {
                    "Name": "minecraft:red_glazed_terracotta",
                    "Color": "#801313"
                },
                {
                    "Name": "minecraft:black_glazed_terracotta",
                    "Color": "#0D0D0D"
                },
                {
                    "Name": "minecraft:tube_coral",
                    "Color": "#2C51C4"
                },
                {
                    "Name": "minecraft:brain_coral",
                    "Color": "#AD4280"
                },
                {
                    "Name": "minecraft:bubble_coral",
                    "Color": "#940C90"
                },
                {
                    "Name": "minecraft:fire_coral",
                    "Color": "#9B171E"
                },
                {
                    "Name": "minecraft:horn_coral",
                    "Color": "#AD991B"
                },
                {
                    "Name": "minecraft:dead_tube_coral",
                    "Color": "#6B645F"
                },
                {
                    "Name": "minecraft:dead_brain_coral",
                    "Color": "#6B645F"
                },
                {
                    "Name": "minecraft:dead_bubble_coral",
                    "Color": "#6B645F"
                },
                {
                    "Name": "minecraft:dead_fire_coral",
                    "Color": "#6B645F"
                },
                {
                    "Name": "minecraft:dead_horn_coral",
                    "Color": "#6B645F"
                },
                {
                    "Name": "minecraft:tube_coral_fan",
                    "Color": "#2C51C4"
                },
                {
                    "Name": "minecraft:brain_coral_fan",
                    "Color": "#AD4280"
                },
                {
                    "Name": "minecraft:bubble_coral_fan",
                    "Color": "#940C90"
                },
                {
                    "Name": "minecraft:fire_coral_fan",
                    "Color": "#9B171E"
                },
                {
                    "Name": "minecraft:horn_coral_fan",
                    "Color": "#AD991B"
                },
                {
                    "Name": "minecraft:dead_tube_coral_fan",
                    "Color": "#6B645F"
                },
                {
                    "Name": "minecraft:dead_brain_coral_fan",
                    "Color": "#6B645F"
                },
                {
                    "Name": "minecraft:dead_bubble_coral_fan",
                    "Color": "#6B645F"
                },
                {
                    "Name": "minecraft:dead_fire_coral_fan",
                    "Color": "#6B645F"
                },
                {
                    "Name": "minecraft:dead_horn_coral_fan",
                    "Color": "#6B645F"
                },
                {
                    "Name": "minecraft:oak_sign",
                    "Color": "#937840"
                },
                {
                    "Name": "minecraft:spruce_sign",
                    "Color": "#654523"
                },
                {
                    "Name": "minecraft:birch_sign",
                    "Color": "#B09A5E"
                },
                {
                    "Name": "minecraft:jungle_sign",
                    "Color": "#956440"
                },
                {
                    "Name": "minecraft:acacia_sign",
                    "Color": "#A14F22"
                },
                {
                    "Name": "minecraft:dark_oak_sign",
                    "Color": "#3E240C"
                },
                {
                    "Name": "minecraft:crimson_sign",
                    "Color": "#5B243B"
                },
                {
                    "Name": "minecraft:warped_sign",
                    "Color": "#1B5B56"
                },
                {
                    "Name": "minecraft:white_bed",
                    "Color": "#E3E3E3"
                },
                {
                    "Name": "minecraft:orange_bed",
                    "Color": "#E35300"
                },
                {
                    "Name": "minecraft:magenta_bed",
                    "Color": "#BC26AF"
                },
                {
                    "Name": "minecraft:light_blue_bed",
                    "Color": "#1288D0"
                },
                {
                    "Name": "minecraft:yellow_bed",
                    "Color": "#E89E00"
                },
                {
                    "Name": "minecraft:lime_bed",
                    "Color": "#58B600"
                },
                {
                    "Name": "minecraft:pink_bed",
                    "Color": "#D95687"
                },
                {
                    "Name": "minecraft:gray_bed",
                    "Color": "#9B9B9B"
                },
                {
                    "Name": "minecraft:light_gray_bed",
                    "Color": "#7C7C76"
                },
                {
                    "Name": "minecraft:cyan_bed",
                    "Color": "#048EA5"
                },
                {
                    "Name": "minecraft:purple_bed",
                    "Color": "#701AB6"
                },
                {
                    "Name": "minecraft:blue_bed",
                    "Color": "#2F32AF"
                },
                {
                    "Name": "minecraft:brown_bed",
                    "Color": "#77451E"
                },
                {
                    "Name": "minecraft:green_bed",
                    "Color": "#597323"
                },
                {
                    "Name": "minecraft:red_bed",
                    "Color": "#AD1D1D"
                },
                {
                    "Name": "minecraft:black_bed",
                    "Color": "#424242"
                },
                {
                    "Name": "minecraft:scaffolding",
                    "Color": "#A9834A"
                },
                {
                    "Name": "minecraft:flower_pot",
                    "Color": "#743F31"
                },
                {
                    "Name": "minecraft:skeleton_head",
                    "Color": "#A1A1A1"
                },
                {
                    "Name": "minecraft:wither_skeleton_head",
                    "Color": "#A1A1A1"
                },
                {
                    "Name": "minecraft:player_head",
                    "Color": "#A1A1A1"
                },
                {
                    "Name": "minecraft:zombie_head",
                    "Color": "#A1A1A1"
                },
                {
                    "Name": "minecraft:creeper_head",
                    "Color": "#A1A1A1"
                },
                {
                    "Name": "minecraft:dragon_head",
                    "Color": "#A1A1A1"
                },
                {
                    "Name": "minecraft:white_banner",
                    "Color": "#E3E3E3"
                },
                {
                    "Name": "minecraft:orange_banner",
                    "Color": "#E35300"
                },
                {
                    "Name": "minecraft:magenta_banner",
                    "Color": "#BC26AF"
                },
                {
                    "Name": "minecraft:light_blue_banner",
                    "Color": "#1288D0"
                },
                {
                    "Name": "minecraft:yellow_banner",
                    "Color": "#E89E00"
                },
                {
                    "Name": "minecraft:lime_banner",
                    "Color": "#58B600"
                },
                {
                    "Name": "minecraft:pink_banner",
                    "Color": "#D95687"
                },
                {
                    "Name": "minecraft:gray_banner",
                    "Color": "#9B9B9B"
                },
                {
                    "Name": "minecraft:light_gray_banner",
                    "Color": "#7C7C76"
                },
                {
                    "Name": "minecraft:cyan_banner",
                    "Color": "#048EA5"
                },
                {
                    "Name": "minecraft:purple_banner",
                    "Color": "#701AB6"
                },
                {
                    "Name": "minecraft:blue_banner",
                    "Color": "#2F32AF"
                },
                {
                    "Name": "minecraft:brown_banner",
                    "Color": "#77451E"
                },
                {
                    "Name": "minecraft:green_banner",
                    "Color": "#597323"
                },
                {
                    "Name": "minecraft:red_banner",
                    "Color": "#AD1D1D"
                },
                {
                    "Name": "minecraft:black_banner",
                    "Color": "#424242"
                },
                {
                    "Name": "minecraft:loom",
                    "Color": "#907962"
                },
                {
                    "Name": "minecraft:composter",
                    "Color": "#663B16"
                },
                {
                    "Name": "minecraft:barrel",
                    "Color": "#704E25"
                },
                {
                    "Name": "minecraft:smoker",
                    "Color": "#494746"
                },
                {
                    "Name": "minecraft:blast_furnace",
                    "Color": "#424142"
                },
                {
                    "Name": "minecraft:cartography_table",
                    "Color": "#695D55"
                },
                {
                    "Name": "minecraft:fletching_table",
                    "Color": "#A39267"
                },
                {
                    "Name": "minecraft:grindstone",
                    "Color": "#717171"
                },
                {
                    "Name": "minecraft:smithing_table",
                    "Color": "#2E2F3C"
                },
                {
                    "Name": "minecraft:stonecutter",
                    "Color": "#64605C"
                },
                {
                    "Name": "minecraft:bell",
                    "Color": "#BB932B"
                },
                {
                    "Name": "minecraft:lantern",
                    "Color": "#E1A153"
                },
                {
                    "Name": "minecraft:soul_lantern",
                    "Color": "#97C3C5"
                },
                {
                    "Name": "minecraft:campfire",
                    "Color": "#D3A556"
                },
                {
                    "Name": "minecraft:soul_campfire",
                    "Color": "#23BEC4"
                },
                {
                    "Name": "minecraft:shroomlight",
                    "Color": "#CF793B"
                },
                {
                    "Name": "minecraft:bee_nest",
                    "Color": "#AF8A2E"
                },
                {
                    "Name": "minecraft:beehive",
                    "Color": "#96743C"
                },
                {
                    "Name": "minecraft:honeycomb_block",
                    "Color": "#C47913"
                },
                {
                    "Name": "minecraft:lodestone",
                    "Color": "#636466"
                },
                {
                    "Name": "minecraft:respawn_anchor",
                    "Color": "#5302B8"
                },
                {
                    "Name": "minecraft:beacon",
                    "Color": "#85D4BB"
                },
                {
                    "Name": "minecraft:water",
                    "Color": "#22417F",
                    "Alpha": 125
                },
                {
                    "Name": "minecraft:bubble_column",
                    "Color": "#22417F"
                },
                {
                    "Name": "minecraft:lava",
                    "Color": "#CC4600"
                }
            ]
        }
        """;
        ViewportDefinition result = DefinitionDeserializer.Deserialize<ViewportDefinition>(input);
        result.IsDefault = true;
        return result;
    }
}

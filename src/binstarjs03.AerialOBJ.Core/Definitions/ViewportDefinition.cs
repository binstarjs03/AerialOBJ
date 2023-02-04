using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace binstarjs03.AerialOBJ.Core.Definitions;

[JsonConverter(typeof(ViewportDefinitionConverter))]
public class ViewportDefinition : IRootDefinition
{
    public required string Name { get; set; }
    public required int FormatVersion { get; set; }
    public required string MinecraftVersion { get; set; }
    public required ViewportBlockDefinition MissingBlockDefinition { get; set; }

    [JsonConverter(typeof(ViewportBlockDefinitionsConverter))]
    public required Dictionary<string, ViewportBlockDefinition> BlockDefinitions { get; set; }

    public string? OriginalFilename { get; set; }
    public bool IsDefault { get; private set; }

    public override string ToString()
    {
        return $"{Name}, Format Version: {FormatVersion}, Minecraft Version: {MinecraftVersion}";
    }

    public static ViewportDefinition GetDefaultDefinition()
    {
        string input =  /*lang=json*/ """
        {
            "Name": "Default Viewport Definition",
            "Kind": "Viewport Definition",
            "FormatVersion": 1,
            "MinecraftVersion": "1.18",
            "MissingBlockDefinition": {
                "Namespace": "",
                "Color": "#FF00CC",
                "DisplayName": "Unknown (Missing Definition)"
            },
            "BlockDefinitions": [
                {
                    "Namespace": "minecraft:air",
                    "Color": "#000000",
                    "Alpha": 0,
                    "IsSolid": false,
                    "IsExcluded": true
                },
                {
                    "Namespace": "minecraft:cave_air",
                    "Color": "#000000",
                    "Alpha": 0,
                    "DisplayName": "Air (Cave)",
                    "IsSolid": false,
                    "IsExcluded": true
                },
                {
                    "Namespace": "minecraft:stone",
                    "Color": "#737373"
                },
                {
                    "Namespace": "minecraft:stone_bricks",
                    "Color": "#606060"
                },
                {
                    "Namespace": "minecraft:mossy_stone_bricks",
                    "Color": "#4C6056"
                },
                {
                    "Namespace": "minecraft:cracked_stone_bricks",
                    "Color": "#606060"
                },
                {
                    "Namespace": "minecraft:chiseled_stone_bricks",
                    "Color": "#606060"
                },
                {
                    "Namespace": "minecraft:smooth_stone",
                    "Color": "#808080"
                },
                {
                    "Namespace": "minecraft:cobblestone",
                    "Color": "#808080"
                },
                {
                    "Namespace": "minecraft:mossy_cobblestone",
                    "Color": "#808080"
                },
                {
                    "Namespace": "minecraft:granite",
                    "Color": "#8D5B48"
                },
                {
                    "Namespace": "minecraft:andesite",
                    "Color": "#656565"
                },
                {
                    "Namespace": "minecraft:diorite",
                    "Color": "#A0A0A0"
                },
                {
                    "Namespace": "minecraft:polished_granite",
                    "Color": "#8D5B48"
                },
                {
                    "Namespace": "minecraft:polished_andesite",
                    "Color": "#656565"
                },
                {
                    "Namespace": "minecraft:polished_diorite",
                    "Color": "#A0A0A0"
                },
                {
                    "Namespace": "minecraft:bedrock",
                    "Color": "#444444"
                },
                {
                    "Namespace": "minecraft:calcite",
                    "Color": "#B3B3B3"
                },
                {
                    "Namespace": "minecraft:tuff",
                    "Color": "#585852"
                },
                {
                    "Namespace": "minecraft:dripstone_block",
                    "Color": "#705646"
                },
                {
                    "Namespace": "minecraft:gravel",
                    "Color": "#6B6161"
                },
                {
                    "Namespace": "minecraft:clay",
                    "Color": "#808592"
                },
                {
                    "Namespace": "minecraft:bricks",
                    "Color": "#83402D"
                },
                {
                    "Namespace": "minecraft:obsidian",
                    "Color": "#151123"
                },
                {
                    "Namespace": "minecraft:crying_obsidian",
                    "Color": "#2A0954"
                },
                {
                    "Namespace": "minecraft:deepslate",
                    "Color": "#535353"
                },
                {
                    "Namespace": "minecraft:cobbled_deepslate",
                    "Color": "#535353"
                },
                {
                    "Namespace": "minecraft:polished_deepslate",
                    "Color": "#535353"
                },
                {
                    "Namespace": "minecraft:deepslate_bricks",
                    "Color": "#4C4C4C"
                },
                {
                    "Namespace": "minecraft:cracked_deepslate_bricks",
                    "Color": "#4C4C4C"
                },
                {
                    "Namespace": "minecraft:deepslate_tiles",
                    "Color": "#2D2D2D"
                },
                {
                    "Namespace": "minecraft:cracked_deepslate_tiles",
                    "Color": "#2D2D2D"
                },
                {
                    "Namespace": "minecraft:chiseled_deepslate_tiles",
                    "Color": "#2D2D2D"
                },
                {
                    "Namespace": "minecraft:amethyst_block",
                    "Color": "#6E4F9C"
                },
                {
                    "Namespace": "minecraft:budding_amethyst",
                    "Color": "#6E4F9C"
                },
                {
                    "Namespace": "minecraft:netherrack",
                    "Color": "#6D3533"
                },
                {
                    "Namespace": "minecraft:soul_sand",
                    "Color": "#473326"
                },
                {
                    "Namespace": "minecraft:soul_soil",
                    "Color": "#473326"
                },
                {
                    "Namespace": "minecraft:crimson_nylium",
                    "Color": "#771919"
                },
                {
                    "Namespace": "minecraft:warped_nylium",
                    "Color": "#206154"
                },
                {
                    "Namespace": "minecraft:magma_block",
                    "Color": "#893A09"
                },
                {
                    "Namespace": "minecraft:glowstone",
                    "Color": "#CDA870"
                },
                {
                    "Namespace": "minecraft:basalt",
                    "Color": "#505050"
                },
                {
                    "Namespace": "minecraft:polished_basalt",
                    "Color": "#525252"
                },
                {
                    "Namespace": "minecraft:smooth_basalt",
                    "Color": "#3B3B3B"
                },
                {
                    "Namespace": "minecraft:nether_bricks",
                    "Color": "#2B1010"
                },
                {
                    "Namespace": "minecraft:cracked_nether_bricks",
                    "Color": "#2B1010"
                },
                {
                    "Namespace": "minecraft:chiseled_nether_bricks",
                    "Color": "#2B1010"
                },
                {
                    "Namespace": "minecraft:red_nether_bricks",
                    "Color": "#410101"
                },
                {
                    "Namespace": "minecraft:blackstone",
                    "Color": "#2A252C"
                },
                {
                    "Namespace": "minecraft:giled_blackstone",
                    "Color": "#2A252C"
                },
                {
                    "Namespace": "minecraft:polished_blackstone",
                    "Color": "#2A252C"
                },
                {
                    "Namespace": "minecraft:chiseled_polished_blackstone",
                    "Color": "#3E3641"
                },
                {
                    "Namespace": "minecraft:polished_blackstone_bricks",
                    "Color": "#3E3641"
                },
                {
                    "Namespace": "minecraft:cracked_polished_blackstone_bricks",
                    "Color": "#3E3641"
                },
                {
                    "Namespace": "minecraft:end_stone",
                    "Color": "#CBCD97"
                },
                {
                    "Namespace": "minecraft:end_stone_bricks",
                    "Color": "#CBCD97"
                },
                {
                    "Namespace": "minecraft:purpur_block",
                    "Color": "#8A5E8A"
                },
                {
                    "Namespace": "minecraft:purpur_pillar",
                    "Color": "#8A5E8A"
                },
                {
                    "Namespace": "minecraft:quartz_block",
                    "Color": "#D1CEC8"
                },
                {
                    "Namespace": "minecraft:quartz_bricks",
                    "Color": "#B3B0AA"
                },
                {
                    "Namespace": "minecraft:quartz_pillar",
                    "Color": "#B3B0AA"
                },
                {
                    "Namespace": "minecraft:chiseled_quartz_block",
                    "Color": "#B3B0AA"
                },
                {
                    "Namespace": "minecraft:smooth_quartz",
                    "Color": "#D1CEC8"
                },
                {
                    "Namespace": "minecraft:sand",
                    "Color": "#CEC298"
                },
                {
                    "Namespace": "minecraft:sandstone",
                    "Color": "#B5AD7B"
                },
                {
                    "Namespace": "minecraft:smooth_sandstone",
                    "Color": "#B5AD7B"
                },
                {
                    "Namespace": "minecraft:cut_sandstone",
                    "Color": "#B5AD7B"
                },
                {
                    "Namespace": "minecraft:chiseled_sandstone",
                    "Color": "#B5AD7B"
                },
                {
                    "Namespace": "minecraft:red_sand",
                    "Color": "#933F0B"
                },
                {
                    "Namespace": "minecraft:red_sandstone",
                    "Color": "#833906"
                },
                {
                    "Namespace": "minecraft:smooth_red_sandstone",
                    "Color": "#833906"
                },
                {
                    "Namespace": "minecraft:cut_red_sandstone",
                    "Color": "#833906"
                },
                {
                    "Namespace": "minecraft:chiseled_red_sandstone",
                    "Color": "#833906"
                },
                {
                    "Namespace": "minecraft:grass_block",
                    "Color": "#5D923A"
                },
                {
                    "Namespace": "minecraft:dirt",
                    "Color": "#845B3C"
                },
                {
                    "Namespace": "minecraft:coarse_dirt",
                    "Color": "#69462B"
                },
                {
                    "Namespace": "minecraft:rooted_dirt",
                    "Color": "#69462B"
                },
                {
                    "Namespace": "minecraft:podzol",
                    "Color": "#51330A"
                },
                {
                    "Namespace": "minecraft:mycelium",
                    "Color": "#544955"
                },
                {
                    "Namespace": "minecraft:prismarine",
                    "Color": "#5DA28C"
                },
                {
                    "Namespace": "minecraft:prismarine_bricks",
                    "Color": "#78B5A6"
                },
                {
                    "Namespace": "minecraft:dark_prismarine",
                    "Color": "#244E3D"
                },
                {
                    "Namespace": "minecraft:sea_lantern",
                    "Color": "#A7AFA7"
                },
                {
                    "Namespace": "minecraft:sponge",
                    "Color": "#AAAB27"
                },
                {
                    "Namespace": "minecraft:wet_sponge",
                    "Color": "#7D7C0D"
                },
                {
                    "Namespace": "minecraft:dried_kelp_block",
                    "Color": "#333726"
                },
                {
                    "Namespace": "minecraft:coal_ore",
                    "Color": "#303030"
                },
                {
                    "Namespace": "minecraft:iron_ore",
                    "Color": "#9B785F"
                },
                {
                    "Namespace": "minecraft:copper_ore",
                    "Color": "#7B542D"
                },
                {
                    "Namespace": "minecraft:gold_ore",
                    "Color": "#CFC11E"
                },
                {
                    "Namespace": "minecraft:redstone_ore",
                    "Color": "#860000"
                },
                {
                    "Namespace": "minecraft:emerald_ore",
                    "Color": "#617265"
                },
                {
                    "Namespace": "minecraft:lapis_ore",
                    "Color": "#092D75",
                    "DisplayName": "Lapis Lazuli Ore"
                },
                {
                    "Namespace": "minecraft:diamond_ore",
                    "Color": "#36C5CE"
                },
                {
                    "Namespace": "minecraft:deepslate_coal_ore",
                    "Color": "#202020"
                },
                {
                    "Namespace": "minecraft:deepslate_iron_ore",
                    "Color": "#735846"
                },
                {
                    "Namespace": "minecraft:deepslate_copper_ore",
                    "Color": "#53381E"
                },
                {
                    "Namespace": "minecraft:deepslate_gold_ore",
                    "Color": "#A79B18"
                },
                {
                    "Namespace": "minecraft:deepslate_redstone_ore",
                    "Color": "#653E3F"
                },
                {
                    "Namespace": "minecraft:deepslate_emerald_ore",
                    "Color": "#3F4A41"
                },
                {
                    "Namespace": "minecraft:deepslate_lapis_ore",
                    "Color": "#051D4D",
                    "DisplayName": "Deepslate Lapis Lazuli Ore"
                },
                {
                    "Namespace": "minecraft:deepslate_diamond_ore",
                    "Color": "#2BA0A6"
                },
                {
                    "Namespace": "minecraft:raw_iron_block",
                    "Color": "#A2836A",
                    "DisplayName": "Block of Raw Iron"
                },
                {
                    "Namespace": "minecraft:raw_copper_block",
                    "Color": "#8A543A",
                    "DisplayName": "Block of Raw Copper"
                },
                {
                    "Namespace": "minecraft:raw_gold_block",
                    "Color": "#BC8C1A",
                    "DisplayName": "Block of Raw Gold"
                },
                {
                    "Namespace": "minecraft:coal_block",
                    "Color": "#171717",
                    "DisplayName": "Block of Coal"
                },
                {
                    "Namespace": "minecraft:iron_block",
                    "Color": "#BEBEBE",
                    "DisplayName": "Block of Iron"
                },
                {
                    "Namespace": "minecraft:copper_block",
                    "Color": "#A75237",
                    "DisplayName": "Block of Copper"
                },
                {
                    "Namespace": "minecraft:gold_block",
                    "Color": "#D0C920",
                    "DisplayName": "Block of Gold"
                },
                {
                    "Namespace": "minecraft:redstone_block",
                    "Color": "#9A1000",
                    "DisplayName": "Block of Redstone"
                },
                {
                    "Namespace": "minecraft:emerald_block",
                    "Color": "#32B657",
                    "DisplayName": "Block of Emerald"
                },
                {
                    "Namespace": "minecraft:lapis_block",
                    "Color": "#0B3EAB",
                    "DisplayName": "Block of Lapis Lazuli"
                },
                {
                    "Namespace": "minecraft:diamond_block",
                    "Color": "#57BBB7",
                    "DisplayName": "Block of Diamond"
                },
                {
                    "Namespace": "minecraft:netherite_block",
                    "Color": "#363234",
                    "DisplayName": "Block of Netherite"
                },
                {
                    "Namespace": "minecraft:nether_gold_ore",
                    "Color": "#78482B"
                },
                {
                    "Namespace": "minecraft:nether_quartz_ore",
                    "Color": "#664743"
                },
                {
                    "Namespace": "minecraft:ancient_debris",
                    "Color": "#553E38"
                },
                {
                    "Namespace": "minecraft:exposed_copper",
                    "Color": "#9A7560"
                },
                {
                    "Namespace": "minecraft:weathered_copper",
                    "Color": "#547853"
                },
                {
                    "Namespace": "minecraft:oxidized_copper",
                    "Color": "#38896B"
                },
                {
                    "Namespace": "minecraft:cut_copper",
                    "Color": "#A75237"
                },
                {
                    "Namespace": "minecraft:exposed_cut_copper",
                    "Color": "#9A7560"
                },
                {
                    "Namespace": "minecraft:weathered_cut_copper",
                    "Color": "#547853"
                },
                {
                    "Namespace": "minecraft:oxidized_cut_copper",
                    "Color": "#38896B"
                },
                {
                    "Namespace": "minecraft:waxed_copper",
                    "Color": "#A75237",
                    "DisplayName": "Waxed Block of Copper"
                },
                {
                    "Namespace": "minecraft:waxed_exposed_copper",
                    "Color": "#9A7560"
                },
                {
                    "Namespace": "minecraft:waxed_weathered_copper",
                    "Color": "#547853"
                },
                {
                    "Namespace": "minecraft:waxed_oxidized_copper",
                    "Color": "#38896B"
                },
                {
                    "Namespace": "minecraft:waxed_cut_copper",
                    "Color": "#A75237"
                },
                {
                    "Namespace": "minecraft:waxed_exposed_cut_copper",
                    "Color": "#9A7560"
                },
                {
                    "Namespace": "minecraft:waxed_weathered_cut_copper",
                    "Color": "#547853"
                },
                {
                    "Namespace": "minecraft:waxed_oxidized_cut_copper",
                    "Color": "#38896B"
                },
                {
                    "Namespace": "minecraft:dead_tube_coral_block",
                    "Color": "#6B645F"
                },
                {
                    "Namespace": "minecraft:dead_brain_coral_block",
                    "Color": "#6B645F"
                },
                {
                    "Namespace": "minecraft:dead_bubble_coral_block",
                    "Color": "#6B645F"
                },
                {
                    "Namespace": "minecraft:dead_fire_coral_block",
                    "Color": "#6B645F"
                },
                {
                    "Namespace": "minecraft:dead_horn_coral_block",
                    "Color": "#6B645F"
                },
                {
                    "Namespace": "minecraft:tube_coral_block",
                    "Color": "#2C51C4"
                },
                {
                    "Namespace": "minecraft:brain_coral_block",
                    "Color": "#AD4280"
                },
                {
                    "Namespace": "minecraft:bubble_coral_block",
                    "Color": "#940C90"
                },
                {
                    "Namespace": "minecraft:fire_coral_block",
                    "Color": "#9B171E"
                },
                {
                    "Namespace": "minecraft:horn_coral_block",
                    "Color": "#AD991B"
                },
                {
                    "Namespace": "minecraft:white_concrete",
                    "Color": "#C7C7C7"
                },
                {
                    "Namespace": "minecraft:orange_concrete",
                    "Color": "#C74800"
                },
                {
                    "Namespace": "minecraft:magenta_concrete",
                    "Color": "#951C8B"
                },
                {
                    "Namespace": "minecraft:light_blue_concrete",
                    "Color": "#0D72B0"
                },
                {
                    "Namespace": "minecraft:yellow_concrete",
                    "Color": "#CD8B00"
                },
                {
                    "Namespace": "minecraft:lime_concrete",
                    "Color": "#438E00"
                },
                {
                    "Namespace": "minecraft:pink_concrete",
                    "Color": "#BA4973"
                },
                {
                    "Namespace": "minecraft:gray_concrete",
                    "Color": "#323232"
                },
                {
                    "Namespace": "minecraft:light_gray_concrete",
                    "Color": "#646464"
                },
                {
                    "Namespace": "minecraft:cyan_concrete",
                    "Color": "#026475"
                },
                {
                    "Namespace": "minecraft:purple_concrete",
                    "Color": "#56128E"
                },
                {
                    "Namespace": "minecraft:blue_concrete",
                    "Color": "#212383"
                },
                {
                    "Namespace": "minecraft:brown_concrete",
                    "Color": "#522E12"
                },
                {
                    "Namespace": "minecraft:green_concrete",
                    "Color": "#394B14"
                },
                {
                    "Namespace": "minecraft:red_concrete",
                    "Color": "#801313"
                },
                {
                    "Namespace": "minecraft:black_concrete",
                    "Color": "#0D0D0D"
                },
                {
                    "Namespace": "minecraft:white_concrete_powder",
                    "Color": "#E3E3E3"
                },
                {
                    "Namespace": "minecraft:orange_concrete_powder",
                    "Color": "#E35300"
                },
                {
                    "Namespace": "minecraft:magenta_concrete_powder",
                    "Color": "#BC26AF"
                },
                {
                    "Namespace": "minecraft:light_blue_concrete_powder",
                    "Color": "#1288D0"
                },
                {
                    "Namespace": "minecraft:yellow_concrete_powder",
                    "Color": "#E89E00"
                },
                {
                    "Namespace": "minecraft:lime_concrete_powder",
                    "Color": "#58B600"
                },
                {
                    "Namespace": "minecraft:pink_concrete_powder",
                    "Color": "#D95687"
                },
                {
                    "Namespace": "minecraft:gray_concrete_powder",
                    "Color": "#9B9B9B"
                },
                {
                    "Namespace": "minecraft:light_gray_concrete_powder",
                    "Color": "#7C7C76"
                },
                {
                    "Namespace": "minecraft:cyan_concrete_powder",
                    "Color": "#048EA5"
                },
                {
                    "Namespace": "minecraft:purple_concrete_powder",
                    "Color": "#701AB6"
                },
                {
                    "Namespace": "minecraft:blue_concrete_powder",
                    "Color": "#2F32AF"
                },
                {
                    "Namespace": "minecraft:brown_concrete_powder",
                    "Color": "#77451E"
                },
                {
                    "Namespace": "minecraft:green_concrete_powder",
                    "Color": "#597323"
                },
                {
                    "Namespace": "minecraft:red_concrete_powder",
                    "Color": "#AD1D1D"
                },
                {
                    "Namespace": "minecraft:black_concrete_powder",
                    "Color": "#424242"
                },
                {
                    "Namespace": "minecraft:terracotta",
                    "Color": "#8D533A"
                },
                {
                    "Namespace": "minecraft:white_terracotta",
                    "Color": "#A98979"
                },
                {
                    "Namespace": "minecraft:orange_terracotta",
                    "Color": "#893C0D"
                },
                {
                    "Namespace": "minecraft:magenta_terracotta",
                    "Color": "#7D3F54"
                },
                {
                    "Namespace": "minecraft:yellow_terracotta",
                    "Color": "#9B6504"
                },
                {
                    "Namespace": "minecraft:lime_terracotta",
                    "Color": "#4F5D1B"
                },
                {
                    "Namespace": "minecraft:pink_terracotta",
                    "Color": "#893637"
                },
                {
                    "Namespace": "minecraft:gray_terracotta",
                    "Color": "#2D1D18"
                },
                {
                    "Namespace": "minecraft:light_gray_terracotta",
                    "Color": "#6E5047"
                },
                {
                    "Namespace": "minecraft:cyan_terracotta",
                    "Color": "#414546"
                },
                {
                    "Namespace": "minecraft:purple_terracotta",
                    "Color": "#623142"
                },
                {
                    "Namespace": "minecraft:blue_terracotta",
                    "Color": "#3A2B4B"
                },
                {
                    "Namespace": "minecraft:brown_terracotta",
                    "Color": "#3F2416"
                },
                {
                    "Namespace": "minecraft:green_terracotta",
                    "Color": "#394018"
                },
                {
                    "Namespace": "minecraft:red_terracotta",
                    "Color": "#7B291B"
                },
                {
                    "Namespace": "minecraft:black_terracotta",
                    "Color": "#1C0D08"
                },
                {
                    "Namespace": "minecraft:white_wool",
                    "Color": "#E3E3E3"
                },
                {
                    "Namespace": "minecraft:orange_wool",
                    "Color": "#E35300"
                },
                {
                    "Namespace": "minecraft:magenta_wool",
                    "Color": "#BC26AF"
                },
                {
                    "Namespace": "minecraft:light_blue_wool",
                    "Color": "#1288D0"
                },
                {
                    "Namespace": "minecraft:yellow_wool",
                    "Color": "#E89E00"
                },
                {
                    "Namespace": "minecraft:lime_wool",
                    "Color": "#58B600"
                },
                {
                    "Namespace": "minecraft:pink_wool",
                    "Color": "#D95687"
                },
                {
                    "Namespace": "minecraft:gray_wool",
                    "Color": "#9B9B9B"
                },
                {
                    "Namespace": "minecraft:light_gray_wool",
                    "Color": "#7C7C76"
                },
                {
                    "Namespace": "minecraft:cyan_wool",
                    "Color": "#048EA5"
                },
                {
                    "Namespace": "minecraft:purple_wool",
                    "Color": "#701AB6"
                },
                {
                    "Namespace": "minecraft:blue_wool",
                    "Color": "#2F32AF"
                },
                {
                    "Namespace": "minecraft:brown_wool",
                    "Color": "#77451E"
                },
                {
                    "Namespace": "minecraft:green_wool",
                    "Color": "#597323"
                },
                {
                    "Namespace": "minecraft:red_wool",
                    "Color": "#AD1D1D"
                },
                {
                    "Namespace": "minecraft:black_wool",
                    "Color": "#424242"
                },
                {
                    "Namespace": "minecraft:glass",
                    "Color": "#A6CED6",
                    "Alpha": 192
                },
                {
                    "Namespace": "minecraft:tinted_glass",
                    "Color": "#252523",
                    "Alpha": 192
                },
                {
                    "Namespace": "minecraft:white_stained_glass",
                    "Color": "#E3E3E3",
                    "Alpha": 192
                },
                {
                    "Namespace": "minecraft:orange_stained_glass",
                    "Color": "#E35300",
                    "Alpha": 192
                },
                {
                    "Namespace": "minecraft:magenta_stained_glass",
                    "Color": "#BC26AF",
                    "Alpha": 192
                },
                {
                    "Namespace": "minecraft:light_blue_stained_glass",
                    "Color": "#1288D0",
                    "Alpha": 192
                },
                {
                    "Namespace": "minecraft:yellow_stained_glass",
                    "Color": "#E89E00",
                    "Alpha": 192
                },
                {
                    "Namespace": "minecraft:lime_stained_glass",
                    "Color": "#58B600",
                    "Alpha": 192
                },
                {
                    "Namespace": "minecraft:pink_stained_glass",
                    "Color": "#D95687",
                    "Alpha": 192
                },
                {
                    "Namespace": "minecraft:gray_stained_glass",
                    "Color": "#9B9B9B",
                    "Alpha": 192
                },
                {
                    "Namespace": "minecraft:light_gray_stained_glass",
                    "Color": "#7C7C76",
                    "Alpha": 192
                },
                {
                    "Namespace": "minecraft:cyan_stained_glass",
                    "Color": "#048EA5",
                    "Alpha": 192
                },
                {
                    "Namespace": "minecraft:purple_stained_glass",
                    "Color": "#701AB6",
                    "Alpha": 192
                },
                {
                    "Namespace": "minecraft:blue_stained_glass",
                    "Color": "#2F32AF",
                    "Alpha": 192
                },
                {
                    "Namespace": "minecraft:brown_stained_glass",
                    "Color": "#77451E",
                    "Alpha": 192
                },
                {
                    "Namespace": "minecraft:green_stained_glass",
                    "Color": "#597323",
                    "Alpha": 192
                },
                {
                    "Namespace": "minecraft:red_stained_glass",
                    "Color": "#AD1D1D",
                    "Alpha": 192
                },
                {
                    "Namespace": "minecraft:black_stained_glass",
                    "Color": "#424242",
                    "Alpha": 192
                },
                {
                    "Namespace": "minecraft:glass_pane",
                    "Color": "#A6CED6",
                    "Alpha": 192
                },
                {
                    "Namespace": "minecraft:white_stained_glass_pane",
                    "Color": "#E3E3E3",
                    "Alpha": 192
                },
                {
                    "Namespace": "minecraft:orange_stained_glass_pane",
                    "Color": "#E35300",
                    "Alpha": 192
                },
                {
                    "Namespace": "minecraft:magenta_stained_glass_pane",
                    "Color": "#BC26AF",
                    "Alpha": 192
                },
                {
                    "Namespace": "minecraft:light_blue_stained_glass_pane",
                    "Color": "#1288D0",
                    "Alpha": 192
                },
                {
                    "Namespace": "minecraft:yellow_stained_glass_pane",
                    "Color": "#E89E00",
                    "Alpha": 192
                },
                {
                    "Namespace": "minecraft:lime_stained_glass_pane",
                    "Color": "#58B600",
                    "Alpha": 192
                },
                {
                    "Namespace": "minecraft:pink_stained_glass_pane",
                    "Color": "#D95687",
                    "Alpha": 192
                },
                {
                    "Namespace": "minecraft:gray_stained_glass_pane",
                    "Color": "#9B9B9B",
                    "Alpha": 192
                },
                {
                    "Namespace": "minecraft:light_gray_stained_glass_pane",
                    "Color": "#7C7C76",
                    "Alpha": 192
                },
                {
                    "Namespace": "minecraft:cyan_stained_glass_pane",
                    "Color": "#048EA5",
                    "Alpha": 192
                },
                {
                    "Namespace": "minecraft:purple_stained_glass_pane",
                    "Color": "#701AB6",
                    "Alpha": 192
                },
                {
                    "Namespace": "minecraft:blue_stained_glass_pane",
                    "Color": "#2F32AF",
                    "Alpha": 192
                },
                {
                    "Namespace": "minecraft:brown_stained_glass_pane",
                    "Color": "#77451E",
                    "Alpha": 192
                },
                {
                    "Namespace": "minecraft:green_stained_glass_pane",
                    "Color": "#597323",
                    "Alpha": 192
                },
                {
                    "Namespace": "minecraft:red_stained_glass_pane",
                    "Color": "#AD1D1D",
                    "Alpha": 192
                },
                {
                    "Namespace": "minecraft:black_stained_glass_pane",
                    "Color": "#424242",
                    "Alpha": 192
                },
                {
                    "Namespace": "minecraft:oak_planks",
                    "Color": "#937840"
                },
                {
                    "Namespace": "minecraft:spruce_planks",
                    "Color": "#654523"
                },
                {
                    "Namespace": "minecraft:birch_planks",
                    "Color": "#B09A5E"
                },
                {
                    "Namespace": "minecraft:jungle_planks",
                    "Color": "#956440"
                },
                {
                    "Namespace": "minecraft:acacia_planks",
                    "Color": "#A14F22"
                },
                {
                    "Namespace": "minecraft:dark_oak_planks",
                    "Color": "#3E240C"
                },
                {
                    "Namespace": "minecraft:crimson_planks",
                    "Color": "#5B243B"
                },
                {
                    "Namespace": "minecraft:warped_planks",
                    "Color": "#1B5B56"
                },
                {
                    "Namespace": "minecraft:oak_log",
                    "Color": "#634D2D"
                },
                {
                    "Namespace": "minecraft:spruce_log",
                    "Color": "#231200"
                },
                {
                    "Namespace": "minecraft:birch_log",
                    "Color": "#B4BAB1"
                },
                {
                    "Namespace": "minecraft:jungle_log",
                    "Color": "#4A360B"
                },
                {
                    "Namespace": "minecraft:acacia_log",
                    "Color": "#4A360B"
                },
                {
                    "Namespace": "minecraft:dark_oak_log",
                    "Color": "#2B1F0D"
                },
                {
                    "Namespace": "minecraft:crimson_stem",
                    "Color": "#6B2943"
                },
                {
                    "Namespace": "minecraft:warped_stem",
                    "Color": "#1F6D69"
                },
                {
                    "Namespace": "minecraft:stripped_oak_log",
                    "Color": "#937840"
                },
                {
                    "Namespace": "minecraft:stripped_spruce_log",
                    "Color": "#231200"
                },
                {
                    "Namespace": "minecraft:stripped_birch_log",
                    "Color": "#B4BAB1"
                },
                {
                    "Namespace": "minecraft:stripped_jungle_log",
                    "Color": "#4A360B"
                },
                {
                    "Namespace": "minecraft:stripped_acacia_log",
                    "Color": "#A14F22"
                },
                {
                    "Namespace": "minecraft:stripped_dark_oak_log",
                    "Color": "#2B1F0D"
                },
                {
                    "Namespace": "minecraft:stripped_crimson_stem",
                    "Color": "#6B2943"
                },
                {
                    "Namespace": "minecraft:stripped_warped_stem",
                    "Color": "#1F6D69"
                },
                {
                    "Namespace": "minecraft:stripped_oak_wood",
                    "Color": "#937840"
                },
                {
                    "Namespace": "minecraft:stripped_spruce_wood",
                    "Color": "#231200"
                },
                {
                    "Namespace": "minecraft:stripped_birch_wood",
                    "Color": "#B4BAB1"
                },
                {
                    "Namespace": "minecraft:stripped_jungle_wood",
                    "Color": "#4A360B"
                },
                {
                    "Namespace": "minecraft:stripped_acacia_wood",
                    "Color": "#A14F22"
                },
                {
                    "Namespace": "minecraft:stripped_dark_oak_wood",
                    "Color": "#2B1F0D"
                },
                {
                    "Namespace": "minecraft:stripped_crimson_hyphae",
                    "Color": "#6B2943"
                },
                {
                    "Namespace": "minecraft:stripped_warped_hyphae",
                    "Color": "#1F6D69"
                },
                {
                    "Namespace": "minecraft:oak_wood",
                    "Color": "#634D2D"
                },
                {
                    "Namespace": "minecraft:spruce_wood",
                    "Color": "#231200"
                },
                {
                    "Namespace": "minecraft:birch_wood",
                    "Color": "#B4BAB1"
                },
                {
                    "Namespace": "minecraft:jungle_wood",
                    "Color": "#4A360B"
                },
                {
                    "Namespace": "minecraft:acacia_wood",
                    "Color": "#4A360B"
                },
                {
                    "Namespace": "minecraft:dark_oak_wood",
                    "Color": "#2B1F0D"
                },
                {
                    "Namespace": "minecraft:crimson_hyphae",
                    "Color": "#6B2943"
                },
                {
                    "Namespace": "minecraft:warped_hyphae",
                    "Color": "#1F6D69"
                },
                {
                    "Namespace": "minecraft:nether_wart_block",
                    "Color": "#74090A"
                },
                {
                    "Namespace": "minecraft:warped_wart_block",
                    "Color": "#046767"
                },
                {
                    "Namespace": "minecraft:stone_slab",
                    "Color": "#737373"
                },
                {
                    "Namespace": "minecraft:smooth_stone_slab",
                    "Color": "#808080"
                },
                {
                    "Namespace": "minecraft:stone_brick_slab",
                    "Color": "#606060"
                },
                {
                    "Namespace": "minecraft:mossy_stone_brick_slab",
                    "Color": "#4C6056"
                },
                {
                    "Namespace": "minecraft:cobblestone_slab",
                    "Color": "#797979"
                },
                {
                    "Namespace": "minecraft:granite_slab",
                    "Color": "#8D5B48"
                },
                {
                    "Namespace": "minecraft:diorite_slab",
                    "Color": "#7B7B7E"
                },
                {
                    "Namespace": "minecraft:andesite_slab",
                    "Color": "#656569"
                },
                {
                    "Namespace": "minecraft:polished_granite_slab",
                    "Color": "#8D5B48"
                },
                {
                    "Namespace": "minecraft:polished_diorite_slab",
                    "Color": "#656565"
                },
                {
                    "Namespace": "minecraft:polished_andesite_slab",
                    "Color": "#A0A0A0"
                },
                {
                    "Namespace": "minecraft:sandstone_slab",
                    "Color": "#B5AD7B"
                },
                {
                    "Namespace": "minecraft:smooth_sandstone_slab",
                    "Color": "#B5AD7B"
                },
                {
                    "Namespace": "minecraft:cut_sandstone_slab",
                    "Color": "#B5AD7B"
                },
                {
                    "Namespace": "minecraft:red_sandstone_slab",
                    "Color": "#833906"
                },
                {
                    "Namespace": "minecraft:smooth_red_sandstone_slab",
                    "Color": "#833906"
                },
                {
                    "Namespace": "minecraft:cut_red_sandstone_slab",
                    "Color": "#833906"
                },
                {
                    "Namespace": "minecraft:brick_slab",
                    "Color": "#83402D"
                },
                {
                    "Namespace": "minecraft:nether_brick_slab",
                    "Color": "#2B1015"
                },
                {
                    "Namespace": "minecraft:red_nether_brick_slab",
                    "Color": "#410103"
                },
                {
                    "Namespace": "minecraft:quartz_slab",
                    "Color": "#B3B0AA"
                },
                {
                    "Namespace": "minecraft:smooth_quartz_slab",
                    "Color": "#B3B0AA"
                },
                {
                    "Namespace": "minecraft:prismarine_slab",
                    "Color": "#5BA296"
                },
                {
                    "Namespace": "minecraft:prismarine_brick_slab",
                    "Color": "#4B8778"
                },
                {
                    "Namespace": "minecraft:dark_prismarine_slab",
                    "Color": "#244E3D"
                },
                {
                    "Namespace": "minecraft:cobbled_deepslate_slab",
                    "Color": "#3C3C3C"
                },
                {
                    "Namespace": "minecraft:polished_deepslate_slab",
                    "Color": "#3C3C3C"
                },
                {
                    "Namespace": "minecraft:deepslate_brick_slab",
                    "Color": "#3C3C3C"
                },
                {
                    "Namespace": "minecraft:deepslate_tile_slab",
                    "Color": "#3C3C3C"
                },
                {
                    "Namespace": "minecraft:end_stone_brick_slab",
                    "Color": "#CBCD97"
                },
                {
                    "Namespace": "minecraft:purpur_slab",
                    "Color": "#8A5E8A"
                },
                {
                    "Namespace": "minecraft:blackstone_slab",
                    "Color": "#241F26"
                },
                {
                    "Namespace": "minecraft:polished_blackstone_slab",
                    "Color": "#241F26"
                },
                {
                    "Namespace": "minecraft:polished_blackstone_brick_slab",
                    "Color": "#241F26"
                },
                {
                    "Namespace": "minecraft:cut_copper_slab",
                    "Color": "#A75237"
                },
                {
                    "Namespace": "minecraft:exposed_cut_copper_slab",
                    "Color": "#9A7560"
                },
                {
                    "Namespace": "minecraft:weathered_cut_copper_slab",
                    "Color": "#547853"
                },
                {
                    "Namespace": "minecraft:oxidized_cut_copper_slab",
                    "Color": "#38896B"
                },
                {
                    "Namespace": "minecraft:waxed_cut_copper_slab",
                    "Color": "#A75237"
                },
                {
                    "Namespace": "minecraft:waxed_exposed_cut_copper_slab",
                    "Color": "#9A7560"
                },
                {
                    "Namespace": "minecraft:waxed_weathered_cut_copper_slab",
                    "Color": "#547853"
                },
                {
                    "Namespace": "minecraft:waxed_oxidized_cut_copper_slab",
                    "Color": "#38896B"
                },
                {
                    "Namespace": "minecraft:petrified_oak_slab",
                    "Color": "#937840"
                },
                {
                    "Namespace": "minecraft:oak_slab",
                    "Color": "#937840"
                },
                {
                    "Namespace": "minecraft:spruce_slab",
                    "Color": "#654523"
                },
                {
                    "Namespace": "minecraft:birch_slab",
                    "Color": "#B09A5E"
                },
                {
                    "Namespace": "minecraft:jungle_slab",
                    "Color": "#956440"
                },
                {
                    "Namespace": "minecraft:acacia_slab",
                    "Color": "#A14F22"
                },
                {
                    "Namespace": "minecraft:dark_oak_slab",
                    "Color": "#3E240C"
                },
                {
                    "Namespace": "minecraft:crimson_slab",
                    "Color": "#5B243B"
                },
                {
                    "Namespace": "minecraft:warped_slab",
                    "Color": "#1B5B56"
                },
                {
                    "Namespace": "minecraft:stone_stairs",
                    "Color": "#737373"
                },
                {
                    "Namespace": "minecraft:stone_brick_stairs",
                    "Color": "#606060"
                },
                {
                    "Namespace": "minecraft:mossy_stone_brick_stairs",
                    "Color": "#4C6056"
                },
                {
                    "Namespace": "minecraft:cobblestone_stairs",
                    "Color": "#797979"
                },
                {
                    "Namespace": "minecraft:mossy_cobblestone_stairs",
                    "Color": "#65796B"
                },
                {
                    "Namespace": "minecraft:granite_stairs",
                    "Color": "#8D5B48"
                },
                {
                    "Namespace": "minecraft:andesite_stairs",
                    "Color": "#656565"
                },
                {
                    "Namespace": "minecraft:diorite_stairs",
                    "Color": "#A0A0A0"
                },
                {
                    "Namespace": "minecraft:polished_granite_stairs",
                    "Color": "#8D5B48"
                },
                {
                    "Namespace": "minecraft:polished_andesite_stairs",
                    "Color": "#656565"
                },
                {
                    "Namespace": "minecraft:polished_diorite_stairs",
                    "Color": "#A0A0A0"
                },
                {
                    "Namespace": "minecraft:cobbled_deepslate_stairs",
                    "Color": "#535353"
                },
                {
                    "Namespace": "minecraft:polished_deepslate_stairs",
                    "Color": "#535353"
                },
                {
                    "Namespace": "minecraft:deepslate_brick_stairs",
                    "Color": "#4C4C4C"
                },
                {
                    "Namespace": "minecraft:deepslate_tile_stairs",
                    "Color": "#2D2D2E"
                },
                {
                    "Namespace": "minecraft:brick_stairs",
                    "Color": "#83402D"
                },
                {
                    "Namespace": "minecraft:sandstone_stairs",
                    "Color": "#B5AD7B"
                },
                {
                    "Namespace": "minecraft:smooth_sandstone_stairs",
                    "Color": "#B5AD7B"
                },
                {
                    "Namespace": "minecraft:red_sandstone_stairs",
                    "Color": "#833906"
                },
                {
                    "Namespace": "minecraft:smooth_red_sandstone_stairs",
                    "Color": "#833906"
                },
                {
                    "Namespace": "minecraft:nether_brick_stairs",
                    "Color": "#2B1015"
                },
                {
                    "Namespace": "minecraft:red_nether_brick_stairs",
                    "Color": "#410103"
                },
                {
                    "Namespace": "minecraft:quartz_stairs",
                    "Color": "#B3B0AA"
                },
                {
                    "Namespace": "minecraft:smooth_quartz_stairs",
                    "Color": "#D1CEC8"
                },
                {
                    "Namespace": "minecraft:blackstone_stairs",
                    "Color": "#2A252C"
                },
                {
                    "Namespace": "minecraft:polished_blackstone_stairs",
                    "Color": "#2A252C"
                },
                {
                    "Namespace": "minecraft:polished_blackstone_brick_stairs",
                    "Color": "#3E3641"
                },
                {
                    "Namespace": "minecraft:end_stone_brick_stairs",
                    "Color": "#CBCD97"
                },
                {
                    "Namespace": "minecraft:purpur_stairs",
                    "Color": "#8A5E8A"
                },
                {
                    "Namespace": "minecraft:prismarine_stairs",
                    "Color": "#5DA28C"
                },
                {
                    "Namespace": "minecraft:prismarine_brick_stairs",
                    "Color": "#78B5A6"
                },
                {
                    "Namespace": "minecraft:dark_prismarine_stairs",
                    "Color": "#244E3D"
                },
                {
                    "Namespace": "minecraft:cut_copper_stairs",
                    "Color": "#A75237"
                },
                {
                    "Namespace": "minecraft:exposed_cut_copper_stairs",
                    "Color": "#9A7560"
                },
                {
                    "Namespace": "minecraft:weathered_cut_copper_stairs",
                    "Color": "#547853"
                },
                {
                    "Namespace": "minecraft:oxidized_cut_copper_stairs",
                    "Color": "#38896B"
                },
                {
                    "Namespace": "minecraft:waxed_cut_copper_stairs",
                    "Color": "#A75237"
                },
                {
                    "Namespace": "minecraft:waxed_exposed_cut_copper_stairs",
                    "Color": "#9A7560"
                },
                {
                    "Namespace": "minecraft:waxed_weathered_cut_copper_stairs",
                    "Color": "#547853"
                },
                {
                    "Namespace": "minecraft:waxed_oxidized_cut_copper_stairs",
                    "Color": "#38896B"
                },
                {
                    "Namespace": "minecraft:oak_stairs",
                    "Color": "#937840"
                },
                {
                    "Namespace": "minecraft:acacia_stairs",
                    "Color": "#654523"
                },
                {
                    "Namespace": "minecraft:dark_oak_stairs",
                    "Color": "#B09A5E"
                },
                {
                    "Namespace": "minecraft:spruce_stairs",
                    "Color": "#956440"
                },
                {
                    "Namespace": "minecraft:birch_stairs",
                    "Color": "#A14F22"
                },
                {
                    "Namespace": "minecraft:jungle_stairs",
                    "Color": "#3E240C"
                },
                {
                    "Namespace": "minecraft:crimson_stairs",
                    "Color": "#5B243B"
                },
                {
                    "Namespace": "minecraft:warped_stairs",
                    "Color": "#1B5B56"
                },
                {
                    "Namespace": "minecraft:ice",
                    "Color": "#71A0F2",
                    "Alpha": 192
                },
                {
                    "Namespace": "minecraft:packed_ice",
                    "Color": "#71A0F2"
                },
                {
                    "Namespace": "minecraft:blue_ice",
                    "Color": "#5487DC"
                },
                {
                    "Namespace": "minecraft:snow_block",
                    "Color": "#BFC8C8"
                },
                {
                    "Namespace": "minecraft:pumpkin",
                    "Color": "#B76D0C"
                },
                {
                    "Namespace": "minecraft:carved_pumpkin",
                    "Color": "#A65C00"
                },
                {
                    "Namespace": "minecraft:jack_o_lantern",
                    "Color": "#C59000"
                },
                {
                    "Namespace": "minecraft:hay_block",
                    "Color": "#AB8D02"
                },
                {
                    "Namespace": "minecraft:bone_block",
                    "Color": "#B5B19D"
                },
                {
                    "Namespace": "minecraft:melon",
                    "Color": "#8E8D07"
                },
                {
                    "Namespace": "minecraft:bookshelf",
                    "Color": "#654625"
                },
                {
                    "Namespace": "minecraft:oak_sapling",
                    "Color": "#607F0E",
                    "IsSolid": false
                },
                {
                    "Namespace": "minecraft:spruce_sapling",
                    "Color": "#607F0E",
                    "IsSolid": false
                },
                {
                    "Namespace": "minecraft:birch_sapling",
                    "Color": "#607F0E",
                    "IsSolid": false
                },
                {
                    "Namespace": "minecraft:jungle_sapling",
                    "Color": "#607F0E",
                    "IsSolid": false
                },
                {
                    "Namespace": "minecraft:acacia_sapling",
                    "Color": "#607F0E",
                    "IsSolid": false
                },
                {
                    "Namespace": "minecraft:dark_oak_sapling",
                    "Color": "#607F0E",
                    "IsSolid": false
                },
                {
                    "Namespace": "minecraft:oak_leaves",
                    "Color": "#337814"
                },
                {
                    "Namespace": "minecraft:spruce_leaves",
                    "Color": "#2C512C"
                },
                {
                    "Namespace": "minecraft:birch_leaves",
                    "Color": "#52742D"
                },
                {
                    "Namespace": "minecraft:jungle_leaves",
                    "Color": "#2C9300"
                },
                {
                    "Namespace": "minecraft:acacia_leaves",
                    "Color": "#268300"
                },
                {
                    "Namespace": "minecraft:dark_oak_leaves",
                    "Color": "#1B5E00"
                },
                {
                    "Namespace": "minecraft:azalea_leaves",
                    "Color": "#486117"
                },
                {
                    "Namespace": "minecraft:flowering_azalea_leaves",
                    "Color": "#555C3C"
                },
                {
                    "Namespace": "minecraft:cobweb",
                    "Color": "#DEDEDE",
                    "IsSolid": false
                },
                {
                    "Namespace": "minecraft:grass",
                    "Color": "#5D923A",
                    "IsSolid": false
                },
                {
                    "Namespace": "minecraft:fern",
                    "Color": "#557C2B",
                    "IsSolid": false
                },
                {
                    "Namespace": "minecraft:dead_bush",
                    "Color": "#7E4E12",
                    "IsSolid": false
                },
                {
                    "Namespace": "minecraft:seagrass",
                    "Color": "#7E4E12",
                    "IsSolid": false
                },
                {
                    "Namespace": "minecraft:tall_seagrass",
                    "Color": "#7E4E12",
                    "IsSolid": false
                },
                {
                    "Namespace": "minecraft:sea_pickle",
                    "Color": "#6B7129",
                    "IsSolid": false
                },
                {
                    "Namespace": "minecraft:dandelion",
                    "Color": "#C6D000",
                    "IsSolid": false
                },
                {
                    "Namespace": "minecraft:poppy",
                    "Color": "#BE0A00",
                    "IsSolid": false
                },
                {
                    "Namespace": "minecraft:blue_orchid",
                    "Color": "#098EDC",
                    "IsSolid": false
                },
                {
                    "Namespace": "minecraft:allium",
                    "Color": "#9946DC",
                    "IsSolid": false
                },
                {
                    "Namespace": "minecraft:azure_bluet",
                    "Color": "#B2B8C0",
                    "IsSolid": false
                },
                {
                    "Namespace": "minecraft:red_tulip",
                    "Color": "#AF1800",
                    "IsSolid": false
                },
                {
                    "Namespace": "minecraft:orange_tulip",
                    "Color": "#C35305",
                    "IsSolid": false
                },
                {
                    "Namespace": "minecraft:white_tulip",
                    "Color": "#B6B6B6",
                    "IsSolid": false
                },
                {
                    "Namespace": "minecraft:pink_tulip",
                    "Color": "#BD91BD",
                    "IsSolid": false
                },
                {
                    "Namespace": "minecraft:cornflower",
                    "Color": "#617FE5",
                    "IsSolid": false
                },
                {
                    "Namespace": "minecraft:lily_of_the_valley",
                    "Color": "#E4E4E4",
                    "IsSolid": false
                },
                {
                    "Namespace": "minecraft:wither_rose",
                    "Color": "#23270F",
                    "IsSolid": false
                },
                {
                    "Namespace": "minecraft:sunflower",
                    "Color": "#DCC511",
                    "IsSolid": false
                },
                {
                    "Namespace": "minecraft:lilac",
                    "Color": "#957899",
                    "IsSolid": false
                },
                {
                    "Namespace": "minecraft:rose_bush",
                    "Color": "#E10E00",
                    "IsSolid": false
                },
                {
                    "Namespace": "minecraft:peony",
                    "Color": "#B992CA",
                    "IsSolid": false
                },
                {
                    "Namespace": "minecraft:tall_grass",
                    "Color": "#5D923A",
                    "IsSolid": false
                },
                {
                    "Namespace": "minecraft:large_fern",
                    "Color": "#6FA535",
                    "IsSolid": false
                },
                {
                    "Namespace": "minecraft:azalea",
                    "Color": "#5F7628"
                },
                {
                    "Namespace": "minecraft:flowering_azalea",
                    "Color": "#5F7628"
                },
                {
                    "Namespace": "minecraft:spore_blossom",
                    "Color": "#5F7628"
                },
                {
                    "Namespace": "minecraft:brown_mushroom",
                    "Color": "#5F4533",
                    "IsSolid": false
                },
                {
                    "Namespace": "minecraft:red_mushroom",
                    "Color": "#9B0D0B",
                    "IsSolid": false
                },
                {
                    "Namespace": "minecraft:crimson_fungus",
                    "Color": "#8B2D19",
                    "IsSolid": false
                },
                {
                    "Namespace": "minecraft:warped_fungus",
                    "Color": "#007969",
                    "IsSolid": false
                },
                {
                    "Namespace": "minecraft:crimson_root",
                    "Color": "#8B2D19",
                    "IsSolid": false
                },
                {
                    "Namespace": "minecraft:warped_roots",
                    "Color": "#007969",
                    "IsSolid": false
                },
                {
                    "Namespace": "minecraft:nether_sprouts",
                    "Color": "#007969",
                    "IsSolid": false
                },
                {
                    "Namespace": "minecraft:weeping_vines",
                    "Color": "#8B2D19"
                },
                {
                    "Namespace": "minecraft:twisting_vines",
                    "Color": "#007969"
                },
                {
                    "Namespace": "minecraft:vine",
                    "Color": "#356600",
                    "IsSolid": false
                },
                {
                    "Namespace": "minecraft:glow_lichen",
                    "Color": "#586A60",
                    "IsSolid": false
                },
                {
                    "Namespace": "minecraft:lily_pad",
                    "Color": "#1A782A",
                    "IsSolid": false
                },
                {
                    "Namespace": "minecraft:sugar_cane",
                    "Color": "#698B45"
                },
                {
                    "Namespace": "minecraft:kelp",
                    "Color": "#508926"
                },
                {
                    "Namespace": "minecraft:moss_block",
                    "Color": "#546827"
                },
                {
                    "Namespace": "minecraft:hanging_roots",
                    "Color": "#895C47"
                },
                {
                    "Namespace": "minecraft:big_dripleaf",
                    "Color": "#59781A"
                },
                {
                    "Namespace": "minecraft:small_dripleaf",
                    "Color": "#59781A"
                },
                {
                    "Namespace": "minecraft:bamboo",
                    "Color": "#607F0E"
                },
                {
                    "Namespace": "minecraft:torch",
                    "Color": "#EDED00",
                    "IsSolid": false
                },
                {
                    "Namespace": "minecraft:end_rod",
                    "Color": "#CACACA"
                },
                {
                    "Namespace": "minecraft:chorus_plant",
                    "Color": "#AF77E5"
                },
                {
                    "Namespace": "minecraft:chorus_flower",
                    "Color": "#AF77E5"
                },
                {
                    "Namespace": "minecraft:chest",
                    "Color": "#98671B"
                },
                {
                    "Namespace": "minecraft:crafting_table",
                    "Color": "#7B4D2B"
                },
                {
                    "Namespace": "minecraft:furnace",
                    "Color": "#5E5E5F"
                },
                {
                    "Namespace": "minecraft:farmland",
                    "Color": "#512B10"
                },
                {
                    "Namespace": "minecraft:dirt_path",
                    "Color": "#7D642E"
                },
                {
                    "Namespace": "minecraft:ladder",
                    "Color": "#7B4D2B"
                },
                {
                    "Namespace": "minecraft:snow",
                    "Color": "#DFE9E9"
                },
                {
                    "Namespace": "minecraft:cactus",
                    "Color": "#085C13"
                },
                {
                    "Namespace": "minecraft:jukebox",
                    "Color": "#7B4D2B"
                },
                {
                    "Namespace": "minecraft:oak_fence",
                    "Color": "#937840"
                },
                {
                    "Namespace": "minecraft:spruce_fence",
                    "Color": "#654523"
                },
                {
                    "Namespace": "minecraft:birch_fence",
                    "Color": "#B09A5E"
                },
                {
                    "Namespace": "minecraft:jungle_fence",
                    "Color": "#956440"
                },
                {
                    "Namespace": "minecraft:acacia_fence",
                    "Color": "#A14F22"
                },
                {
                    "Namespace": "minecraft:dark_oak_fence",
                    "Color": "#3E240C"
                },
                {
                    "Namespace": "minecraft:crimson_fence",
                    "Color": "#5B243B"
                },
                {
                    "Namespace": "minecraft:warped_fence",
                    "Color": "#1B5B56"
                },
                {
                    "Namespace": "minecraft:infested_stone",
                    "Color": "#737373"
                },
                {
                    "Namespace": "minecraft:infested_cobblestone",
                    "Color": "#797979"
                },
                {
                    "Namespace": "minecraft:infested_stone_bricks",
                    "Color": "#606060"
                },
                {
                    "Namespace": "minecraft:infested_mossy_stone_bricks",
                    "Color": "#4C6056"
                },
                {
                    "Namespace": "minecraft:infested_cracked_stone_bricks",
                    "Color": "#606060"
                },
                {
                    "Namespace": "minecraft:infested_chiseled_stone_bricks",
                    "Color": "#606060"
                },
                {
                    "Namespace": "minecraft:infested_deepslate",
                    "Color": "#535353"
                },
                {
                    "Namespace": "minecraft:brown_mushroom_block",
                    "Color": "#5F4533"
                },
                {
                    "Namespace": "minecraft:red_mushroom_block",
                    "Color": "#9B0D0B"
                },
                {
                    "Namespace": "minecraft:mushroom_stem",
                    "Color": "#C5BFB2"
                },
                {
                    "Namespace": "minecraft:iron_bars",
                    "Color": "#828383"
                },
                {
                    "Namespace": "minecraft:chain",
                    "Color": "#3D4352"
                },
                {
                    "Namespace": "minecraft:enchanting_table",
                    "Color": "#A02828"
                },
                {
                    "Namespace": "minecraft:end_portal_frame",
                    "Color": "#376559"
                },
                {
                    "Namespace": "minecraft:ender_chest",
                    "Color": "#253638"
                },
                {
                    "Namespace": "minecraft:cobblestone_wall",
                    "Color": "#797979"
                },
                {
                    "Namespace": "minecraft:mossy_cobblestone_wall",
                    "Color": "#65796B"
                },
                {
                    "Namespace": "minecraft:brick_wall",
                    "Color": "#83402D"
                },
                {
                    "Namespace": "minecraft:stone_brick_wall",
                    "Color": "#606060"
                },
                {
                    "Namespace": "minecraft:mossy_stone_brick_wall",
                    "Color": "#4C6056"
                },
                {
                    "Namespace": "minecraft:granite_wall",
                    "Color": "#8D5B48"
                },
                {
                    "Namespace": "minecraft:andesite_wall",
                    "Color": "#656565"
                },
                {
                    "Namespace": "minecraft:diorite_wall",
                    "Color": "#A0A0A0"
                },
                {
                    "Namespace": "minecraft:nether_brick_wall",
                    "Color": "#2B1015"
                },
                {
                    "Namespace": "minecraft:red_nether_brick_wall",
                    "Color": "#410103"
                },
                {
                    "Namespace": "minecraft:blackstone_wall",
                    "Color": "#2A252C"
                },
                {
                    "Namespace": "minecraft:polished_blackstone_wall",
                    "Color": "#2A252C"
                },
                {
                    "Namespace": "minecraft:polished_blackstone_brick_wall",
                    "Color": "#2A252C"
                },
                {
                    "Namespace": "minecraft:cobbled_deepslate_wall",
                    "Color": "#535353"
                },
                {
                    "Namespace": "minecraft:polished_deepslate_wall",
                    "Color": "#535353"
                },
                {
                    "Namespace": "minecraft:deepslate_brick_wall",
                    "Color": "#4C4C4C"
                },
                {
                    "Namespace": "minecraft:deepslate_tile_wall",
                    "Color": "#2D2D2E"
                },
                {
                    "Namespace": "minecraft:sandstone_wall",
                    "Color": "#B5AD7B"
                },
                {
                    "Namespace": "minecraft:red_sandstone_wall",
                    "Color": "#833906"
                },
                {
                    "Namespace": "minecraft:prismarine_wall",
                    "Color": "#5DA28C"
                },
                {
                    "Namespace": "minecraft:end_stone_brick_wall",
                    "Color": "#CBCD97"
                },
                {
                    "Namespace": "minecraft:anvil",
                    "Color": "#3B3B3B"
                },
                {
                    "Namespace": "minecraft:chipped_anvil",
                    "Color": "#3B3B3B"
                },
                {
                    "Namespace": "minecraft:damaged_anvil",
                    "Color": "#3B3B3B"
                },
                {
                    "Namespace": "minecraft:shulker_box",
                    "Color": "#845984"
                },
                {
                    "Namespace": "minecraft:white_shulker_box",
                    "Color": "#ACB1B2"
                },
                {
                    "Namespace": "minecraft:orange_shulker_box",
                    "Color": "#D05000"
                },
                {
                    "Namespace": "minecraft:magenta_shulker_box",
                    "Color": "#99228F"
                },
                {
                    "Namespace": "minecraft:light_blue_shulker_box",
                    "Color": "#1689B8"
                },
                {
                    "Namespace": "minecraft:yellow_shulker_box",
                    "Color": "#D29700"
                },
                {
                    "Namespace": "minecraft:lime_shulker_box",
                    "Color": "#499300"
                },
                {
                    "Namespace": "minecraft:pink_shulker_box",
                    "Color": "#C75B7E"
                },
                {
                    "Namespace": "minecraft:gray_shulker_box",
                    "Color": "#2C2F33"
                },
                {
                    "Namespace": "minecraft:light_gray_shulker_box",
                    "Color": "#65655C"
                },
                {
                    "Namespace": "minecraft:cyan_shulker_box",
                    "Color": "#016775"
                },
                {
                    "Namespace": "minecraft:purple_shulker_box",
                    "Color": "#754A75"
                },
                {
                    "Namespace": "minecraft:blue_shulker_box",
                    "Color": "#202281"
                },
                {
                    "Namespace": "minecraft:brown_shulker_box",
                    "Color": "#5C3315"
                },
                {
                    "Namespace": "minecraft:green_shulker_box",
                    "Color": "#3D530E"
                },
                {
                    "Namespace": "minecraft:red_shulker_box",
                    "Color": "#811312"
                },
                {
                    "Namespace": "minecraft:black_shulker_box",
                    "Color": "#141418"
                },
                {
                    "Namespace": "minecraft:white_glazed_terracotta",
                    "Color": "#C7C7C7"
                },
                {
                    "Namespace": "minecraft:orange_glazed_terracotta",
                    "Color": "#C74800"
                },
                {
                    "Namespace": "minecraft:magenta_glazed_terracotta",
                    "Color": "#951C8B"
                },
                {
                    "Namespace": "minecraft:light_blue_glazed_terracotta",
                    "Color": "#0D72B0"
                },
                {
                    "Namespace": "minecraft:yellow_glazed_terracotta",
                    "Color": "#CD8B00"
                },
                {
                    "Namespace": "minecraft:lime_glazed_terracotta",
                    "Color": "#438E00"
                },
                {
                    "Namespace": "minecraft:pink_glazed_terracotta",
                    "Color": "#BA4973"
                },
                {
                    "Namespace": "minecraft:gray_glazed_terracotta",
                    "Color": "#323232"
                },
                {
                    "Namespace": "minecraft:light_gray_glazed_terracotta",
                    "Color": "#646464"
                },
                {
                    "Namespace": "minecraft:cyan_glazed_terracotta",
                    "Color": "#026475"
                },
                {
                    "Namespace": "minecraft:purple_glazed_terracotta",
                    "Color": "#56128E"
                },
                {
                    "Namespace": "minecraft:blue_glazed_terracotta",
                    "Color": "#212383"
                },
                {
                    "Namespace": "minecraft:brown_glazed_terracotta",
                    "Color": "#522E12"
                },
                {
                    "Namespace": "minecraft:green_glazed_terracotta",
                    "Color": "#394B14"
                },
                {
                    "Namespace": "minecraft:red_glazed_terracotta",
                    "Color": "#801313"
                },
                {
                    "Namespace": "minecraft:black_glazed_terracotta",
                    "Color": "#0D0D0D"
                },
                {
                    "Namespace": "minecraft:tube_coral",
                    "Color": "#2C51C4"
                },
                {
                    "Namespace": "minecraft:brain_coral",
                    "Color": "#AD4280"
                },
                {
                    "Namespace": "minecraft:bubble_coral",
                    "Color": "#940C90"
                },
                {
                    "Namespace": "minecraft:fire_coral",
                    "Color": "#9B171E"
                },
                {
                    "Namespace": "minecraft:horn_coral",
                    "Color": "#AD991B"
                },
                {
                    "Namespace": "minecraft:dead_tube_coral",
                    "Color": "#6B645F"
                },
                {
                    "Namespace": "minecraft:dead_brain_coral",
                    "Color": "#6B645F"
                },
                {
                    "Namespace": "minecraft:dead_bubble_coral",
                    "Color": "#6B645F"
                },
                {
                    "Namespace": "minecraft:dead_fire_coral",
                    "Color": "#6B645F"
                },
                {
                    "Namespace": "minecraft:dead_horn_coral",
                    "Color": "#6B645F"
                },
                {
                    "Namespace": "minecraft:tube_coral_fan",
                    "Color": "#2C51C4"
                },
                {
                    "Namespace": "minecraft:brain_coral_fan",
                    "Color": "#AD4280"
                },
                {
                    "Namespace": "minecraft:bubble_coral_fan",
                    "Color": "#940C90"
                },
                {
                    "Namespace": "minecraft:fire_coral_fan",
                    "Color": "#9B171E"
                },
                {
                    "Namespace": "minecraft:horn_coral_fan",
                    "Color": "#AD991B"
                },
                {
                    "Namespace": "minecraft:dead_tube_coral_fan",
                    "Color": "#6B645F"
                },
                {
                    "Namespace": "minecraft:dead_brain_coral_fan",
                    "Color": "#6B645F"
                },
                {
                    "Namespace": "minecraft:dead_bubble_coral_fan",
                    "Color": "#6B645F"
                },
                {
                    "Namespace": "minecraft:dead_fire_coral_fan",
                    "Color": "#6B645F"
                },
                {
                    "Namespace": "minecraft:dead_horn_coral_fan",
                    "Color": "#6B645F"
                },
                {
                    "Namespace": "minecraft:oak_sign",
                    "Color": "#937840"
                },
                {
                    "Namespace": "minecraft:spruce_sign",
                    "Color": "#654523"
                },
                {
                    "Namespace": "minecraft:birch_sign",
                    "Color": "#B09A5E"
                },
                {
                    "Namespace": "minecraft:jungle_sign",
                    "Color": "#956440"
                },
                {
                    "Namespace": "minecraft:acacia_sign",
                    "Color": "#A14F22"
                },
                {
                    "Namespace": "minecraft:dark_oak_sign",
                    "Color": "#3E240C"
                },
                {
                    "Namespace": "minecraft:crimson_sign",
                    "Color": "#5B243B"
                },
                {
                    "Namespace": "minecraft:warped_sign",
                    "Color": "#1B5B56"
                },
                {
                    "Namespace": "minecraft:white_bed",
                    "Color": "#E3E3E3"
                },
                {
                    "Namespace": "minecraft:orange_bed",
                    "Color": "#E35300"
                },
                {
                    "Namespace": "minecraft:magenta_bed",
                    "Color": "#BC26AF"
                },
                {
                    "Namespace": "minecraft:light_blue_bed",
                    "Color": "#1288D0"
                },
                {
                    "Namespace": "minecraft:yellow_bed",
                    "Color": "#E89E00"
                },
                {
                    "Namespace": "minecraft:lime_bed",
                    "Color": "#58B600"
                },
                {
                    "Namespace": "minecraft:pink_bed",
                    "Color": "#D95687"
                },
                {
                    "Namespace": "minecraft:gray_bed",
                    "Color": "#9B9B9B"
                },
                {
                    "Namespace": "minecraft:light_gray_bed",
                    "Color": "#7C7C76"
                },
                {
                    "Namespace": "minecraft:cyan_bed",
                    "Color": "#048EA5"
                },
                {
                    "Namespace": "minecraft:purple_bed",
                    "Color": "#701AB6"
                },
                {
                    "Namespace": "minecraft:blue_bed",
                    "Color": "#2F32AF"
                },
                {
                    "Namespace": "minecraft:brown_bed",
                    "Color": "#77451E"
                },
                {
                    "Namespace": "minecraft:green_bed",
                    "Color": "#597323"
                },
                {
                    "Namespace": "minecraft:red_bed",
                    "Color": "#AD1D1D"
                },
                {
                    "Namespace": "minecraft:black_bed",
                    "Color": "#424242"
                },
                {
                    "Namespace": "minecraft:scaffolding",
                    "Color": "#A9834A"
                },
                {
                    "Namespace": "minecraft:flower_pot",
                    "Color": "#743F31"
                },
                {
                    "Namespace": "minecraft:skeleton_head",
                    "Color": "#A1A1A1"
                },
                {
                    "Namespace": "minecraft:wither_skeleton_head",
                    "Color": "#A1A1A1"
                },
                {
                    "Namespace": "minecraft:player_head",
                    "Color": "#A1A1A1"
                },
                {
                    "Namespace": "minecraft:zombie_head",
                    "Color": "#A1A1A1"
                },
                {
                    "Namespace": "minecraft:creeper_head",
                    "Color": "#A1A1A1"
                },
                {
                    "Namespace": "minecraft:dragon_head",
                    "Color": "#A1A1A1"
                },
                {
                    "Namespace": "minecraft:white_banner",
                    "Color": "#E3E3E3"
                },
                {
                    "Namespace": "minecraft:orange_banner",
                    "Color": "#E35300"
                },
                {
                    "Namespace": "minecraft:magenta_banner",
                    "Color": "#BC26AF"
                },
                {
                    "Namespace": "minecraft:light_blue_banner",
                    "Color": "#1288D0"
                },
                {
                    "Namespace": "minecraft:yellow_banner",
                    "Color": "#E89E00"
                },
                {
                    "Namespace": "minecraft:lime_banner",
                    "Color": "#58B600"
                },
                {
                    "Namespace": "minecraft:pink_banner",
                    "Color": "#D95687"
                },
                {
                    "Namespace": "minecraft:gray_banner",
                    "Color": "#9B9B9B"
                },
                {
                    "Namespace": "minecraft:light_gray_banner",
                    "Color": "#7C7C76"
                },
                {
                    "Namespace": "minecraft:cyan_banner",
                    "Color": "#048EA5"
                },
                {
                    "Namespace": "minecraft:purple_banner",
                    "Color": "#701AB6"
                },
                {
                    "Namespace": "minecraft:blue_banner",
                    "Color": "#2F32AF"
                },
                {
                    "Namespace": "minecraft:brown_banner",
                    "Color": "#77451E"
                },
                {
                    "Namespace": "minecraft:green_banner",
                    "Color": "#597323"
                },
                {
                    "Namespace": "minecraft:red_banner",
                    "Color": "#AD1D1D"
                },
                {
                    "Namespace": "minecraft:black_banner",
                    "Color": "#424242"
                },
                {
                    "Namespace": "minecraft:loom",
                    "Color": "#907962"
                },
                {
                    "Namespace": "minecraft:composter",
                    "Color": "#663B16"
                },
                {
                    "Namespace": "minecraft:barrel",
                    "Color": "#704E25"
                },
                {
                    "Namespace": "minecraft:smoker",
                    "Color": "#494746"
                },
                {
                    "Namespace": "minecraft:blast_furnace",
                    "Color": "#424142"
                },
                {
                    "Namespace": "minecraft:cartography_table",
                    "Color": "#695D55"
                },
                {
                    "Namespace": "minecraft:fletching_table",
                    "Color": "#A39267"
                },
                {
                    "Namespace": "minecraft:grindstone",
                    "Color": "#717171"
                },
                {
                    "Namespace": "minecraft:smithing_table",
                    "Color": "#2E2F3C"
                },
                {
                    "Namespace": "minecraft:stonecutter",
                    "Color": "#64605C"
                },
                {
                    "Namespace": "minecraft:bell",
                    "Color": "#BB932B"
                },
                {
                    "Namespace": "minecraft:lantern",
                    "Color": "#E1A153"
                },
                {
                    "Namespace": "minecraft:soul_lantern",
                    "Color": "#97C3C5"
                },
                {
                    "Namespace": "minecraft:campfire",
                    "Color": "#D3A556"
                },
                {
                    "Namespace": "minecraft:soul_campfire",
                    "Color": "#23BEC4"
                },
                {
                    "Namespace": "minecraft:shroomlight",
                    "Color": "#CF793B"
                },
                {
                    "Namespace": "minecraft:bee_nest",
                    "Color": "#AF8A2E"
                },
                {
                    "Namespace": "minecraft:beehive",
                    "Color": "#96743C"
                },
                {
                    "Namespace": "minecraft:honeycomb_block",
                    "Color": "#C47913"
                },
                {
                    "Namespace": "minecraft:lodestone",
                    "Color": "#636466"
                },
                {
                    "Namespace": "minecraft:respawn_anchor",
                    "Color": "#5302B8"
                },
                {
                    "Namespace": "minecraft:beacon",
                    "Color": "#85D4BB"
                },
                {
                    "Namespace": "minecraft:water",
                    "Color": "#22417F",
                    "Alpha": 125
                },
                {
                    "Namespace": "minecraft:bubble_column",
                    "Color": "#22417F"
                },
                {
                    "Namespace": "minecraft:lava",
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

using binstarjs03.AerialOBJ.Core.Primitives;

namespace binstarjs03.AerialOBJ.Core.Definitions;

public class ViewportBlockDefinition
{
    /// <summary>
    /// Minecraft Namespace for this block, example: "minecraft:dirt"
    /// </summary>
    public required string Namespace { get; set; }

    /// <summary>
    /// Base color for this block to be rendered
    /// </summary>
    public required Color Color { get; set; }

    /// <summary>
    /// Name to be displayed to the GUI
    /// </summary>
    public required string DisplayName { get; set; }

    /// <summary>
    /// Affect rendering only. Height position of this block will be flushed 
    /// with lower first solid block. Useful for foliage such as grass to make the 
    /// rendered  terrain topography flushed with the ground
    /// </summary>
    public required bool IsSolid { get; set; }

    /// <summary>
    /// Whether this block will be excluded (ignored) in both viewport and 
    /// 3D model exporting
    /// </summary>
    public required bool IsExcluded { get; set; }

    public override string ToString()
    {
        return $"{DisplayName} - {Color}";
    }
}

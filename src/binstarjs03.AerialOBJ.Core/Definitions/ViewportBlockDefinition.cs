using binstarjs03.AerialOBJ.Core.Primitives;

namespace binstarjs03.AerialOBJ.Core.Definitions;

/// <summary>
/// Represent defined, known Minecraft block. Do not be mistaken, 
/// this class do not store block information by itself
/// </summary>
public class ViewportBlockDefinition
{
    /// <summary>
    /// Minecraft Name for this block, including the namespace
    /// </summary>
    public required string Name { get; set; }

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

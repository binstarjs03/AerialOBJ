namespace binstarjs03.AerialOBJ.Core.Definitions;
public static class ViewportDefinitionExtension
{
    public static bool IsExcluded(this string blockName, ViewportDefinition viewportDefinition)
    {
        if (viewportDefinition.BlockDefinitions.TryGetValue(blockName, out ViewportBlockDefinition? block))
            return block.IsExcluded;
        return false;
    }
}

using binstarjs03.AerialOBJ.Core.Definitions;

namespace binstarjs03.AerialOBJ.Core.MinecraftWorld;
public static class BlockSlimExtension
{
    public static ViewportBlockDefinition GetBlockDefinitionOrDefault(this BlockSlim block, ViewportDefinition viewportDefinition)
    {
        if (!viewportDefinition.BlockDefinitions.TryGetValue(block.Name, out var blockDefinition))
            blockDefinition = viewportDefinition.MissingBlockDefinition;
        return blockDefinition;
    }
}

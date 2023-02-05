using binstarjs03.AerialOBJ.Core.Definitions;
using binstarjs03.AerialOBJ.Core.MinecraftWorld;
using binstarjs03.AerialOBJ.Core.Primitives;

namespace binstarjs03.AerialOBJ.Imaging.ChunkRendering;

public class ChunkRenderOptions
{
    public required ViewportDefinition ViewportDefinition { get; set; }
    public required IImage Image { get; set; }
    public PointY<int> RenderPosition { get; set; }
    public int HeightLimit { get; set; }
    public required IChunk Chunk { get; set; }
    public BlockSlim[,]? HighestBlocks { get; set; }
}
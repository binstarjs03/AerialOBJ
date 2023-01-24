using binstarjs03.AerialOBJ.Core.Definitions;
using binstarjs03.AerialOBJ.Core.MinecraftWorld;
using binstarjs03.AerialOBJ.Core.Primitives;

namespace binstarjs03.AerialOBJ.Imaging.ChunkRendering;
public interface IChunkShader
{
    void RenderChunk(ChunkRenderSetting setting);

    static PointY<int> GetPixelCoordsForBlock(PointY<int> renderPosition, PointZ<int> chunkCoordsRel, PointZ<int> blockCoordsRel)
    {
        int pixelX = renderPosition.X 
                   + (chunkCoordsRel.X * IChunk.BlockCount) 
                   + blockCoordsRel.X;

        int pixelY = renderPosition.Y
                   + (chunkCoordsRel.Z * IChunk.BlockCount)
                   + blockCoordsRel.Z;

        return new PointY<int>(pixelX, pixelY);
    }
}

public class ChunkRenderSetting
{
    public required ViewportDefinition ViewportDefinition { get; set; }
    public required IImage Image { get; set; }
    public PointY<int> RenderPosition { get; set; }
    public int HeightLimit { get; set; }
    public required IChunk Chunk { get; set; }
    public BlockSlim[,]? HighestBlocks { get; set; }
    public List<string>? Exclusions { get; set; }
}
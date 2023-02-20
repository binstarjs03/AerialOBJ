using binstarjs03.AerialOBJ.Core.MinecraftWorld;
using binstarjs03.AerialOBJ.Core.Primitives;
using binstarjs03.AerialOBJ.Imaging.ChunkRendering;
using binstarjs03.AerialOBJ.MvvmAppCore.Components;
using binstarjs03.AerialOBJ.MvvmAppCore.Models.Settings;

namespace binstarjs03.AerialOBJ.MvvmAppCore.Services;
public class ChunkRenderer : IChunkRenderer
{
    private readonly DefinitionSetting _definitionSetting;
    private readonly ViewportSetting _viewportSetting;
    private readonly Color _transparent = new() { Alpha = 0, Red = 0, Green = 0, Blue = 0 };

    public ChunkRenderer(Setting setting)
    {
        _definitionSetting = setting.DefinitionSetting;
        _viewportSetting = setting.ViewportSetting;
    }

    public void RenderChunk(IRegionImage regionImage, IChunk chunk, BlockSlim[,] highestBlocks, int heightLimit)
    {
        ChunkRenderOptions renderOptions = new()
        {
            ViewportDefinition = _definitionSetting.CurrentViewportDefinition,
            Image = regionImage,
            RenderPosition = new PointY<int>(0, 0),
            HeightLimit = heightLimit,
            Chunk = chunk,
            HighestBlocks = highestBlocks,
        };
        _viewportSetting.ChunkShader.RenderChunk(renderOptions);
    }

    public void EraseChunk(IRegionImage regionImage, PointZ<int> chunkCoordsRel)
    {
        for (int x = 0; x < IChunk.BlockCount; x++)
            for (int z = 0; z < IChunk.BlockCount; z++)
            {
                // use chunk coords as rendering position also as the chunk coords rel too
                PointZ<int> blockCoordsRel = new(x, z);
                PointY<int> pixelCoords = IChunkShader.GetPixelCoordsForBlock(chunkCoordsRel, chunkCoordsRel, blockCoordsRel);
                regionImage[pixelCoords.X, pixelCoords.Y] = _transparent;
            }
    }
}

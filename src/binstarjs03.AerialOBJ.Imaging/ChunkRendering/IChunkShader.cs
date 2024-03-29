﻿using binstarjs03.AerialOBJ.Core.MinecraftWorld;
using binstarjs03.AerialOBJ.Core.Primitives;

namespace binstarjs03.AerialOBJ.Imaging.ChunkRendering;
public interface IChunkShader
{
    string ShaderName { get; }

    void RenderChunk(ChunkRenderOptions options);

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
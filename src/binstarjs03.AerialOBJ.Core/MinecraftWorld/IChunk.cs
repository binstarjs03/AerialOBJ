using System;
using System.Collections.Generic;

using binstarjs03.AerialOBJ.Core.Definitions;
using binstarjs03.AerialOBJ.Core.Primitives;

namespace binstarjs03.AerialOBJ.Core.MinecraftWorld;

/// <summary>
/// Represent Chunk Data
/// </summary>
public interface IChunk : IDisposable
{
    /// <summary>
    /// Block count in single dimension in section
    /// </summary>
    const int BlockCount = 16;

    /// <summary>
    /// Block index range for indexing. Use this to write cleaner for loop
    /// instead of having to write "BlockCount - 1"
    /// </summary>
    const int BlockRange = BlockCount - 1;

    /// <summary>
    /// Returns the minimum supported DataVersion for this chunk
    /// </summary>
    int DataVersion { get; }

    /// <summary>
    /// Returns the minimum supported Minecraft version for this chunk
    /// </summary>
    string MinecraftVersion { get; } // example: "1.18.2"

    /// <summary>
    /// Returns absolute position of this chunk
    /// </summary>
    PointZ<int> CoordsAbs { get; }

    /// <summary>
    /// Returns relative position of this chunk, relative to region
    /// </summary>
    PointZ<int> CoordsRel { get; }

    /// <summary>
    /// Get highest block of this chunk, starting from <paramref name="heightLimit"/>
    /// and scans it to the lowest block of lowest section of this chunk.
    /// </summary>
    void GetHighestBlockSlim(ViewportDefinition vd, BlockSlim[,] buffer, int heightLimit);

    BlockSlim GetHighestBlockSlimSingleNoCheck(ViewportDefinition vd, PointZ<int> blockCoordsRel, int heightLimit, string exclusion);
}
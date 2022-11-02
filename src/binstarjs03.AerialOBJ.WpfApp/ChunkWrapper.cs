using System.IO;
using System.Windows.Controls;

using binstarjs03.AerialOBJ.Core.CoordinateSystem;
using binstarjs03.AerialOBJ.Core.WorldRegion;
using binstarjs03.AerialOBJ.WpfApp.UIElements.Components;
using binstarjs03.AerialOBJ.WpfApp.UIElements.Controls;

namespace binstarjs03.AerialOBJ.WpfApp;

public class ChunkWrapper
{
    private readonly Chunk _chunk;
    private readonly ViewportControlVM _viewport;
    private readonly string[,] _highestBlocks = Chunk.GenerateHighestBlocksBuffer();

    public Coords2 ChunkCoordsAbs => _chunk.CoordsAbs;
    public Coords2 ChunkCoordsRel => _chunk.CoordsRel;
    public string[,] HighestBlocks => _highestBlocks;

    public ChunkWrapper(Chunk chunk, ViewportControlVM viewport)
    {
        _chunk = chunk;
        _viewport = viewport;
    }
}

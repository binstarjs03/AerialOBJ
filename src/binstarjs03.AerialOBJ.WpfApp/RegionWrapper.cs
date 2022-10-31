using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using binstarjs03.AerialOBJ.Core.CoordinateSystem;
using binstarjs03.AerialOBJ.Core.WorldRegion;

namespace binstarjs03.AerialOBJ.WpfApp;
public class RegionWrapper
{
    private readonly Region _region;
    private readonly Coords2 _regionCoords;
    private readonly HashSet<Coords2> _generatedChunks;

    public RegionWrapper(Region region)
    {
        _region = region;
        _regionCoords = region.Coords;
        _generatedChunks = region.GetGeneratedChunksAsCoordsRelSet();
    }

    public Coords2 RegionCoords => _regionCoords;

    public Chunk GetChunk(Coords2 chunkCoords, bool relative)
    {
        return _region.GetChunk(chunkCoords, relative);
    }

    public bool HasChunkGenerated(Coords2 chunkCoordsRel)
    {
        return _generatedChunks.Contains(chunkCoordsRel);
    }
}

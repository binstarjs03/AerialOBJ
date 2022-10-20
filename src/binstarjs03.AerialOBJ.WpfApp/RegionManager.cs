using System;
using System.Collections.Generic;

using binstarjs03.AerialOBJ.Core;
using binstarjs03.AerialOBJ.Core.CoordinateSystem;
using binstarjs03.AerialOBJ.Core.WorldRegion;
using binstarjs03.AerialOBJ.WpfApp.Services;

using Range = binstarjs03.AerialOBJ.Core.Range;

namespace binstarjs03.AerialOBJ.WpfApp;

public class RegionManager
{
    private readonly Dictionary<Coords2, RegionWrapper> _regions = new();

    public RegionManager() { }

    public Chunk? GetChunk(Coords2 chunkCoordsAbs)
    {
        Coords2 regionCoords = new((int)MathF.Floor((float)chunkCoordsAbs.X / Region.ChunkCount),
                                   (int)MathF.Floor((float)chunkCoordsAbs.Z / Region.ChunkCount));
        if (!_regions.ContainsKey(regionCoords))
            throw new ArgumentOutOfRangeException(nameof(chunkCoordsAbs), 
                "chunk is outside loaded region dictionary!");
        Region? region = _regions[regionCoords].Region;
        if (region is null)
            return null;
        Coords2 chunkCoordsRel = Region.ConvertChunkCoordsAbsToRel(chunkCoordsAbs);
        if (!region.HasChunkGenerated(chunkCoordsRel))
            return null;
        return region.GetChunk(chunkCoordsAbs, relative: false);
    }

    public void Update(CoordsRange2 visibleChunkRange)
    {
        CoordsRange2 visibleRegionRange = GetVisibleRegionRange(visibleChunkRange);
        Range regionX = visibleRegionRange.XRange;
        Range regionZ = visibleRegionRange.ZRange;

        List<Coords2> deallocatedRegionsPos = new();
        List<Coords2> allocatedRegionsPos = new();

        // perform boundary checking for regions outside visible range
        foreach (Coords2 regionCoords in _regions.Keys)
        {
            if (visibleRegionRange.IsInside(regionCoords))
                continue;
            else
            {
                deallocatedRegionsPos.Add(regionCoords);
            }
        }

        // perform sweep checking for regions inside visible range
        for (int x = regionX.Min; x <= regionX.Max; x++)
        {
            for (int z = regionZ.Min; z <= regionZ.Max; z++)
            {
                Coords2 regionCoords = new(x, z);
                if (_regions.ContainsKey(regionCoords))
                    continue;
                else
                {
                    allocatedRegionsPos.Add(regionCoords);
                }
            }
        }

        // deallocate
        foreach (Coords2 regionCoords in deallocatedRegionsPos)
        {
            Region? region = _regions[regionCoords].Region;
            _regions.Remove(regionCoords);
            if (region is not null)
                region.Dispose();
        }

        // allocate
        foreach (Coords2 regionCoords in allocatedRegionsPos)
        {
            RegionWrapper regionWrapper = new(IOService.LoadRegion(regionCoords));
            _regions.Add(regionCoords, regionWrapper);
        }
    }

    private static CoordsRange2 GetVisibleRegionRange(CoordsRange2 visibleChunkRange)
    {
        int regionMinX = (int)MathF.Floor((float)visibleChunkRange.XRange.Min / Region.ChunkCount);
        int regionMaxX = (int)MathF.Floor((float)visibleChunkRange.XRange.Max / Region.ChunkCount);

        int regionMinZ = (int)MathF.Floor((float)visibleChunkRange.ZRange.Min / Region.ChunkCount);
        int regionMaxZ = (int)MathF.Floor((float)visibleChunkRange.ZRange.Max / Region.ChunkCount);

        CoordsRange2 visibleRegionRange = new(regionMinX, regionMaxX, regionMinZ, regionMaxZ);
        return visibleRegionRange;
    }
}

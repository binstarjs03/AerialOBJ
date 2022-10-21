using System;
using System.Collections.Generic;
using System.Threading;

using binstarjs03.AerialOBJ.Core;
using binstarjs03.AerialOBJ.Core.CoordinateSystem;
using binstarjs03.AerialOBJ.Core.WorldRegion;
using binstarjs03.AerialOBJ.WpfApp.Services;

using Range = binstarjs03.AerialOBJ.Core.Range;

namespace binstarjs03.AerialOBJ.WpfApp;

public class RegionManager
{
    private readonly Dictionary<Coords2, RegionWrapper> _regionWrappers = new();

    public RegionManager() { }

    public RegionWrapper? GetRegionWrapper(Coords2 chunkCoordsAbs)
    {
        Coords2 regionCoords = new((int)MathF.Floor((float)chunkCoordsAbs.X / Region.ChunkCount),
                                   (int)MathF.Floor((float)chunkCoordsAbs.Z / Region.ChunkCount));
        if (!_regionWrappers.ContainsKey(regionCoords))
            return null;
        return _regionWrappers[regionCoords];
    }

    public Chunk? GetChunk(Coords2 chunkCoordsAbs)
    {
        Coords2 regionCoords = new((int)MathF.Floor((float)chunkCoordsAbs.X / Region.ChunkCount),
                                   (int)MathF.Floor((float)chunkCoordsAbs.Z / Region.ChunkCount));
        if (!_regionWrappers.ContainsKey(regionCoords))
            return null;
        RegionWrapper regionWrapper = _regionWrappers[regionCoords];
        Region? region = regionWrapper.Region;
        if (region is null)
            return null;
        Coords2 chunkCoordsRel = Region.ConvertChunkCoordsAbsToRel(chunkCoordsAbs);
        if (!region.HasChunkGenerated(chunkCoordsRel))
            return null;
        return region.GetChunk(chunkCoordsAbs, relative: false);
    }

    public bool CanGetChunk(Coords2 chunkCoordsAbs)
    {
        Coords2 regionCoords = new((int)MathF.Floor((float)chunkCoordsAbs.X / Region.ChunkCount),
                                   (int)MathF.Floor((float)chunkCoordsAbs.Z / Region.ChunkCount));
        if (!_regionWrappers.ContainsKey(regionCoords))
            return false;
        RegionWrapper regionWrapper = _regionWrappers[regionCoords];
        Region? region = regionWrapper.Region;
        if (region is null)
            return false;
        Coords2 chunkCoordsRel = Region.ConvertChunkCoordsAbsToRel(chunkCoordsAbs);
        if (!region.HasChunkGenerated(chunkCoordsRel))
            return false;
        return true;
    }

    public void Update(CoordsRange2 visibleChunkRange)
    {
        CoordsRange2 visibleRegionRange = GetVisibleRegionRange(visibleChunkRange);
        Range regionX = visibleRegionRange.XRange;
        Range regionZ = visibleRegionRange.ZRange;

        //List<Coords2> deallocatedRegionsPos = new();
        List<Coords2> allocatedRegionsPos = new();

        // perform boundary checking for regions outside visible range
        //foreach (Coords2 regionCoordsAbs in _regionWrappers.Keys)
        //{
        //    if (visibleRegionRange.IsInside(regionCoordsAbs))
        //        continue;
        //    else
        //    {
        //        deallocatedRegionsPos.Add(regionCoordsAbs);
        //    }
        //}

        // perform sweep checking for regions inside visible range
        for (int x = regionX.Min; x <= regionX.Max; x++)
        {
            for (int z = regionZ.Min; z <= regionZ.Max; z++)
            {
                Coords2 regionCoordsAbs = new(x, z);
                if (_regionWrappers.ContainsKey(regionCoordsAbs))
                    continue;
                else
                {
                    allocatedRegionsPos.Add(regionCoordsAbs);
                }
            }
        }

        // deallocate
        //foreach (Coords2 regionCoordsAbs in deallocatedRegionsPos)
        //{
        //    RegionWrapper regionWrapper = _regionWrappers[regionCoordsAbs];
        //    _regionWrappers.Remove(regionCoordsAbs);
        //    regionWrapper.Deallocate();
        //}

        // allocate
        foreach (Coords2 regionCoordsAbs in allocatedRegionsPos)
        {
            RegionWrapper regionWrapper = new(regionCoordsAbs);
            _regionWrappers.Add(regionCoordsAbs, regionWrapper);
            regionWrapper.Allocate();
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

    public void OnSessionClosed()
    {
        foreach (RegionWrapper region in _regionWrappers.Values)
        {
            if (region.Region is not null)
                region.Region.Dispose();
            _regionWrappers.Clear();
        }
    }
}

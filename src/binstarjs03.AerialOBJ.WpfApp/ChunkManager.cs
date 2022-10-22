using System;
using System.Collections.Generic;

using binstarjs03.AerialOBJ.Core.CoordinateSystem;
using binstarjs03.AerialOBJ.Core.WorldRegion;
using binstarjs03.AerialOBJ.WpfApp.UIElements.Controls;

using Range = binstarjs03.AerialOBJ.Core.Range;

namespace binstarjs03.AerialOBJ.WpfApp;

public class ChunkManager
{
    private readonly ViewportControlVM _viewport;
    private readonly RegionManager _regionManager = new();
    private readonly Dictionary<Coords2, ChunkWrapper> _chunks = new();

    private CoordsRange2 _visibleChunkRange;
    private bool _needReallocate = false;

    public ChunkManager(ViewportControlVM viewport)
    {
        _viewport = viewport;
    }

    // public accessors
    public ViewportControlVM Viewport => _viewport;
    public RegionManager RegionManager => _regionManager;
    public CoordsRange2 VisibleChunkRange => _visibleChunkRange;
    public CoordsRange2 VisibleRegionRange => GetVisibleRegionRange();
    public int VisibleChunkCount => _chunks.Count;
    public int LoadedChunkCount => _viewport.Control.ChunkCanvas.Children.Count;

    private CoordsRange2 GetVisibleRegionRange()
    {
        CoordsRange2 vcr = _visibleChunkRange;

        int regionMinX = vcr.XRange.Min / Region.ChunkCount;
        int regionMaxX = vcr.XRange.Max / Region.ChunkCount;

        int regionMinZ = vcr.ZRange.Min / Region.ChunkCount;
        int regionMaxZ = vcr.ZRange.Max / Region.ChunkCount;

        CoordsRange2 visibleRegionRange = new(regionMinX, regionMaxX, regionMinZ, regionMaxZ);
        return visibleRegionRange;
    }

    public void Update()
    {
        if (SharedProperty.SessionInfo is null)
            return;
        UpdateVisibleChunkRange();
        ReallocateChunks();
        UpdateChunks();
    }

    private void UpdateVisibleChunkRange()
    {
        ViewportControlVM v = _viewport;

        // camera chunk in here means which chunk the camera is pointing to
        // here we dont use int because floating point accuracy is crucial
        double xCameraChunk = v.ViewportCameraPos.X / Section.BlockCount;
        double zCameraChunk = v.ViewportCameraPos.Y / Section.BlockCount;

        // min/maxCanvasCenterChunk means which chunk that is visible at the edgemost of the control
        // that is measured by the length/height of the control size (which is chunk canvas)
        double minXCanvasCenterChunk = -(v.ViewportChunkCanvasCenter.X / v.ViewportPixelPerChunk);
        double maxXCanvasCenterChunk = v.ViewportChunkCanvasCenter.X / v.ViewportPixelPerChunk;

        int minX = (int)Math.Floor(Math.Round(xCameraChunk + minXCanvasCenterChunk, 3));
        int maxX = (int)Math.Floor(Math.Round(xCameraChunk + maxXCanvasCenterChunk, 3));
        Range visibleChunkXRange = new(minX, maxX);

        double minZCanvasCenterChunk = -(v.ViewportChunkCanvasCenter.Y / v.ViewportPixelPerChunk);
        double maxZCanvasCenterChunk = v.ViewportChunkCanvasCenter.Y / v.ViewportPixelPerChunk;

        int minZ = (int)Math.Floor(Math.Round(zCameraChunk + minZCanvasCenterChunk, 3));
        int maxZ = (int)Math.Floor(Math.Round(zCameraChunk + maxZCanvasCenterChunk, 3));
        Range visibleChunkZRange = new(minZ, maxZ);

        CoordsRange2 oldVisibleChunkRange = _visibleChunkRange;
        CoordsRange2 newVisibleChunkRange = new(visibleChunkXRange, visibleChunkZRange);
        if (newVisibleChunkRange == oldVisibleChunkRange)
            return;
        _visibleChunkRange = newVisibleChunkRange;
        _regionManager.Update(newVisibleChunkRange);
        _needReallocate = true;

        string[] propertyNames = new string[]
        {
            nameof(v.ChunkManagerVisibleChunkRangeXBinding),
            nameof(v.ChunkManagerVisibleChunkRangeZBinding),
            nameof(v.ChunkManagerVisibleRegionRangeXBinding),
            nameof(v.ChunkManagerVisibleRegionRangeZBinding),
        };
        v.NotifyPropertyChanged(propertyNames);
    }

    private void ReallocateChunks()
    {
        if (_needReallocate)
        {
            List<Coords2> deallocatedChunksPos = new(30);
            List<Coords2> allocatedChunksPos = new(30);

            // perform boundary checking for chunks outside display frame
            foreach (Coords2 chunkPos in _chunks.Keys)
                if (!_visibleChunkRange.IsInside(chunkPos))
                    deallocatedChunksPos.Add(chunkPos);

            // perform sweep checking for chunks inside display frame
            for (int x = _visibleChunkRange.XRange.Min; x <= _visibleChunkRange.XRange.Max; x++)
                for (int z = _visibleChunkRange.ZRange.Min; z <= _visibleChunkRange.ZRange.Max; z++)
                {
                    Coords2 chunkCoords = new(x, z);
                    if (!_chunks.ContainsKey(chunkCoords))
                        allocatedChunksPos.Add(chunkCoords);
                }

            // deallocate
            foreach (Coords2 chunkPos in deallocatedChunksPos)
            {
                ChunkWrapper chunk = _chunks[chunkPos];
                _chunks.Remove(chunkPos);
                chunk.Deallocate();
            }

            // allocate
            foreach (Coords2 chunkPos in allocatedChunksPos)
            {
                ChunkWrapper chunk = new(chunkPos, this);
                if (chunk.Allocate()) // only add to buffer if can allocate
                    _chunks.Add(chunkPos, chunk);
            }

            _viewport.NotifyPropertyChanged(nameof(_viewport.ChunkManagerVisibleChunkCount));
            _needReallocate = false;
        }
    }

    private void UpdateChunks()
    {
        foreach (ChunkWrapper chunk in _chunks.Values)
            chunk.Update();
    }

    public void OnSessionClosed()
    {
        foreach (ChunkWrapper chunk in _chunks.Values)
            chunk.Deallocate();
        _chunks.Clear();
        // reset visible chunk range to zero, this will ensure next
        // update will set visible chunk range to different value,
        // setting needreallocate to true, in turns allowing chunk reallocation
        _visibleChunkRange = new CoordsRange2();
        _regionManager.OnSessionClosed();
    }
}
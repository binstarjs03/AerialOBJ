using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Threading;

using binstarjs03.AerialOBJ.Core.CoordinateSystem;
using binstarjs03.AerialOBJ.Core.MinecraftWorld;
using binstarjs03.AerialOBJ.WpfApp.Converters;
using binstarjs03.AerialOBJ.WpfApp.Services;
using binstarjs03.AerialOBJ.WpfApp.UIElements.Controls;

using Range = binstarjs03.AerialOBJ.Core.Range;
using Region = binstarjs03.AerialOBJ.Core.MinecraftWorld.Region;

namespace binstarjs03.AerialOBJ.WpfApp;

public class ChunkRegionManager
{
    // twelve should be more than enough to avoid collection resizing,
    // unless for huge monitor resolution at zoom level 0 of course
    private const int s_regionBufferSize = 15;
    private const int s_chunkBufferSize = s_regionBufferSize * Region.TotalChunkCount;

    private readonly ViewportControlVM _viewport;

    private readonly Dictionary<Coords2, RegionWrapper> _loadedRegions = new(s_regionBufferSize);
    private readonly Dictionary<Coords2, ChunkWrapper> _loadedChunks = new(s_regionBufferSize * Region.TotalChunkCount);

    private readonly Random _rng = new();
    private readonly HashSet<Coords2> _pendingChunkSet = new(s_chunkBufferSize);
    private readonly List<Coords2> _pendingChunkList = new(s_chunkBufferSize);
    private readonly List<Coords2> _workedChunks = new(Environment.ProcessorCount);

    private CoordsRange2 _visibleRegionRange = new();
    private CoordsRange2 _visibleChunkRange = new();
    private int _runningChunkWorkerThreadCount = 0;
    private int _displayedHeightLimit;

    public ChunkRegionManager(ViewportControlVM viewport)
    {
        _viewport = viewport;
    }

    // public accessors
    public ViewportControlVM Viewport => _viewport;
    public CoordsRange2 VisibleRegionRange => _visibleRegionRange;
    public CoordsRange2 VisibleChunkRange => _visibleChunkRange;
    public int VisibleRegionCount => _visibleRegionRange.Sum;
    public int VisibleChunkCount => _visibleChunkRange.Sum;
    public int LoadedRegionCount => _loadedRegions.Count;
    public int LoadedChunkCount => _loadedChunks.Count;
    public int PendingChunkCount => _pendingChunkList.Count;
    public int WorkedChunkCount => _workedChunks.Count;


    public void Update()
    {
        if (App.CurrentCast.Properties.SessionInfo is null)
            return;
        if (_displayedHeightLimit != _viewport.ViewportHeightLimit)
            ClearChunks();
        _displayedHeightLimit = _viewport.ViewportHeightLimit;
        RecalculateVisibleChunkRange(out bool chunkRangeChanged);
        if (chunkRangeChanged)
        {
            _viewport.NotifyPropertyChanged(nameof(ViewportControlVM.ChunkRegionManagerVisibleChunkCount));
            _viewport.NotifyPropertyChanged(nameof(ViewportControlVM.ChunkRegionManagerVisibleChunkRangeXStringized));
            _viewport.NotifyPropertyChanged(nameof(ViewportControlVM.ChunkRegionManagerVisibleChunkRangeZStringized));
            RecalculateVisibleRegionRange(out bool regionRangeChanged);
            if (regionRangeChanged)
            {
                _viewport.NotifyPropertyChanged(nameof(ViewportControlVM.ChunkRegionManagerVisibleRegionCount));
                _viewport.NotifyPropertyChanged(nameof(ViewportControlVM.ChunkRegionManagerVisibleRegionRangeXStringized));
                _viewport.NotifyPropertyChanged(nameof(ViewportControlVM.ChunkRegionManagerVisibleRegionRangeZStringized));
                LoadUnloadRegions();
            }
            LoadUnloadChunks();
        }
        UpdateChunks();
    }

    private void RecalculateVisibleChunkRange(out bool chunkRangeChanged)
    {
        ViewportControlVM v = _viewport;

        // Camera chunk in here means which chunk the camera is pointing to.
        // Here we dont use int because floating point accuracy is crucial
        double xCameraChunk = v.ViewportCameraPos.X / Section.BlockCount;
        double zCameraChunk = v.ViewportCameraPos.Y / Section.BlockCount;

        // min/maxCanvasCenterChunk means which chunk that is visible at the edgemost of the control
        // that is measured by the length or height of the control size (which is chunk canvas)
        double minXCanvasCenterChunk = -(v.ViewportChunkCanvasCenter.X / v.ViewportPixelPerChunk);
        double maxXCanvasCenterChunk = v.ViewportChunkCanvasCenter.X / v.ViewportPixelPerChunk;
        int minX = (int)Math.Floor(xCameraChunk + minXCanvasCenterChunk);
        int maxX = (int)Math.Floor(xCameraChunk + maxXCanvasCenterChunk);
        Range visibleChunkXRange = new(minX, maxX);

        double minZCanvasCenterChunk = -(v.ViewportChunkCanvasCenter.Y / v.ViewportPixelPerChunk);
        double maxZCanvasCenterChunk = v.ViewportChunkCanvasCenter.Y / v.ViewportPixelPerChunk;
        int minZ = (int)Math.Floor(zCameraChunk + minZCanvasCenterChunk);
        int maxZ = (int)Math.Floor(zCameraChunk + maxZCanvasCenterChunk);
        Range visibleChunkZRange = new(minZ, maxZ);

        CoordsRange2 oldVisibleChunkRange = _visibleChunkRange;
        CoordsRange2 newVisibleChunkRange = new(visibleChunkXRange, visibleChunkZRange);
        if (newVisibleChunkRange == oldVisibleChunkRange)
            chunkRangeChanged = false;
        else
        {
            chunkRangeChanged = true;
            _visibleChunkRange = newVisibleChunkRange;
        }
    }

    private void RecalculateVisibleRegionRange(out bool regionRangeChanged)
    {
        CoordsRange2 vcr = _visibleChunkRange;

        // Calculating region range is easier since we only need to
        // divide the range by how many chunks in region (in single axis)

        int regionMinX = (int)MathF.Floor((float)vcr.XRange.Min / Region.ChunkCount);
        int regionMaxX = (int)MathF.Floor((float)vcr.XRange.Max / Region.ChunkCount);
        Range visibleRegionXRange = new(regionMinX, regionMaxX);

        int regionMinZ = (int)MathF.Floor((float)vcr.ZRange.Min / Region.ChunkCount);
        int regionMaxZ = (int)MathF.Floor((float)vcr.ZRange.Max / Region.ChunkCount);
        Range visibleRegionZRange = new(regionMinZ, regionMaxZ);

        CoordsRange2 oldVisibleRegionRange = _visibleRegionRange;
        CoordsRange2 newVisibleRegionRange = new(visibleRegionXRange, visibleRegionZRange);
        if (newVisibleRegionRange == oldVisibleRegionRange)
            regionRangeChanged = false;
        else
        {
            regionRangeChanged = true;
            _visibleRegionRange = newVisibleRegionRange;
        }
    }

    private void LoadUnloadRegions()
    {
        // perform boundary range checking for regions outside display frame
        // unload them if outside
        foreach (Coords2 regionCoords in _loadedRegions.Keys)
        {
            if (_visibleRegionRange.IsInside(regionCoords))
                continue;
            RegionWrapper regionWrapper = _loadedRegions[regionCoords];
            UnloadRegion(regionWrapper);
        }

        // perform sweep checking from min to max range for visible regions
        // read region files in all visible range and load them if not loaded yet
        for (int x = _visibleRegionRange.XRange.Min; x <= _visibleRegionRange.XRange.Max; x++)
            for (int z = _visibleRegionRange.ZRange.Min; z <= _visibleRegionRange.ZRange.Max; z++)
            {
                Coords2 regionCoords = new(x, z);
                if (_loadedRegions.ContainsKey(regionCoords))
                    continue;
                Region? region = IOService.ReadRegionFile(regionCoords, out Exception? e);

                // cancel allocation if we can't get the region at specified coords
                // (corrupted, file not exist or not generated yet etc)
                if (region is null)
                {
                    if (e is not null)
                    {
                        // TODO display messagebox only once and never again
                        LogService.LogError($"Region {regionCoords} was skipped.");
                        LogService.LogError($"Cause of exception: {e.GetType()}");
                        LogService.LogError($"Exception details: {e}", useSeparator: true);
                    }
                    continue;
                }
                LoadRegion(region);
            }
        _viewport.NotifyPropertyChanged(nameof(ViewportControlVM.ChunkRegionManagerPendingChunkCount));
    }

    private void LoadRegion(Region region)
    {
        // cancel region loading if specified region is outside view screen
        if (!_visibleRegionRange.IsInside(region.RegionCoords))
            return;

        // cancel region loading if specified region already loaded (no duplicate).
        // Since nobody referencing that region, the GC will collect it eventually.
        if (_loadedRegions.ContainsKey(region.RegionCoords))
            return;
        RegionWrapper regionWrapper = new(region);
        _loadedRegions.Add(region.RegionCoords, regionWrapper);
        OnRegionLoadChanged();
    }
    private void UnloadRegion(RegionWrapper regionWrapper)
    {
        _loadedRegions.Remove(regionWrapper.RegionCoords);
        OnRegionLoadChanged();
    }

    private void OnRegionLoadChanged()
    {
        _viewport.NotifyPropertyChanged(nameof(ViewportControlVM.ChunkRegionManagerRenderedChunkCount));
        _viewport.NotifyPropertyChanged(nameof(ViewportControlVM.ChunkRegionManagerRenderedRegionCount));
    }

    private void LoadUnloadChunks()
    {
        // perform boundary range checking for chunks outside display frame
        foreach (Coords2 chunkCoordsAbs in _loadedChunks.Keys)
        {
            if (_visibleChunkRange.IsInside(chunkCoordsAbs))
                continue;
            UnloadChunk(chunkCoordsAbs);
        }

        // remove all pending chunk queue that is no longer visible
        _pendingChunkSet.RemoveWhere(
            chunkCoordsAbs => !_visibleChunkRange.IsInside(chunkCoordsAbs));
        _pendingChunkList.RemoveAll(
            chunkCoordsAbs => !_visibleChunkRange.IsInside(chunkCoordsAbs));

        // perform sweep-checking from min range to max range for chunks inside display frame
        for (int x = _visibleChunkRange.XRange.Min; x <= _visibleChunkRange.XRange.Max; x++)
            for (int z = _visibleChunkRange.ZRange.Min; z <= _visibleChunkRange.ZRange.Max; z++)
            {
                Coords2 chunkCoordsAbs = new(x, z);
                if (_loadedChunks.ContainsKey(chunkCoordsAbs))
                    continue;
                // set is a whole lot faster to check for item existence
                // if the content has hundreds of items, especially for
                // tight-loop like this (approx. millions of comparison performed)
                if (_pendingChunkSet.Contains(chunkCoordsAbs))
                    continue;
                if (_workedChunks.Contains(chunkCoordsAbs))
                    continue;
                _pendingChunkSet.Add(chunkCoordsAbs);
                _pendingChunkList.Add(chunkCoordsAbs);
            }
        _viewport.NotifyPropertyChanged(nameof(ViewportControlVM.ChunkRegionManagerPendingChunkCount));
        InitiateChunkAllocation();
    }

    private void LoadChunk(ChunkWrapper chunkWrapper)
    {
        if (!_visibleChunkRange.IsInside(chunkWrapper.ChunkCoordsAbs))
            return;
        if (_loadedChunks.ContainsKey(chunkWrapper.ChunkCoordsAbs))
            return;
        _loadedChunks.Add(chunkWrapper.ChunkCoordsAbs, chunkWrapper);
        _viewport.Control.ChunkCanvas.Children.Add(chunkWrapper.ChunkImage);
        OnChunkLoadChanged();
    }

    public void UnloadChunk(Coords2 chunkCoordsAbs)
    {
        if (!_loadedChunks.ContainsKey(chunkCoordsAbs))
            return;
        ChunkWrapper chunkWrapper = _loadedChunks[chunkCoordsAbs];
        _loadedChunks.Remove(chunkWrapper.ChunkCoordsAbs);
        _viewport.Control.ChunkCanvas.Children.Remove(chunkWrapper.ChunkImage);
        OnChunkLoadChanged();
    }

    private void OnChunkLoadChanged()
    {
        _viewport.NotifyPropertyChanged(nameof(ViewportControlVM.ChunkRegionManagerRenderedChunkCount));
    }

    private RegionWrapper? GetRegionWrapper(Coords2 chunkCoordsAbs)
    {
        Coords2 regionCoords = Region.GetRegionCoordsFromChunkCoordsAbs(chunkCoordsAbs);
        if (_loadedRegions.ContainsKey(regionCoords))
            return _loadedRegions[regionCoords];
        return null;
    }

    private void InitiateChunkAllocation()
    {
        // TODO set max chunk worker thread count according to user setting chunk threads!
        while (_runningChunkWorkerThreadCount < Environment.ProcessorCount)
        {
            if (_pendingChunkSet.Count == 0)
                break;
            int index = _rng.Next(0, _pendingChunkSet.Count);
            Coords2 chunkCoordsAbs = _pendingChunkList[index];
            _pendingChunkSet.Remove(chunkCoordsAbs);
            _pendingChunkList.RemoveAt(index);

            // Check if region is loaded. Theoretically, all pending chunks have
            // their underlying regions but this is added for extra safety.
            // Simply don't do anything if the region for particular chunk
            // is not loaded (e.g culled or whatnot)
            RegionWrapper? regionWrapper = GetRegionWrapper(chunkCoordsAbs);
            Coords2 chunkCoordsRel = Region.ConvertChunkCoordsAbsToRel(chunkCoordsAbs);
            if (regionWrapper is not null && regionWrapper.HasChunkGenerated(chunkCoordsRel))
            {
                _workedChunks.Add(chunkCoordsAbs);
                Task.Run(() => AllocateChunkAsync(chunkCoordsAbs));
                _runningChunkWorkerThreadCount++;
            }
        }
        _viewport.NotifyPropertyChanged(nameof(ViewportControlVM.ChunkRegionManagerPendingChunkCount));
        _viewport.NotifyPropertyChanged(nameof(ViewportControlVM.ChunkRegionManagerWorkedChunkCount));
    }

    private void AllocateChunkAsync(Coords2 chunkCoordsAbs)
    {
        // locking is unneccessary because chunk may not be visible anymore
        // at any point in time after this statement, so its irrelevant
        if (!_visibleChunkRange.IsInside(chunkCoordsAbs))
        {
            // if this method wants to return (aborting), we have to remove the chunk coordinate
            // this thread is working on in the list, and we do it on main thread using dispatcher
            App.CurrentCast?.Dispatcher.BeginInvoke(
                () => OnAllocateChunkAsyncExit(chunkCoordsAbs),
                DispatcherPriority.Normal);
            return;
        }

        RegionWrapper? regionWrapper = GetRegionWrapper(chunkCoordsAbs);
        if (regionWrapper is null)
        {
            App.CurrentCast?.Dispatcher.BeginInvoke(
                () => OnAllocateChunkAsyncExit(chunkCoordsAbs),
                DispatcherPriority.Normal);
            return;
        }
        Chunk chunk = regionWrapper.GetChunk(chunkCoordsAbs, relative: false);
        Bitmap chunkImage = new(16, 16, PixelFormat.Format32bppArgb);
        chunkImage.MakeTransparent();
        string[,] highestBlocks = Chunk.GenerateHighestBlocksBuffer();

        // we also do not lock height limit, and if the height limit is changed
        // while this thread is rendering on different height limit,
        // it will not be added to viewport (see AllocateChunkSynchronized method)
        int renderedHeight = _viewport.ViewportHeightLimit;
        chunk.GetHighestBlock(highestBlocks, heightLimit: renderedHeight);
        for (int x = 0; x < Section.BlockCount; x++)
            for (int z = 0; z < Section.BlockCount; z++)
                chunkImage.SetPixel(x, z, BlockToColor.Convert(highestBlocks[x, z]));
        MemoryStream chunkImageStream = new();
        // TODO consideration. Png may have transparency, lesser memory footprint,
        // but using Bmp is a lot faster than Png, but no transparency and
        // larger memory footprint. We will see if it does matter.
        chunkImage.Save(chunkImageStream, ImageFormat.Bmp);

        App.CurrentCast?.Dispatcher.BeginInvoke(
            () => OnAllocateChunkAsyncExit(chunkCoordsAbs),
            DispatcherPriority.Background);

        App.CurrentCast?.Dispatcher.BeginInvoke(
            () => AllocateChunkSynchronized(chunkCoordsAbs, chunkImageStream, renderedHeight),
            DispatcherPriority.Background);
    }

    private void OnAllocateChunkAsyncExit(Coords2 chunkCoordsAbs)
    {
        _runningChunkWorkerThreadCount--;
        _workedChunks.Remove(chunkCoordsAbs);
        _viewport.NotifyPropertyChanged(nameof(ViewportControlVM.ChunkRegionManagerWorkedChunkCount));
        // rerun chunk allocation to see if there
        // is still pending chunk to be processed
        InitiateChunkAllocation();
    }

    private void AllocateChunkSynchronized(Coords2 chunkCoordsAbs, MemoryStream chunkImageStream, int renderedHeight)
    {
        // discard this chunk if session is no longer exist
        if (App.CurrentCast.Properties.SessionInfo is null)
            return;

        // discard this chunk if it is no longer visible
        if (!_visibleChunkRange.IsInside(chunkCoordsAbs))
            return;

        // discard this chunk if it was rendered on different height limit
        // than the display is currently displaying
        if (renderedHeight != _displayedHeightLimit)
            return;
        ChunkWrapper chunkWrapper = new(chunkCoordsAbs, this, chunkImageStream);
        LoadChunk(chunkWrapper);
    }

    private void ClearChunks()
    {
        foreach (Coords2 chunkCoordsAbs in _loadedChunks.Keys)
            UnloadChunk(chunkCoordsAbs);
        _loadedChunks.Clear();
        _pendingChunkSet.Clear();
        _pendingChunkList.Clear();
    }

    private void UpdateChunks()
    {
        foreach (ChunkWrapper chunk in _loadedChunks.Values)
            chunk.Update();
    }

    public void OnSessionClosed()
    {
        _loadedRegions.Clear();
        _visibleChunkRange = new CoordsRange2();
    }
}

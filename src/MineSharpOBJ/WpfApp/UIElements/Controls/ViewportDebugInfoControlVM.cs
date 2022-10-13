namespace binstarjs03.MineSharpOBJ.WpfApp.UIElements.Controls;
public class ViewportDebugInfoControlVM : ViewModelBase<ViewportDebugInfoControlVM, ViewportDebugInfoControl>
{
    public ViewportDebugInfoControlVM(ViewportDebugInfoControl control) : base(control)
    {
        // initialize states
        Reinitialize();
    }

    public static ViewportDebugInfoControlVM? Context { get; set; }

    // Data Binders -----------------------------------------------------------

    private string _viewportCameraPos = "";
    private string _viewportChunkPosOffset = "";
    private string _viewportZoomLevel = "";
    private string _viewportPixelPerBlock = "";
    private string _viewportPixelPerChunk = "";

    private string _mousePos = "";
    private string _mousePosDelta = "";
    private string _mouseIsClickHolding = "";
    private string _mouseIsOutside = "";

    private string _chunkServiceVisibleChunkCount = "";
    private string _chunkServiceVisibleChunkXRange = "";
    private string _chunkServiceVisibleChunkZRange = "";
    private string _chunkServiceLoadedChunkCount = "";
    private string _chunkServicePendingChunkCount = "";



    #region Data Binders

    #region Viewport Group
    public string ViewportCameraPos
    {
        get => _viewportCameraPos;
        set => SetAndNotifyPropertyChanged(value, ref _viewportCameraPos);
    }
    public string ViewportChunkPosOffset
    {
        get => _viewportChunkPosOffset;
        set => SetAndNotifyPropertyChanged(value, ref _viewportChunkPosOffset);
    }
    public string ViewportZoomLevel
    {
        get => _viewportZoomLevel;
        set => SetAndNotifyPropertyChanged(value, ref _viewportZoomLevel);
    }
    public string ViewportPixelPerBlock
    {
        get => _viewportPixelPerBlock;
        set => SetAndNotifyPropertyChanged(value, ref _viewportPixelPerBlock);
    }
    public string ViewportPixelPerChunk
    {
        get => _viewportPixelPerChunk;
        set => SetAndNotifyPropertyChanged(value, ref _viewportPixelPerChunk);
    }
    #endregion



    #region Mouse Group
    public string MousePos
    {
        get => _mousePos;
        set => SetAndNotifyPropertyChanged(value, ref _mousePos);
    }
    public string MousePosDelta
    {
        get => _mousePosDelta;
        set => SetAndNotifyPropertyChanged(value, ref _mousePosDelta);
    }
    public string MouseIsClickHolding
    {
        get => _mouseIsClickHolding;
        set => SetAndNotifyPropertyChanged(value, ref _mouseIsClickHolding);
    }
    public string MouseIsOutside
    {
        get => _mouseIsOutside;
        set => SetAndNotifyPropertyChanged(value, ref _mouseIsOutside);
    }
    #endregion



    #region Chunk Service Group
    public string ChunkServiceVisibleChunkCount
    {
        get => _chunkServiceVisibleChunkCount;
        set => SetAndNotifyPropertyChanged(value, ref _chunkServiceVisibleChunkCount);
    }
    public string ChunkServiceVisibleChunkXRange
    {
        get => _chunkServiceVisibleChunkXRange;
        set => SetAndNotifyPropertyChanged(value, ref _chunkServiceVisibleChunkXRange);
    }
    public string ChunkServiceVisibleChunkZRange
    {
        get => _chunkServiceVisibleChunkZRange;
        set => SetAndNotifyPropertyChanged(value, ref _chunkServiceVisibleChunkZRange);
    }
    public string ChunkServiceLoadedChunkCount
    {
        get => _chunkServiceLoadedChunkCount;
        set => SetAndNotifyPropertyChanged(value, ref _chunkServiceLoadedChunkCount);
    }
    public string ChunkServicePendingChunkCount
    {
        get => _chunkServicePendingChunkCount;
        set => SetAndNotifyPropertyChanged(value, ref _chunkServicePendingChunkCount);
    }
    #endregion

    #endregion



    // Methods ----------------------------------------------------------------

    private void Reinitialize()
    {
        ViewportCameraPos = "Camera Pos:";
        ViewportChunkPosOffset = "Chunk Pos Offset:";
        ViewportZoomLevel = "Zoom Level:";
        ViewportPixelPerBlock = "Pixel-Per-Block:";
        ViewportPixelPerChunk = "Pixel-Per-Chunk:";

        MousePos = "Mouse Pos:";
        MousePosDelta = "Mouse Pos Delta:";
        MouseIsClickHolding = "Is Click Holding:";
        MouseIsOutside = "Is Outside:";

        ChunkServiceVisibleChunkCount = "Visible Chunk Count:";
        ChunkServiceVisibleChunkXRange = "- X Range:";
        ChunkServiceVisibleChunkZRange = "- Z Range:";
        ChunkServiceLoadedChunkCount = "Loaded Chunk Count:";
        ChunkServicePendingChunkCount = "Pending Chunk Count:";
    }
}

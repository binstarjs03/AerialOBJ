namespace binstarjs03.MineSharpOBJ.WpfApp.UIElements.Controls;
public class ViewportDebugInfoControlVM : ViewModelBase<ViewportDebugInfoControlVM, ViewportDebugInfoControl>
{
    public ViewportDebugInfoControlVM(ViewportDebugInfoControl control) : base(control)
    {
        // initialize states
        ReinitializeText();
    }

    public static ViewportDebugInfoControlVM? Context { get; set; }

    // States -----------------------------------------------------------------

    private string _viewportCameraPos = "";
    public string ViewportCameraPos
    {
        get => _viewportCameraPos;
        set => SetAndNotifyPropertyChanged(value, ref _viewportCameraPos);
    }

    private string _viewportChunkPosOffset = "";
    public string ViewportChunkPosOffset
    {
        get => _viewportChunkPosOffset;
        set => SetAndNotifyPropertyChanged(value, ref _viewportChunkPosOffset);
    }

    private string _viewportZoomLevel = "";
    public string ViewportZoomLevel
    {
        get => _viewportZoomLevel;
        set => SetAndNotifyPropertyChanged(value, ref _viewportZoomLevel);
    }

    private string _viewportPixelPerBlock = "";
    public string ViewportPixelPerBlock
    {
        get => _viewportPixelPerBlock;
        set => SetAndNotifyPropertyChanged(value, ref _viewportPixelPerBlock);
    }

    private string _viewportPixelPerChunk = "";
    public string ViewportPixelPerChunk
    {
        get => _viewportPixelPerChunk;
        set => SetAndNotifyPropertyChanged(value, ref _viewportPixelPerChunk);
    }

    private string _mousePos = "";
    public string MousePos
    {
        get => _mousePos;
        set => SetAndNotifyPropertyChanged(value, ref _mousePos);
    }

    private string _mousePosDelta = "";
    public string MousePosDelta
    {
        get => _mousePosDelta;
        set => SetAndNotifyPropertyChanged(value, ref _mousePosDelta);
    }

    private string _mouseIsClickHolding = "";
    public string MouseIsClickHolding
    {
        get => _mouseIsClickHolding;
        set => SetAndNotifyPropertyChanged(value, ref _mouseIsClickHolding);
    }

    private string _mouseIsOutside = "";
    public string MouseIsOutside
    {
        get => _mouseIsOutside;
        set => SetAndNotifyPropertyChanged(value, ref _mouseIsOutside);
    }

    // Methods ----------------------------------------------------------------

    private void ReinitializeText()
    {
        _viewportCameraPos = "Camera Pos:";
        _viewportChunkPosOffset = "Chunk Pos Offset:";
        _viewportZoomLevel = "Zoom Level:";
        _viewportPixelPerBlock = "Pixel-Per-Block:";
        _viewportPixelPerChunk = "Pixel-Per-Chunk:";

        _mousePos = "Mouse Pos:";
        _mousePosDelta = "Mouse Pos Delta:";
        _mouseIsClickHolding = "Is Click Holding:";
        _mouseIsOutside = "Is Outside:";
    }
}

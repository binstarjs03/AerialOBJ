using CommunityToolkit.Mvvm.ComponentModel;

namespace binstarjs03.AerialOBJ.WpfApp;

[ObservableObject]
public partial class ViewportSetting
{
    [ObservableProperty] private ChunkShadingStyle _chunkShadingStyle;

    public ViewportSetting(ChunkShadingStyle chunkShadingStyle)
    {
        _chunkShadingStyle = chunkShadingStyle;
    }

    public static ChunkShadingStyle DefaultChunkShadingStyle { get; } = ChunkShadingStyle.Standard;

    public static ViewportSetting GetDefaultSetting() => new(DefaultChunkShadingStyle);
}
using System;

using CommunityToolkit.Mvvm.ComponentModel;

namespace binstarjs03.AerialOBJ.WpfApp;

[ObservableObject]
public partial class ViewportSetting
{
    [ObservableProperty] private ChunkShadingStyle _chunkShadingStyle;
    [ObservableProperty] private int _chunkThreads;

    public ViewportSetting(ChunkShadingStyle chunkShadingStyle, int chunkThreads)
    {
        _chunkShadingStyle = chunkShadingStyle;
        _chunkThreads = chunkThreads;
    }

    public static ChunkShadingStyle DefaultChunkShadingStyle { get; } = ChunkShadingStyle.Standard;
    public static int DefaultChunkThreads { get; } = Math.Max(1, Environment.ProcessorCount - 1);
}

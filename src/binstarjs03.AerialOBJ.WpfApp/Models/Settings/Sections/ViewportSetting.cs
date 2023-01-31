using System;

using binstarjs03.AerialOBJ.Imaging.ChunkRendering;
using binstarjs03.AerialOBJ.WpfApp.Services.ChunkRendering;

using CommunityToolkit.Mvvm.ComponentModel;

namespace binstarjs03.AerialOBJ.WpfApp.Models.Settings;

[ObservableObject]
public partial class ViewportSetting
{
    [ObservableProperty] private IChunkShader _chunkShader;

    public ViewportSetting(IChunkShader chunkShader)
    {
        _chunkShader = chunkShader;
    }

    public event Action? ChunkShaderChanged;

    public static ViewportSetting GetDefaultSetting(IChunkShaderRepository repository) => new(repository.DefaultShader);

    partial void OnChunkShaderChanged(IChunkShader value) => ChunkShaderChanged?.Invoke();
}
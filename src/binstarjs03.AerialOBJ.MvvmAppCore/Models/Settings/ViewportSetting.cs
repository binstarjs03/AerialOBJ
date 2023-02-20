using System;

using binstarjs03.AerialOBJ.Imaging.ChunkRendering;
using binstarjs03.AerialOBJ.MvvmAppCore.Services.ChunkLoadingPatterns;
using binstarjs03.AerialOBJ.MvvmAppCore.Repositories;

using CommunityToolkit.Mvvm.ComponentModel;

namespace binstarjs03.AerialOBJ.MvvmAppCore.Models.Settings;
public partial class ViewportSetting : ObservableObject
{
    [ObservableProperty] private IChunkShader _chunkShader;
    [ObservableProperty] private IChunkLoadingPattern _chunkLoadingPattern;

    public ViewportSetting(IChunkShader chunkShader, IChunkLoadingPattern chunkLoadingPattern)
    {
        _chunkShader = chunkShader;
        _chunkLoadingPattern = chunkLoadingPattern;
    }

    public event Action? ChunkShaderChanged;

    public static ViewportSetting GetDefaultSetting(IRepository<IChunkShader> chunkShaderRepo, IRepository<IChunkLoadingPattern> chunkLoadingPatternRepo)
        => new(chunkShaderRepo.Default, chunkLoadingPatternRepo.Default);

    partial void OnChunkShaderChanged(IChunkShader value) => ChunkShaderChanged?.Invoke();
}
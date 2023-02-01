using System;

using binstarjs03.AerialOBJ.Core.Primitives;
using binstarjs03.AerialOBJ.Imaging.ChunkRendering;
using binstarjs03.AerialOBJ.WpfApp.Models.Settings;
using binstarjs03.AerialOBJ.WpfApp.Repositories;
using binstarjs03.AerialOBJ.WpfApp.Services.ChunkLoadingPatterns;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace binstarjs03.AerialOBJ.WpfApp.ViewModels;

[ObservableObject]
public partial class SettingViewModel
{
    private readonly ConstantPath _path;

    public SettingViewModel(Setting setting, ConstantPath path, IRepository<IChunkShader> shaderRepository, IRepository<IChunkLoadingPattern> chunkLoadingPatternRepository)
    {
        Setting = setting;
        _path = path;
        ShaderRepository = shaderRepository;
        ChunkLoadingPatternRepository = chunkLoadingPatternRepository;
    }

    public Setting Setting { get; }
    public IRepository<IChunkShader> ShaderRepository { get; }
    public IRepository<IChunkLoadingPattern> ChunkLoadingPatternRepository { get; }
    public Rangeof<int> ViewportChunkThreadRange { get; }
        = new Rangeof<int>(1, Environment.ProcessorCount);

    [RelayCommand]
    private void OnClosing()
    {
        SettingIO.SaveSetting(_path.SettingPath, Setting);
    }
}

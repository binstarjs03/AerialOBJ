using System;

using binstarjs03.AerialOBJ.Core.Primitives;
using binstarjs03.AerialOBJ.WpfApp.Models.Settings;
using binstarjs03.AerialOBJ.WpfApp.Services.ChunkRendering;

using CommunityToolkit.Mvvm.ComponentModel;

namespace binstarjs03.AerialOBJ.WpfApp.ViewModels;

[ObservableObject]
public partial class SettingViewModel
{
    public SettingViewModel(Setting setting, IChunkShaderRepository shaderRepository)
    {
        Setting = setting;
        ShaderRepository = shaderRepository;
    }

    public Setting Setting { get; }
    public IChunkShaderRepository ShaderRepository { get; }
    public Rangeof<int> ViewportChunkThreadRange { get; }
        = new Rangeof<int>(1, Environment.ProcessorCount);
}

using CommunityToolkit.Mvvm.ComponentModel;

namespace binstarjs03.AerialOBJ.WpfApp;

[ObservableObject]
public partial class Setting
{
    public required DefinitionSetting DefinitionSetting { get; init;}
    public required ViewportSetting ViewportSetting { get; init;}
    public required PerformanceSetting PerformanceSetting { get; init; }
}
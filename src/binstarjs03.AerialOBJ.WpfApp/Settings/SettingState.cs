using CommunityToolkit.Mvvm.ComponentModel;

namespace binstarjs03.AerialOBJ.WpfApp;

[ObservableObject]
public partial class SettingState
{
    public required DefinitionSetting DefinitionSetting { get; init; }
    public required ViewportSetting ViewportSetting { get; init; }
}
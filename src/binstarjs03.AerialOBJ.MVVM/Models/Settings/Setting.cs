using binstarjs03.AerialOBJ.MVVM.Models.Settings.Sections;

namespace binstarjs03.AerialOBJ.MVVM.Models.Settings;
public partial class Setting
{
    public required DefinitionSetting DefinitionSetting { get; init; }
    public required ViewportSetting ViewportSetting { get; init; }
    public required PerformanceSetting PerformanceSetting { get; init; }
}
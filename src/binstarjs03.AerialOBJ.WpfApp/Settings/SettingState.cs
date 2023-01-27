using CommunityToolkit.Mvvm.ComponentModel;

namespace binstarjs03.AerialOBJ.WpfApp;

[ObservableObject]
public partial class SettingState
{
    public SettingState(DefinitionSetting definitionSetting, ViewportSetting viewportSetting, PerformanceSetting performanceSetting)
    {
        DefinitionSetting = definitionSetting;
        ViewportSetting = viewportSetting;
        PerformanceSetting = performanceSetting;
    }

    public DefinitionSetting DefinitionSetting { get; set; }
    public ViewportSetting ViewportSetting { get; set; }
    public PerformanceSetting PerformanceSetting { get; set; }

    public static SettingState GetDefaultSetting() => new(DefinitionSetting.GetDefaultSetting(),
                                                          ViewportSetting.GetDefaultSetting(),
                                                          PerformanceSetting.GetDefaultSetting());
}
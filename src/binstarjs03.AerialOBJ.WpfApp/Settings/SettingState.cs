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

    public DefinitionSetting DefinitionSetting { get; }
    public ViewportSetting ViewportSetting { get; }
    public PerformanceSetting PerformanceSetting { get; }

    public static SettingState GetDefaultSetting() => new(DefinitionSetting.GetDefaultSetting(),
                                                          ViewportSetting.GetDefaultSetting(),
                                                          PerformanceSetting.GetDefaultSetting());
}
using CommunityToolkit.Mvvm.ComponentModel;

namespace binstarjs03.AerialOBJ.WpfApp;

[ObservableObject]
public partial class SettingState
{
    [ObservableProperty] private HeightSliderSetting _heightSlider;
}

public enum HeightSliderSetting
{
    Responsive,
    Blocking
}
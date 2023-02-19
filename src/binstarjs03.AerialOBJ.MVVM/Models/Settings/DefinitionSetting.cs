using System;

using binstarjs03.AerialOBJ.Core.Definitions;

using CommunityToolkit.Mvvm.ComponentModel;

namespace binstarjs03.AerialOBJ.MVVM.Models.Settings;
public partial class DefinitionSetting : ObservableObject
{
    [ObservableProperty] private ViewportDefinition _currentViewportDefinition;
    [ObservableProperty] private ModelDefinition _currentModelDefinition;

    public DefinitionSetting(ViewportDefinition currentViewportDefinition, ModelDefinition currentModelDefinition)
    {
        _currentViewportDefinition = currentViewportDefinition;
        _currentModelDefinition = currentModelDefinition;
    }

    public event Action? ViewportDefinitionChanged;

    public static DefinitionSetting GetDefaultSetting() => new(ViewportDefinition.DefaultDefinition, ModelDefinition.DefaultDefinition);

    partial void OnCurrentViewportDefinitionChanged(ViewportDefinition value) => ViewportDefinitionChanged?.Invoke();
}

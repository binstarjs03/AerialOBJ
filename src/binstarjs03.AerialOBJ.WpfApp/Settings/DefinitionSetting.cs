﻿using System;

using binstarjs03.AerialOBJ.Core.Definitions;

using CommunityToolkit.Mvvm.ComponentModel;

namespace binstarjs03.AerialOBJ.WpfApp;

[ObservableObject]
public partial class DefinitionSetting
{
    [ObservableProperty] private ViewportDefinition _currentViewportDefinition;

    public DefinitionSetting(ViewportDefinition currentViewportDefinition)
    {
        _currentViewportDefinition = currentViewportDefinition;
    }

    public static ViewportDefinition DefaultViewportDefinition { get; } = ViewportDefinition.GetDefaultDefinition();

    public event Action? ViewportDefinitionChanging;
    public event Action? ViewportDefinitionChanged;

    public static DefinitionSetting GetDefaultSetting() => new(DefaultViewportDefinition);
    partial void OnCurrentViewportDefinitionChanged(ViewportDefinition value) => ViewportDefinitionChanged?.Invoke();
    partial void OnCurrentViewportDefinitionChanging(ViewportDefinition value) => ViewportDefinitionChanging?.Invoke();
}

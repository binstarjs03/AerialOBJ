using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using binstarjs03.AerialOBJ.Core.Definitions;

namespace binstarjs03.AerialOBJ.WpfApp.Services;
public class DefinitionManagerService
{
    private readonly ViewportDefinition _defaultViewportDefinition = ViewportDefinition.GetDefaultDefinition();
    private ViewportDefinition _currentDefinition;

    public DefinitionManagerService()
    {
        ViewportDefinition emptyDefinition = new ViewportDefinition()
        {
            Name = "Empty Definition",
            FormatVersion = 1,
            MinecraftVersion = "1.18",
            MissingBlockDefinition = new ViewportBlockDefinition()
            {
                Color = new Core.Primitives.Color()
                {
                    Alpha = 255,
                    Red = 0,
                    Green = 0,
                    Blue = 0,
                },
                DisplayName = "Unknown Block",
                LightLevel = 0
            },
            BlockDefinitions = new Dictionary<string, ViewportBlockDefinition>()
        };

        ViewportDefinitions = new ObservableCollection<ViewportDefinition>
        {
            DefaultViewportDefinition,
            emptyDefinition
        };
        _currentDefinition = DefaultViewportDefinition;
    }

    public ViewportDefinition DefaultViewportDefinition => _defaultViewportDefinition;
    public ViewportDefinition CurrentViewportDefinition
    {
        get => _currentDefinition;
        set
        {
            if (value == _currentDefinition)
                return;
            OnViewportDefinitionChanging?.Invoke();
            _currentDefinition = value;
        }

    }
    public ObservableCollection<ViewportDefinition> ViewportDefinitions { get; private set; }

    public event Action? OnViewportDefinitionChanging;
}

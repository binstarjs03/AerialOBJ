using System;
using System.Collections.ObjectModel;

using binstarjs03.AerialOBJ.Core.Definitions;

using CommunityToolkit.Mvvm.ComponentModel;

namespace binstarjs03.AerialOBJ.WpfApp.Services;

[ObservableObject]
public partial class DefinitionManager : IDefinitionManager
{
    [ObservableProperty] private ViewportDefinition _currentViewportDefinition;

    public DefinitionManager()
    {
        LoadedViewportDefinitions = new ObservableCollection<ViewportDefinition> { DefaultViewportDefinition, };
        _currentViewportDefinition = DefaultViewportDefinition;
    }

    public ViewportDefinition DefaultViewportDefinition { get; } = ViewportDefinition.GetDefaultDefinition();
    public ObservableCollection<ViewportDefinition> LoadedViewportDefinitions { get; private set; }

    public event Action? ViewportDefinitionChanging;
    public event Action? ViewportDefinitionChanged;

    public void LoadDefinition(IRootDefinition definition)
    {
        if (definition is ViewportDefinition viewportDefinition)
            LoadedViewportDefinitions.Add(viewportDefinition);
        else
            throw new NotImplementedException();
    }

    public void UnloadDefinition(IRootDefinition definition)
    {
        if (definition.IsDefault)
            throw new InvalidOperationException("Attempting to unload default definition");
        if (definition is ViewportDefinition viewportDefinition)
            LoadedViewportDefinitions.Remove(viewportDefinition);
        else
            throw new NotImplementedException();
    }

    partial void OnCurrentViewportDefinitionChanged(ViewportDefinition value) => ViewportDefinitionChanged?.Invoke();
    partial void OnCurrentViewportDefinitionChanging(ViewportDefinition value) => ViewportDefinitionChanging?.Invoke();
}

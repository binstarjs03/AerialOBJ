using System;
using System.Collections.ObjectModel;

using binstarjs03.AerialOBJ.Core.Definitions;

namespace binstarjs03.AerialOBJ.WpfApp.Services;
public interface IDefinitionManager
{
    ViewportDefinition CurrentViewportDefinition { get; set; }
    ViewportDefinition DefaultViewportDefinition { get; }
    ObservableCollection<ViewportDefinition> LoadedViewportDefinitions { get; }

    event Action? ViewportDefinitionChanging;
    event Action? ViewportDefinitionChanged;

    void LoadDefinition(IRootDefinition definition);
    void UnloadDefinition(IRootDefinition definition);
}
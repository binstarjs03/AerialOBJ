using System;
using System.Collections.ObjectModel;

using binstarjs03.AerialOBJ.Core.Definitions;

namespace binstarjs03.AerialOBJ.WpfApp.Services;
public interface IDefinitionManagerService
{
    ViewportDefinition CurrentViewportDefinition { get; set; }
    ViewportDefinition DefaultViewportDefinition { get; }
    ObservableCollection<ViewportDefinition> ViewportDefinitions { get; }

    event Action? OnViewportDefinitionChanging;

    void LoadDefinitionFolder();
    void LoadViewportDefinition(ViewportDefinition definition);
}
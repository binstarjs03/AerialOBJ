using System;
using System.Collections.ObjectModel;

using binstarjs03.AerialOBJ.Core.Definitions;

namespace binstarjs03.AerialOBJ.WpfApp.Services;
public interface IDefinitionManagerService
{
    ViewportDefinition CurrentViewportDefinition { get; set; }
    ViewportDefinition DefaultViewportDefinition { get; }
    ObservableCollection<ViewportDefinition> LoadedViewportDefinitions { get; }

    event Action? OnViewportDefinitionChanging;
    event Action? OnViewportDefinitionChanged;

    void ImportDefinitionFile(string path);
    void LoadDefinitionFolder();
    void LoadViewportDefinition(ViewportDefinition definition);
}
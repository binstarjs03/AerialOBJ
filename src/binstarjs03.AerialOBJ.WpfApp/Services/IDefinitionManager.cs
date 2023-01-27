using System.Collections.ObjectModel;

using binstarjs03.AerialOBJ.Core.Definitions;

namespace binstarjs03.AerialOBJ.WpfApp.Services;
public interface IDefinitionManager
{
    ObservableCollection<ViewportDefinition> LoadedViewportDefinitions { get; }
    void LoadDefinition(IRootDefinition definition);
    void UnloadDefinition(IRootDefinition definition, DefinitionSetting definitionSetting);
}
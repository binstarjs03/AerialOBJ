using System.Collections.ObjectModel;

using binstarjs03.AerialOBJ.Core.Definitions;
using binstarjs03.AerialOBJ.WpfApp.Models.Settings;

namespace binstarjs03.AerialOBJ.WpfApp.Repositories;
public interface IDefinitionRepository
{
    ObservableCollection<ViewportDefinition> LoadedViewportDefinitions { get; }
    void LoadDefinition(IRootDefinition definition);
    void UnloadDefinition(IRootDefinition definition, DefinitionSetting definitionSetting);
}
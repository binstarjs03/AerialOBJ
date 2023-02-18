using System.Collections.ObjectModel;

using binstarjs03.AerialOBJ.Core.Definitions;
using binstarjs03.AerialOBJ.MVVM.Models.Settings.Sections;

namespace binstarjs03.AerialOBJ.MVVM.Repositories;
public interface IDefinitionRepository
{
    ObservableCollection<ViewportDefinition> LoadedViewportDefinitions { get; }
    void LoadDefinition(IRootDefinition definition);
    void UnloadDefinition(IRootDefinition definition, DefinitionSetting definitionSetting);
}
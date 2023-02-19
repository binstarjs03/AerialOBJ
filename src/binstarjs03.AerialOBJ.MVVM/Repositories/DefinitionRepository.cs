using System.Collections.ObjectModel;

using binstarjs03.AerialOBJ.Core.Definitions;

namespace binstarjs03.AerialOBJ.MVVM.Repositories;
public class DefinitionRepository : IDefinitionRepository
{
    public DefinitionRepository()
    {
        ViewportDefinitions = new() { ViewportDefinition.DefaultDefinition };
        ModelDefinitions = new() { ModelDefinition.DefaultDefinition };
    }

    public ObservableCollection<ViewportDefinition> ViewportDefinitions { get; }
    public ObservableCollection<ModelDefinition> ModelDefinitions { get; }
}

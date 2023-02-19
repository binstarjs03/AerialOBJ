using System.Collections.ObjectModel;

using binstarjs03.AerialOBJ.Core.Definitions;

namespace binstarjs03.AerialOBJ.MVVM.Repositories;
public interface IDefinitionRepository
{
    ObservableCollection<ViewportDefinition> ViewportDefinitions { get; }
    ObservableCollection<ModelDefinition> ModelDefinitions { get; }
}
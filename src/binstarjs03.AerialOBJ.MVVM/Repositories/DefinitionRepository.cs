using System;
using System.Collections.ObjectModel;

using binstarjs03.AerialOBJ.Core.Definitions;
using binstarjs03.AerialOBJ.MVVM.Models.Settings;

namespace binstarjs03.AerialOBJ.MVVM.Repositories;

public partial class DefinitionRepository : IDefinitionRepository
{
    public DefinitionRepository()
    {
        LoadedViewportDefinitions = new ObservableCollection<ViewportDefinition> { DefinitionSetting.DefaultViewportDefinition, };
    }

    public ObservableCollection<ViewportDefinition> LoadedViewportDefinitions { get; private set; }

    public void LoadDefinition(IRootDefinition definition)
    {
        if (definition is ViewportDefinition viewportDefinition)
            LoadedViewportDefinitions.Add(viewportDefinition);
        else
            throw new NotImplementedException();
    }

    public void UnloadDefinition(IRootDefinition definition, DefinitionSetting setting)
    {
        if (definition.IsDefault)
            throw new InvalidOperationException("Attempting to unload default definition");

        if (definition is ViewportDefinition viewportDefinition)
        {
            if (setting.CurrentViewportDefinition == viewportDefinition)
                setting.CurrentViewportDefinition = DefinitionSetting.DefaultViewportDefinition;
            LoadedViewportDefinitions.Remove(viewportDefinition);
        }
        else
            throw new NotImplementedException();
    }
}

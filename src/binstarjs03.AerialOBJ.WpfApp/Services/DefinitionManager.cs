using System;
using System.Collections.ObjectModel;

using binstarjs03.AerialOBJ.Core.Definitions;

using CommunityToolkit.Mvvm.ComponentModel;

namespace binstarjs03.AerialOBJ.WpfApp.Services;

[ObservableObject]
public partial class DefinitionManager : IDefinitionManager
{
    public DefinitionManager()
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

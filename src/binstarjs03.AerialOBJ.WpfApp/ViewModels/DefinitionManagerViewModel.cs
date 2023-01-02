using System.Collections.Generic;

using binstarjs03.AerialOBJ.Core.Definitions;
using binstarjs03.AerialOBJ.WpfApp.Services;

namespace binstarjs03.AerialOBJ.WpfApp.ViewModels;
public class DefinitionManagerViewModel
{
    private readonly DefinitionManagerService _definitionManager;

    public DefinitionManagerViewModel(DefinitionManagerService definitionManager)
	{
        _definitionManager = definitionManager;
    }

    public List<ViewportDefinition> ViewportDefinitions => _definitionManager.ViewportDefinitions;
}

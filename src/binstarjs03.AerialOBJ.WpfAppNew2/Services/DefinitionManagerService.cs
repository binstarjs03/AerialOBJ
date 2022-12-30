using System.Collections.Generic;

using binstarjs03.AerialOBJ.Core.Definitions;

namespace binstarjs03.AerialOBJ.WpfAppNew2.Services;
public class DefinitionManagerService
{
    private ViewportDefinition _currentViewportDefinition;

    public ViewportDefinition CurrentViewportDefinition => _currentViewportDefinition;
    public List<ViewportDefinition> ViewportDefinitions { get; set; } = new();

    public DefinitionManagerService()
    {
        _currentViewportDefinition = ViewportDefinition.GetDefaultDefinition();
    }
}

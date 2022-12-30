using binstarjs03.AerialOBJ.Core.Definitions;

namespace binstarjs03.AerialOBJ.WpfAppNew2.Services;
public class DefinitionManagerService
{
    private readonly ViewportDefinition _defaultViewportDefinition = ViewportDefinition.GetDefaultDefinition();
    public ViewportDefinition DefaultViewportDefinition => _defaultViewportDefinition;
}

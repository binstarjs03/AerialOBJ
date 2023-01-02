using binstarjs03.AerialOBJ.Core.Definitions;

namespace binstarjs03.AerialOBJ.WpfApp.Services;
public class DefinitionManagerService
{
    private readonly ViewportDefinition _defaultViewportDefinition = ViewportDefinition.GetDefaultDefinition();
    public ViewportDefinition DefaultViewportDefinition => _defaultViewportDefinition;
    public ViewportDefinition CurrentViewportDefinition { get; private set; }
    public DefinitionManagerService()
    {
        CurrentViewportDefinition = DefaultViewportDefinition;
    }
}

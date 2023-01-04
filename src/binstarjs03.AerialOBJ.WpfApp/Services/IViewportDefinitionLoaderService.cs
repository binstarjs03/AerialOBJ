using binstarjs03.AerialOBJ.Core.Definitions;

namespace binstarjs03.AerialOBJ.WpfApp.Services;
public interface IViewportDefinitionLoaderService
{
    ViewportDefinition ImportDefinitionFile(string path);
}
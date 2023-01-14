using System.Collections.Generic;

using binstarjs03.AerialOBJ.Core.Definitions;

namespace binstarjs03.AerialOBJ.WpfApp.Services.IOService;
public interface IViewportDefinitionLoader
{
    ViewportDefinition ImportDefinitionFile(string path);
    List<ViewportDefinition> LoadDefinitionFolder(LoadDefinitionFileExceptionHandler exceptionHandler);
}
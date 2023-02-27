using System.Collections.Generic;

using binstarjs03.AerialOBJ.Core.Definitions;

namespace binstarjs03.AerialOBJ.MvvmAppCore.Services.IOService;

public interface IDefinitionIO
{
    IRootDefinition ImportDefinition(string path);
    void DeleteDefinition(IRootDefinition definition);
    List<IRootDefinition> LoadDefinitionFolder(LoadDefinitionFileExceptionHandler exceptionHandler);
}

using System;
using System.IO;
using System.Text.Json;

using binstarjs03.AerialOBJ.Core.Definitions;

namespace binstarjs03.AerialOBJ.WpfApp.Services;
public class ViewportDefinitionLoaderService : IViewportDefinitionLoaderService
{
    private readonly GlobalState _globalState;

    public ViewportDefinitionLoaderService(GlobalState globalState)
    {
        _globalState = globalState;
    }

    public ViewportDefinition ImportDefinitionFile(string path)
    {
        string input = File.ReadAllText(path);
        ViewportDefinition definition = ParseJson(input);
        CopyToDefinitionFolder(path);
        return definition;
    }

    private static ViewportDefinition ParseJson(string input)
    {
        ViewportDefinition? definition = JsonSerializer.Deserialize<ViewportDefinition>(input);
        if (definition is null)
            throw new NullReferenceException("Resulting deserialized definition is null");
        return definition;
    }

    private void CopyToDefinitionFolder(string originalFilePath)
    {
        FileInfo fileInfo = new(originalFilePath);
        Directory.CreateDirectory(_globalState.DefinitionsPath);
        fileInfo.CopyTo(_globalState.DefinitionsPath);
    }
}

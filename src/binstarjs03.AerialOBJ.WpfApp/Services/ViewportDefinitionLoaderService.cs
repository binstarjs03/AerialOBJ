using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

using binstarjs03.AerialOBJ.Core.Definitions;

namespace binstarjs03.AerialOBJ.WpfApp.Services;
public delegate void LoadDefinitionFileExceptionHandler(Exception e, string definitionFileName);
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
        string targetCopyPath = Path.Combine(_globalState.DefinitionsPath, fileInfo.Name);
        fileInfo.CopyTo(targetCopyPath);
    }

    public List<ViewportDefinition> LoadDefinitionFolder(LoadDefinitionFileExceptionHandler exceptionHandler)
    {
        List<ViewportDefinition> definitions = new();
        Directory.CreateDirectory(_globalState.DefinitionsPath);
        DirectoryInfo definitionDirectory = new (_globalState.DefinitionsPath);
        foreach (FileInfo definitionFile in definitionDirectory.GetFiles("*.json"))
        {
            try
            {
                string jsonInput = File.ReadAllText(definitionFile.FullName);
                ViewportDefinition definition = ParseJson(jsonInput);
                definitions.Add(definition);
            }
            catch (Exception e) { exceptionHandler(e, definitionFile.Name); }
        }
        return definitions;
    }
}

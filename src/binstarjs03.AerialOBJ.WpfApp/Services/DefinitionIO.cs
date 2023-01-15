using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

using binstarjs03.AerialOBJ.Core.Definitions;
using binstarjs03.AerialOBJ.WpfApp.Services.IOService;

namespace binstarjs03.AerialOBJ.WpfApp.Services;
public interface IDefinitionIO
{
    IRootDefinition ImportDefinition(string path);
    void DeleteDefinition(IRootDefinition definition);
    List<IRootDefinition> LoadDefinitionFolder(LoadDefinitionFileExceptionHandler exceptionHandler);
}

public class DefinitionIO : IDefinitionIO
{
    private readonly GlobalState _globalState;

    public DefinitionIO(GlobalState globalState)
    {
        _globalState = globalState;
    }

    public IRootDefinition ImportDefinition(string path)
    {
        string input = File.ReadAllText(path);
        IRootDefinition definition = DefinitionDeserializer.Deserialize<IRootDefinition>(input);
        definition.OriginalFilename = Path.GetFileName(path);
        CopyToDefinitionFolder(path);
        return definition;
    }

    private void CopyToDefinitionFolder(string originalPath)
    {
        Directory.CreateDirectory(_globalState.DefinitionsPath);
        string originalFilename = Path.GetFileName(originalPath);
        string copyPath = Path.Combine(_globalState.DefinitionsPath, originalFilename);
        if (File.Exists(copyPath))
            throw new OverwriteException();
        File.Copy(originalPath, copyPath);
    }

    public void DeleteDefinition(IRootDefinition definition)
    {
        string? originalFilename = definition.OriginalFilename;
        if (originalFilename is null)
            throw new InvalidOperationException();
        string deletePath = Path.Combine(_globalState.DefinitionsPath, originalFilename);
        File.Delete(deletePath);
    }

    public List<IRootDefinition> LoadDefinitionFolder(LoadDefinitionFileExceptionHandler exceptionHandler)
    {
        List<IRootDefinition> definitions = new();
        DirectoryInfo definitionDirectory = Directory.CreateDirectory(_globalState.DefinitionsPath);
        foreach (FileInfo definitionFile in definitionDirectory.GetFiles("*.json"))
            try
            {
                string input = File.ReadAllText(definitionFile.FullName);
                IRootDefinition definition = DefinitionDeserializer.Deserialize<IRootDefinition>(input);
                definition.OriginalFilename = definitionFile.Name;
                definitions.Add(definition);
            }
            catch (Exception e) { exceptionHandler(e, definitionFile.Name); }
        return definitions;
    }
}
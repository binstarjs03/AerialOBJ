using System;
using System.Collections.Generic;
using System.IO;

using binstarjs03.AerialOBJ.Core.Definitions;

namespace binstarjs03.AerialOBJ.WpfApp.Services.IOService;

public delegate void LoadDefinitionFileExceptionHandler(Exception e, string definitionFileName);

public class DefinitionIO : IDefinitionIO
{
    private readonly string _definitionPath;

    public DefinitionIO(string definitionPath)
    {
        _definitionPath = definitionPath;
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
        Directory.CreateDirectory(_definitionPath);
        string originalFilename = Path.GetFileName(originalPath);
        string copyPath = Path.Combine(_definitionPath, originalFilename);
        if (File.Exists(copyPath))
            throw new OverwriteException();
        File.Copy(originalPath, copyPath);
    }

    public void DeleteDefinition(IRootDefinition definition)
    {
        string? originalFilename = definition.OriginalFilename;
        if (originalFilename is null)
            throw new InvalidOperationException();
        string deletePath = Path.Combine(_definitionPath, originalFilename);
        File.Delete(deletePath);
    }

    public List<IRootDefinition> LoadDefinitionFolder(LoadDefinitionFileExceptionHandler exceptionHandler)
    {
        List<IRootDefinition> definitions = new();
        DirectoryInfo definitionDirectory = Directory.CreateDirectory(_definitionPath);
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
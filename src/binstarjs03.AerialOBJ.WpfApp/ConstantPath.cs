namespace binstarjs03.AerialOBJ.WpfApp;

public class ConstantPath
{
    public ConstantPath(string currentPath, string definitionsPath, string settingPath)
    {
        CurrentPath = currentPath;
        DefinitionsPath = definitionsPath;
        SettingPath = settingPath;
    }

    public string CurrentPath { get; }
    public string DefinitionsPath { get; }
    public string SettingPath { get; }
}

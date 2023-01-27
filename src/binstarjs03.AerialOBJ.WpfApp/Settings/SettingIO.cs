using System;
using System.IO;

using binstarjs03.AerialOBJ.Core.IniFormat;

namespace binstarjs03.AerialOBJ.WpfApp.Settings;
public static class SettingIO
{
    public static SettingState LoadSetting()
    {
        throw new NotImplementedException();
    }

    public static void SaveSetting(string path)
    {

    }

    public static void SaveDefaultSetting(string settingPath)
    {
        IniDocument doc = new();
        doc.RootSection.Properties.Add("SyntaxVersion", "1");
        doc.RootSection.Properties.Add("FormatVersion", "1");

        IniSection definitionSettingSection = new();
        definitionSettingSection.Properties.Add("ViewportDefinition", "-");

        IniSection viewportSettingSection = new();
        viewportSettingSection.Properties.Add(nameof(ViewportSetting.ChunkShadingStyle), ViewportSetting.DefaultChunkShadingStyle.ToString());
        viewportSettingSection.Properties.Add(nameof(ViewportSetting.ChunkThreads), "-");

        doc.Subsections.Add("DefinitionSetting", definitionSettingSection);
        doc.Subsections.Add("ViewportSetting", viewportSettingSection);

        string output = IniSerializing.Serialize(doc);
        File.WriteAllText(settingPath, output);
    }
}

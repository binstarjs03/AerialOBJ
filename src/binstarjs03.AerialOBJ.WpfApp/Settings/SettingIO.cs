using System;
using System.IO;
using System.Text.Json;

using binstarjs03.AerialOBJ.WpfApp.Services;

namespace binstarjs03.AerialOBJ.WpfApp.Settings;
public static class SettingIO
{
    public static SettingState LoadSetting(string settingPath, IDefinitionManager definitionManager)
    {
        string settingContent = File.ReadAllText(settingPath);
        SettingState? setting;
        SettingJsonConverter settingConverter = new() { DefinitionManager = definitionManager };
        JsonSerializerOptions options = new() { Converters = { settingConverter } };
        setting = JsonSerializer.Deserialize<SettingState>(settingContent, options);
        if (setting is null)
            throw new NullReferenceException();
        return setting;
    }

    public static void SaveSetting(string settingPath, SettingState setting)
    {
        using FileStream fs = File.Open(settingPath, FileMode.Create, FileAccess.Write, FileShare.Read);

        JsonSerializerOptions options = new()
        {
            Converters = { new SettingJsonConverter() },
            WriteIndented = true,

        };
        JsonSerializer.Serialize<SettingState>(fs, setting, options);
    }

    public static void SaveDefaultSetting(string settingPath)
    {
        SettingState setting = SettingState.GetDefaultSetting();
        SaveSetting(settingPath, setting);
    }
}

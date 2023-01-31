using System;
using System.IO;
using System.Linq;
using System.Text.Json;

using binstarjs03.AerialOBJ.Core.Definitions;
using binstarjs03.AerialOBJ.Core.JsonFormat;
using binstarjs03.AerialOBJ.Imaging.ChunkRendering;
using binstarjs03.AerialOBJ.WpfApp.Services;
using binstarjs03.AerialOBJ.WpfApp.Services.ChunkRendering;

namespace binstarjs03.AerialOBJ.WpfApp.Settings;
public static class SettingIO
{
    public static void LoadSetting(Setting setting, string settingPath, IDefinitionManager definitionManager, IShaderRepository shaderRepository)
    {
        string settingJson = File.ReadAllText(settingPath);
        JsonElement root = JsonDocument.Parse(settingJson).RootElement;

        readDefinitionSetting();
        readViewportSetting();
        readPerformanceSetting();

        void readDefinitionSetting()
        {
            if (!root.TryGetProperty(nameof(DefinitionSetting), out JsonElement definitionSettingSection))
                return;
            if (!JsonHelper.TryGetString(definitionSettingSection, nameof(ViewportDefinition), out string vdName))
                return;
            foreach (ViewportDefinition vd in definitionManager.LoadedViewportDefinitions
                                                                .Where(vd => vd.Name == vdName))
            {
                setting.DefinitionSetting.CurrentViewportDefinition = vd;
                break;
            }
        }

        void readViewportSetting()
        {
            if (!root.TryGetProperty(nameof(ViewportSetting), out JsonElement viewportSettingSection))
                return;
            if (!JsonHelper.TryGetString(viewportSettingSection, nameof(ViewportSetting.ChunkShader), out string chunkShader))
                return;
            if (shaderRepository.Shaders.TryGetValue(chunkShader, out IChunkShader? shader))
                setting.ViewportSetting.ChunkShader = shader;
        }

        void readPerformanceSetting()
        {
            if (!root.TryGetProperty(nameof(PerformanceSetting), out JsonElement performanceSettingSection))
                return;
            if (JsonHelper.TryGetInt(performanceSettingSection, nameof(PerformanceSetting.ViewportChunkThreads), out int viewportChunkThreads))
                setting.PerformanceSetting.ViewportChunkThreads = viewportChunkThreads;
            if (JsonHelper.TryGetEnumFromString(performanceSettingSection, nameof(PerformanceSetting.ViewportChunkLoading), out PerformancePreference viewportChunkLoading))
                setting.PerformanceSetting.ViewportChunkLoading = viewportChunkLoading;
            if (JsonHelper.TryGetEnumFromString(performanceSettingSection, nameof(PerformanceSetting.ImageExporting), out PerformancePreference imageExporting))
                setting.PerformanceSetting.ImageExporting = imageExporting;
            if (JsonHelper.TryGetEnumFromString(performanceSettingSection, nameof(PerformanceSetting.ModelExporting), out PerformancePreference modelExporting))
                setting.PerformanceSetting.ModelExporting = modelExporting;
        }
    }

    public static void SaveSetting(string settingPath, Setting setting)
    {
        throw new NotImplementedException();
    }

    public static void SaveDefaultSetting(string settingPath)
    {
        throw new NotImplementedException();
        //Setting setting = Setting.GetDefaultSetting();
        //SaveSetting(settingPath, setting);
    }
}

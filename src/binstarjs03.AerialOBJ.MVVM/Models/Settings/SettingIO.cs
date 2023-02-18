using System;
using System.IO;
using System.Linq;
using System.Text.Json;

using binstarjs03.AerialOBJ.Core.Definitions;
using binstarjs03.AerialOBJ.Core.JsonFormat;
using binstarjs03.AerialOBJ.Imaging.ChunkRendering;
using binstarjs03.AerialOBJ.MVVM.Services.ChunkLoadingPatterns;
using binstarjs03.AerialOBJ.MVVM.Repositories;

namespace binstarjs03.AerialOBJ.MVVM.Models.Settings;
public static class SettingIO
{
    public static void LoadSetting(
        Setting setting,
        string settingPath,
        IDefinitionRepository definitionManager,
        IRepository<IChunkShader> shaderRepo,
        IRepository<IChunkLoadingPattern> chunkLoadingPatternRepo)
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
            if (JsonHelper.TryGetString(viewportSettingSection, nameof(ViewportSetting.ChunkShader), out string chunkShader))
                if (shaderRepo.TryGet(chunkShader, out IChunkShader? shader))
                    setting.ViewportSetting.ChunkShader = shader;
            if (JsonHelper.TryGetString(viewportSettingSection, nameof(ViewportSetting.ChunkLoadingPattern), out string chunkLoadingPattern))
                if (chunkLoadingPatternRepo.TryGet(chunkLoadingPattern, out IChunkLoadingPattern? pattern))
                    setting.ViewportSetting.ChunkLoadingPattern = pattern;
        }

        void readPerformanceSetting()
        {
            if (!root.TryGetProperty(nameof(PerformanceSetting), out JsonElement performanceSettingSection))
                return;
            if (JsonHelper.TryGetInt(performanceSettingSection, nameof(PerformanceSetting.ViewportChunkThreads), out int viewportChunkThreads))
                setting.PerformanceSetting.ViewportChunkThreads = Math.Clamp(viewportChunkThreads, 1, Environment.ProcessorCount);
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
        JsonWriterOptions options = new() { Indented = true, };
        using Stream stream = File.Open(settingPath, FileMode.Create, FileAccess.Write, FileShare.None);
        using Utf8JsonWriter writer = new(stream, options);

        writer.WriteStartObject();
        writer.WriteNumber("FormatDefinition", 1);
        writeDefinitionSetting();
        writeViewportSetting();
        writePerformanceSetting();
        writer.WriteEndObject();

        writer.Flush();
        stream.Flush();

        void writeDefinitionSetting()
        {
            DefinitionSetting definitionSetting = setting.DefinitionSetting;
            writer.WriteStartObject(nameof(DefinitionSetting));
            if (definitionSetting.CurrentViewportDefinition.IsDefault)
                writer.WriteNull(nameof(ViewportDefinition));
            else
                writer.WriteString(nameof(ViewportDefinition), definitionSetting.CurrentViewportDefinition.Name);
            writer.WriteEndObject();
        }

        void writeViewportSetting()
        {
            ViewportSetting viewportSetting = setting.ViewportSetting;
            writer.WriteStartObject(nameof(ViewportSetting));
            writer.WriteString(nameof(ViewportSetting.ChunkShader), viewportSetting.ChunkShader.ShaderName);
            writer.WriteString(nameof(ViewportSetting.ChunkLoadingPattern), viewportSetting.ChunkLoadingPattern.PatternName);
            writer.WriteEndObject();
        }

        void writePerformanceSetting()
        {
            PerformanceSetting performanceSetting = setting.PerformanceSetting;
            writer.WriteStartObject(nameof(PerformanceSetting));
            writer.WriteNumber(nameof(PerformanceSetting.ViewportChunkThreads), performanceSetting.ViewportChunkThreads);
            writer.WriteString(nameof(PerformanceSetting.ViewportChunkLoading), performanceSetting.ViewportChunkLoading.ToString());
            writer.WriteString(nameof(PerformanceSetting.ImageExporting), performanceSetting.ImageExporting.ToString());
            writer.WriteString(nameof(PerformanceSetting.ModelExporting), performanceSetting.ModelExporting.ToString());
            writer.WriteEndObject();
        }
    }
}


public class SettingIOException : Exception
{
    public SettingIOException() { }
    public SettingIOException(string message) : base(message) { }
    public SettingIOException(string message, Exception inner) : base(message, inner) { }
}


public class SettingLoadingException : Exception
{
    public SettingLoadingException() { }
    public SettingLoadingException(string message) : base(message) { }
    public SettingLoadingException(string message, Exception inner) : base(message, inner) { }
}
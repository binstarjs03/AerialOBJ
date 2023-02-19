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
    public static void LoadSetting(Setting setting,
                                   string settingPath,
                                   IDefinitionRepository definitionManager,
                                   IRepository<IChunkShader> shaderRepo,
                                   IRepository<IChunkLoadingPattern> chunkLoadingPatternRepo)
    {
        string settingJson = File.ReadAllText(settingPath);
        JsonElement root = JsonDocument.Parse(settingJson).RootElement;

        if (root.TryGetProperty(nameof(DefinitionSetting), out JsonElement definitionSettingSection))
            readDefinitionSetting(definitionSettingSection, setting.DefinitionSetting, definitionManager);
        if (root.TryGetProperty(nameof(ViewportSetting), out JsonElement viewportSettingSection))
            readViewportSetting(viewportSettingSection, setting.ViewportSetting, shaderRepo, chunkLoadingPatternRepo);
        if (root.TryGetProperty(nameof(PerformanceSetting), out JsonElement performanceSettingSection))
            readPerformanceSetting(performanceSettingSection, setting.PerformanceSetting);

        static void readDefinitionSetting(JsonElement definitionSettingSection, DefinitionSetting definitionSetting, IDefinitionRepository definitionRepo)
        {
            // set current definition setting to what is being set on the json definition name,
            // if we don't do this, current viewport definition setting will be set to default
            if (JsonHelper.TryGetString(definitionSettingSection, nameof(ViewportDefinition), out string vdName))
            {
                var vd = definitionRepo.ViewportDefinitions.Where(vd => vd.Name == vdName).FirstOrDefault();
                if (vd is not null)
                    definitionSetting.CurrentViewportDefinition = vd;
            }

            if (JsonHelper.TryGetString(definitionSettingSection, nameof(ModelDefinition), out string mdName))
            {
                var md = definitionRepo.ModelDefinitions.Where(md => md.Name == mdName).FirstOrDefault();
                if (md is not null)
                    definitionSetting.CurrentModelDefinition = md;
            }
        }

        static void readViewportSetting(JsonElement viewportSettingSection, ViewportSetting viewportSetting, IRepository<IChunkShader> shaderRepo, IRepository<IChunkLoadingPattern> chunkLoadingPatternRepo)
        {
            if (JsonHelper.TryGetString(viewportSettingSection, nameof(ViewportSetting.ChunkShader), out string chunkShader))
                if (shaderRepo.TryGet(chunkShader, out IChunkShader? shader))
                    viewportSetting.ChunkShader = shader;
            if (JsonHelper.TryGetString(viewportSettingSection, nameof(ViewportSetting.ChunkLoadingPattern), out string chunkLoadingPattern))
                if (chunkLoadingPatternRepo.TryGet(chunkLoadingPattern, out IChunkLoadingPattern? pattern))
                    viewportSetting.ChunkLoadingPattern = pattern;
        }

        static void readPerformanceSetting(JsonElement performanceSettingSection, PerformanceSetting performanceSetting)
        {
            if (JsonHelper.TryGetInt(performanceSettingSection, nameof(PerformanceSetting.ViewportChunkThreads), out int viewportChunkThreads))
                performanceSetting.ViewportChunkThreads = Math.Clamp(viewportChunkThreads, 1, Environment.ProcessorCount);
            if (JsonHelper.TryGetEnumFromString(performanceSettingSection, nameof(PerformanceSetting.ViewportChunkLoading), out PerformancePreference viewportChunkLoading))
                performanceSetting.ViewportChunkLoading = viewportChunkLoading;
            if (JsonHelper.TryGetEnumFromString(performanceSettingSection, nameof(PerformanceSetting.ImageExporting), out PerformancePreference imageExporting))
                performanceSetting.ImageExporting = imageExporting;
            if (JsonHelper.TryGetEnumFromString(performanceSettingSection, nameof(PerformanceSetting.ModelExporting), out PerformancePreference modelExporting))
                performanceSetting.ModelExporting = modelExporting;
        }
    }

    public static void SaveSetting(string settingPath, Setting setting)
    {
        var options = new JsonWriterOptions { Indented = true, };
        using var stream = File.Open(settingPath, FileMode.Create, FileAccess.Write, FileShare.None);
        using var writer = new Utf8JsonWriter(stream, options);

        writer.WriteStartObject();
        writer.WriteNumber("FormatDefinition", 1);
        writeDefinitionSetting(writer, setting.DefinitionSetting);
        writeViewportSetting(writer, setting.ViewportSetting);
        writePerformanceSetting(writer, setting.PerformanceSetting);
        writer.WriteEndObject();

        writer.Flush();
        stream.Flush();

        static void writeDefinitionSetting(Utf8JsonWriter writer, DefinitionSetting definitionSetting)
        {
            writer.WriteStartObject(nameof(DefinitionSetting));
            writeDefinitionNameOrNull(writer, definitionSetting.CurrentViewportDefinition);
            writeDefinitionNameOrNull(writer, definitionSetting.CurrentModelDefinition);
            writer.WriteEndObject();

            static void writeDefinitionNameOrNull(Utf8JsonWriter writer, IRootDefinition definition)
            {
                var typeName = definition.GetType().Name;
                if (definition.IsDefault)
                    writer.WriteNull(typeName);
                else
                    writer.WriteString(typeName, definition.Name);
            }
        }

        static void writeViewportSetting(Utf8JsonWriter writer, ViewportSetting viewportSetting)
        {
            writer.WriteStartObject(nameof(ViewportSetting));
            writer.WriteString(nameof(ViewportSetting.ChunkShader), viewportSetting.ChunkShader.ShaderName);
            writer.WriteString(nameof(ViewportSetting.ChunkLoadingPattern), viewportSetting.ChunkLoadingPattern.PatternName);
            writer.WriteEndObject();
        }

        static void writePerformanceSetting(Utf8JsonWriter writer, PerformanceSetting performanceSetting)
        {
            writer.WriteStartObject(nameof(PerformanceSetting));
            writer.WriteNumber(nameof(PerformanceSetting.ViewportChunkThreads), performanceSetting.ViewportChunkThreads);
            writer.WriteString(nameof(PerformanceSetting.ViewportChunkLoading), performanceSetting.ViewportChunkLoading.ToString());
            writer.WriteString(nameof(PerformanceSetting.ImageExporting), performanceSetting.ImageExporting.ToString());
            writer.WriteString(nameof(PerformanceSetting.ModelExporting), performanceSetting.ModelExporting.ToString());
            writer.WriteEndObject();
        }
    }
}
using System;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

using binstarjs03.AerialOBJ.Core.Definitions;
using binstarjs03.AerialOBJ.WpfApp.Services;

namespace binstarjs03.AerialOBJ.WpfApp.Settings;
public class SettingJsonConverter : JsonConverter<SettingState>
{
    public IDefinitionManager? DefinitionManager { get; set; }

    public override SettingState? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        JsonElement root = JsonDocument.ParseValue(ref reader).RootElement;
        DefinitionSetting definitionSetting = readDefinitionSetting();
        ViewportSetting viewportSetting = readViewportSetting();
        PerformanceSetting performanceSetting = readPerformanceSetting();
        return new SettingState(definitionSetting, viewportSetting, performanceSetting);

        DefinitionSetting readDefinitionSetting()
        {
            JsonElement definitionSettingElement = root.GetProperty(nameof(DefinitionSetting));

            string? vdName = definitionSettingElement.GetProperty(nameof(ViewportDefinition)).GetString();
            if (vdName is not null && DefinitionManager is not null)
            {
                foreach (ViewportDefinition vd in DefinitionManager.LoadedViewportDefinitions
                                                                    .Where(vd => vd.Name == vdName))
                    return new DefinitionSetting(vd);
            }
            return new DefinitionSetting(ViewportDefinition.GetDefaultDefinition());
        }

        ViewportSetting readViewportSetting()
        {
            JsonElement viewportSettingElement = root.GetProperty(nameof(ViewportSetting));

            // [C]hunk [S]hading [S]tyle
            string? cssString = viewportSettingElement.GetProperty(nameof(ChunkShadingStyle)).GetString();
            ChunkShadingStyle css = GetEnumOrDefault(cssString, ViewportSetting.DefaultChunkShadingStyle);
            return new ViewportSetting(css);
        }

        PerformanceSetting readPerformanceSetting()
        {
            JsonElement performanceSettingElement = root.GetProperty(nameof(PerformanceSetting));

            int viewportChunkThreads = GetIntOrDefault(performanceSettingElement.GetProperty(nameof(PerformanceSetting.ViewportChunkThreads)),
                                                       PerformanceSetting.DefaultViewportChunkThreads);

            string? viewportChunkLoadingString = performanceSettingElement.GetProperty(nameof(PerformanceSetting.ViewportChunkLoading)).GetString();
            string? imageExportingString = performanceSettingElement.GetProperty(nameof(PerformanceSetting.ImageExporting)).GetString();
            string? modelExportingString = performanceSettingElement.GetProperty(nameof(PerformanceSetting.ModelExporting)).GetString();

            PerformancePreference viewportChunkLoading = GetEnumOrDefault(viewportChunkLoadingString, PerformanceSetting.DefaultViewportChunkLoading);
            PerformancePreference imageExporting = GetEnumOrDefault(imageExportingString, PerformanceSetting.DefaultImageExporting);
            PerformancePreference modelExporting = GetEnumOrDefault(modelExportingString, PerformanceSetting.DefaultModelExporting);

            return new PerformanceSetting(viewportChunkThreads, viewportChunkLoading, imageExporting, modelExporting);
        }

        static int GetIntOrDefault(JsonElement element, int defaultValue)
        {
            if (!element.TryGetInt32(out int result))
                result = defaultValue;
            return result;
        }

        static TEnum GetEnumOrDefault<TEnum>(string? enumString, TEnum defaultValue) where TEnum : struct, Enum
        {
            if (enumString is null || !Enum.TryParse<TEnum>(enumString, out TEnum result))
                result = defaultValue;
            return result;
        }
    }

    public override void Write(Utf8JsonWriter writer, SettingState value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();
        writer.WriteNumber("FormatDefinition", 1);
        writeDefinitionSetting();
        writeViewportSetting();
        writePerformanceSetting();
        writer.WriteEndObject();

        void writeDefinitionSetting()
        {
            DefinitionSetting definitionSetting = value.DefinitionSetting;
            writer.WriteStartObject(nameof(DefinitionSetting));
            if (definitionSetting.CurrentViewportDefinition.IsDefault)
                writer.WriteNull(nameof(ViewportDefinition));
            else
                writer.WriteString(nameof(ViewportDefinition), definitionSetting.CurrentViewportDefinition.Name);
            writer.WriteEndObject();
        }

        void writeViewportSetting()
        {
            ViewportSetting viewportSetting = value.ViewportSetting;
            writer.WriteStartObject(nameof(ViewportSetting));
            writer.WriteString(nameof(ViewportSetting.ChunkShadingStyle), viewportSetting.ChunkShadingStyle.ToString());
            writer.WriteEndObject();
        }

        void writePerformanceSetting()
        {
            writer.WriteStartObject(nameof(PerformanceSetting));
            writer.WriteNumber(nameof(PerformanceSetting.ViewportChunkThreads), Environment.ProcessorCount);
            writer.WriteString(nameof(PerformanceSetting.ViewportChunkLoading), nameof(PerformancePreference.OptimalMemoryUsage));
            writer.WriteString(nameof(PerformanceSetting.ImageExporting), nameof(PerformancePreference.OptimalMemoryUsage));
            writer.WriteString(nameof(PerformanceSetting.ModelExporting), nameof(PerformancePreference.OptimalMemoryUsage));
            writer.WriteEndObject();
        }
    }
}

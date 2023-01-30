using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace binstarjs03.AerialOBJ.Core.JsonFormat.Converters;
public class EnumConverter<TEnum> : JsonConverter<TEnum> where TEnum : struct, Enum
{
    public override TEnum Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        string? valueStr = reader.GetString();
        if (Enum.TryParse(valueStr, out TEnum value))
            return value;
        return default;
    }

    public override void Write(Utf8JsonWriter writer, TEnum value, JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }
}
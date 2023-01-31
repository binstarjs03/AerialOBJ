using System;
using System.Text.Json;

namespace binstarjs03.AerialOBJ.Core.JsonFormat;
public static class JsonHelper
{
    public static bool TryGetInt(JsonElement jsonObject, string jsonPropertyName, out int value)
    {
        value = 0;
        if (jsonObject.TryGetProperty(jsonPropertyName, out JsonElement targetJsonProperty)
            && targetJsonProperty.TryGetInt32(out value))
            return true;
        return false;
    }

    public static bool TryGetString(JsonElement jsonObject, string jsonPropertyName, out string value, bool throwIfNotString = false)
    {
        value = string.Empty;
        string? outValue;
        if (!jsonObject.TryGetProperty(jsonPropertyName, out JsonElement targetJsonProperty))
            return false;

        try
        {
            outValue = targetJsonProperty.GetString();
        }
        catch (InvalidOperationException)
        {
            if (throwIfNotString)
                throw;
            return false;
        }

        if (outValue is null)
            return false;
        value = outValue;
        return true;
    }

    public static bool TryGetEnumFromString<TEnum>(JsonElement jsonObject, string jsonPropertyName, out TEnum value) where TEnum : struct, Enum
    {
        value = default;
        if (!TryGetString(jsonObject, jsonPropertyName, out string valueStr))
            return false;
        if (!Enum.TryParse(valueStr, out value))
            return false;
        return true;
    }
}

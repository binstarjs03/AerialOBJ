using System.Globalization;

namespace binstarjs03.AerialOBJ.Core;
public static class StringUtils
{
    public static string ToTitleCase(this string str) =>
        CultureInfo.InvariantCulture.TextInfo.ToTitleCase(str);
}

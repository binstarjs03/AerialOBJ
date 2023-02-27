using System;

namespace binstarjs03.AerialOBJ.Core;

public static class MathUtils
{
    private static readonly string[] s_dataUnits = { "B", "KB", "MB", "GB", "TB" };

    public static int Mod(int left, int right)
    {
        int result = left % right;
        if (right >= 0 && result < 0 || right < 0 && result >= 0)
            result += right;
        return result;
    }

    public static int DivFloor(double num, double divisor)
    {
        return (int)Math.Floor(num / divisor);
    }

    public static int DivFloor(int num, int divisor)
    {
        return (int)MathF.Floor((float)num / divisor);
    }

    public static int Floor(this double num)
    {
        return (int)Math.Floor(num);
    }

    public static int Floor(this float num)
    {
        return (int)MathF.Floor(num);
    }

    public static int Ceiling(this float num)
    {
        return (int)MathF.Ceiling(num);
    }

    public static float Round(this float num)
    {
        return MathF.Round(num, 2);
    }

    public static string DataUnitToString(long bytes)
    {
        int unit = 0;
        double size = bytes;
        while (size >= 1000 && unit < s_dataUnits.Length - 1)
        {
            size /= 1000;
            unit++;
        }

        // return decimal if one digit, else return whole number
        return size <= 9 ? $"{Math.Round(size, 3)} {s_dataUnits[unit]}"
                         : $"{size.Floor()} {s_dataUnits[unit]}";
    }
}

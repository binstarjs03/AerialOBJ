using System;

namespace binstarjs03.AerialOBJ.Core;

public static class MathUtils
{
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

    public static float Round(this float num)
    {
        return (float)Math.Round(num, 2);
    }
}

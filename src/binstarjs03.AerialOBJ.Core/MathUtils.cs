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
}

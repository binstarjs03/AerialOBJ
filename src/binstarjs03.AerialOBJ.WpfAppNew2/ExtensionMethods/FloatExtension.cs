using System;

namespace binstarjs03.AerialOBJ.WpfAppNew2.ExtensionMethods;
public static class FloatExtension
{
    public static float Floor(this float f)
    {
        return MathF.Floor(f);
    }
}

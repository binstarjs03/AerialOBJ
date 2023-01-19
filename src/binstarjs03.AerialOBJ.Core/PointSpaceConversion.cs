using binstarjs03.AerialOBJ.Core.Primitives;

namespace binstarjs03.AerialOBJ.Core;

public static class PointSpaceConversion
{
    public static PointY<float> ConvertWorldPosToScreenPos(PointZ<float> worldPos, PointZ<float> cameraPos, float unitMultiplier, Size<float> screenSize)
    {
        float screenX = ConvertWorldPosToScreenPos(worldPos.X, cameraPos.X, unitMultiplier, screenSize.Width);
        float screenY = ConvertWorldPosToScreenPos(worldPos.Z, cameraPos.Z, unitMultiplier, screenSize.Height);
        return new PointY<float>(screenX, screenY);
    }

    public static PointZ<float> ConvertScreenPosToWorldPos(PointY<float> screenPos, PointZ<float> cameraPos, float unitMultiplier, Size<float> screenSize)
    {
        float worldX = ConvertScreenPosToWorldPos(screenPos.X, cameraPos.X, unitMultiplier, screenSize.Width);
        float worldZ = ConvertScreenPosToWorldPos(screenPos.Y, cameraPos.Z, unitMultiplier, screenSize.Height);
        return new PointZ<float>(worldX, worldZ);
    }

    public static float ConvertWorldPosToScreenPos(float worldPos, float cameraPos, float unitMultiplier, float screenSize)
    {
        return -(cameraPos * unitMultiplier) + (screenSize / 2) + (worldPos * unitMultiplier);
    }

    public static float ConvertScreenPosToWorldPos(float screenPos, float cameraPos, float unitMultiplier, float screenSize)
    {
        return -(-(cameraPos * unitMultiplier) + (screenSize / 2) - screenPos) / unitMultiplier;
    }
}

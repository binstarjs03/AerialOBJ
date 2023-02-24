using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using binstarjs03.AerialOBJ.Core;
using binstarjs03.AerialOBJ.Core.Primitives;

using CommunityToolkit.Mvvm.ComponentModel;

namespace binstarjs03.AerialOBJ.MvvmAppCore.Services;
public class ViewportCoordsManager : ObservableObject
{
    private PointZRange<int> _visibleRange = default;
    private readonly HashSet<PointZ<int>> _points = new();
    public ObservableCollection<PointZ<int>> Coords { get; } = new();

    public void Update(int screenDistance, PointZ<float> cameraPos, Size<int> screenSize, float unitMultiplier)
    {
        float worldDivision = MathF.Pow(2, MathF.Log2(unitMultiplier).Floor());

        // step amount to display the coord in world space
        float worldStepF = screenDistance / worldDivision;
        int worldStep = worldStepF.Floor();

        var indexRange = CalculateIndexRange(cameraPos, screenSize, screenDistance, worldStepF);
        var visibleRange = CalculateVisibleRange(indexRange, worldStep);

        if (!IsVisibleRangeChanged(visibleRange))
            return;

        _points.RemoveWhere(_visibleRange.IsOutside);
        for (int i = Coords.Count - 1; i >= 0; i--)
            if (_visibleRange.IsOutside(Coords[i]))
                Coords.RemoveAt(i);

        var xRange = indexRange.XRange;
        var zRange = indexRange.ZRange;
        for (int x = xRange.Min; x <= xRange.Max; x++)
            for (int z = zRange.Min; z <= zRange.Max; z++)
            {
                PointZ<int> point = new(x * worldStep, z * worldStep);
                if (_points.Contains(point))
                    continue;
                else
                {
                    _points.Add(point);
                    Coords.Add(point);
                }
            }
    }

    public void Reinitialize()
    {
        _visibleRange = default;
        _points.Clear();
        Coords.Clear();
    }

    private bool IsVisibleRangeChanged(PointZRange<int> visibleRange)
    {
        if (_visibleRange == visibleRange)
            return false;
        _visibleRange = visibleRange;
        return true;
    }

    private static PointZRange<int> CalculateVisibleRange(PointZRange<int> indexRange, int step)
    {
        int minXRange = indexRange.XRange.Min * step;
        int maxXRange = indexRange.XRange.Max * step;
        int minZRange = indexRange.ZRange.Min * step;
        int maxZRange = indexRange.ZRange.Max * step;
        return new PointZRange<int>(minXRange, maxXRange, minZRange, maxZRange);
    }

    private static PointZRange<int> CalculateIndexRange(PointZ<float> cameraPos, Size<int> screenSize, int screenDistance, float step)
    {
        Rangeof<int> xRange = CalculateIndexRangeSingle(cameraPos.X, screenSize.Width, screenDistance, step);
        Rangeof<int> zRange = CalculateIndexRangeSingle(cameraPos.Z, screenSize.Height, screenDistance, step);
        return new PointZRange<int>(xRange, zRange);
    }

    private static Rangeof<int> CalculateIndexRangeSingle(float cameraPos, int screenSize, int screenDistance, float step)
    {
        float cameraIndex = cameraPos / step;
        float visibleIndex = screenSize / 2f / screenDistance;

        int minIndexRange = MathUtils.Floor(cameraIndex - visibleIndex);
        int maxIndexRange = MathUtils.Floor(cameraIndex + visibleIndex);

        return new Rangeof<int>(minIndexRange, maxIndexRange);
    }
}

﻿using System;

using CommunityToolkit.Mvvm.ComponentModel;

namespace binstarjs03.AerialOBJ.MvvmAppCore.Models.Settings;
public partial class PerformanceSetting : ObservableObject
{
    [ObservableProperty] private int _viewportChunkThreads;
    [ObservableProperty] private PerformancePreference _viewportChunkLoading;
    [ObservableProperty] private PerformancePreference _imageExporting;
    [ObservableProperty] private PerformancePreference _modelExporting;
    public PerformanceSetting(int viewportChunkThreads,
                              PerformancePreference viewportChunkLoading,
                              PerformancePreference imageExporting,
                              PerformancePreference modelExporting)
    {
        _viewportChunkThreads = viewportChunkThreads;
        _viewportChunkLoading = viewportChunkLoading;
        _imageExporting = imageExporting;
        _modelExporting = modelExporting;
    }

    public static int DefaultViewportChunkThreads => Math.Max(Environment.ProcessorCount - 1, 1);
    public static PerformancePreference DefaultViewportChunkLoading => PerformancePreference.OptimalMemoryUsage;
    public static PerformancePreference DefaultImageExporting => PerformancePreference.OptimalMemoryUsage;
    public static PerformancePreference DefaultModelExporting => PerformancePreference.OptimalMemoryUsage;

    public event Action? ViewportChunkThreadsChanged;

    public static PerformanceSetting GetDefaultSetting() => new(DefaultViewportChunkThreads,
                                                                DefaultViewportChunkLoading,
                                                                DefaultImageExporting,
                                                                DefaultModelExporting);

    partial void OnViewportChunkThreadsChanged(int value) => ViewportChunkThreadsChanged?.Invoke();
}

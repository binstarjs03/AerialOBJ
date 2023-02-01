using System;

namespace binstarjs03.AerialOBJ.WpfApp.Services.Diagnostics;
public interface IMemoryInfo
{
    long MemoryAllocatedSize { get; }
    long MemoryUsedSize { get; }

    event Action? MemoryInfoUpdated;

    void StartMonitorMemory();
    void StopMonitorMemory();
}
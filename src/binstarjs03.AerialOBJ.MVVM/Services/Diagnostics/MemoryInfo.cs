using System;
using System.Threading;
using System.Threading.Tasks;

using binstarjs03.AerialOBJ.MVVM.Primitives;

namespace binstarjs03.AerialOBJ.MVVM.Services.Diagnostics;

public class MemoryInfo : IMemoryInfo
{
    private readonly ReferenceWrap<bool> _isRunning = new() { Value = false };
    private Task _memoryInfoTask = Task.CompletedTask;
    private CancellationTokenSource _cts = new();
    private Action<Action> _dispatcher;

    public MemoryInfo(Action<Action> dispatcher)
    {
        _dispatcher = dispatcher;
    }

    public long MemoryUsedSize { get; private set; }
    public long MemoryAllocatedSize { get; private set; }

    public event Action? MemoryInfoUpdated;

    public void StartMonitorMemory()
    {
        lock (_isRunning)
        {
            if (_isRunning.Value)
                return;
            _isRunning.Value = true;
            _memoryInfoTask = Task.Factory.StartNew(UpdateMemoryInfo,
                                                    _cts.Token,
                                                    TaskCreationOptions.LongRunning,
                                                    TaskScheduler.Default);
        }
    }

    public void StopMonitorMemory()
    {
        _cts.Cancel();
        _memoryInfoTask.Wait();
        _cts.Dispose();
        _cts = new CancellationTokenSource();
    }

    private void UpdateMemoryInfo()
    {
        try
        {
            while (!_cts.IsCancellationRequested)
            {
                GCMemoryInfo memoryInfo = GC.GetGCMemoryInfo();
                MemoryUsedSize = memoryInfo.HeapSizeBytes;
                MemoryAllocatedSize = memoryInfo.TotalCommittedBytes;
                try
                {
                    _dispatcher(() => MemoryInfoUpdated?.Invoke());
                    Task.Delay(1000, _cts.Token).Wait(_cts.Token);
                }
                catch (TaskCanceledException) { return; }
                catch (OperationCanceledException) { return; }
            }
        }
        finally
        {
            lock (_isRunning)
                _isRunning.Value = false;
        }
    }
}

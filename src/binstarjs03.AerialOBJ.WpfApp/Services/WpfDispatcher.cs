using System;
using System.Threading;
using System.Threading.Tasks;

using binstarjs03.AerialOBJ.MvvmAppCore.Services.Dispatcher;

using WpfDispatcherPriority = System.Windows.Threading.DispatcherPriority;

namespace binstarjs03.AerialOBJ.WpfApp.Services.Dispatcher;

public class WpfDispatcher : IDispatcher
{
    private readonly System.Windows.Threading.Dispatcher _wpfDispatcher;

    public WpfDispatcher(System.Windows.Threading.Dispatcher wpfDispatcher)
    {
        _wpfDispatcher = wpfDispatcher;
    }

    public bool CheckAccess() => _wpfDispatcher.CheckAccess();

    public void Invoke(Action callback, DispatcherPriority priority, CancellationToken token)
    {
        try
        {
            _wpfDispatcher.Invoke(callback, TranslatePriority(priority), token);
        }
        catch (TaskCanceledException) { }
    }

    public T? Invoke<T>(Func<T> callback, DispatcherPriority priority, CancellationToken token)
    {
        try
        {
            return _wpfDispatcher.Invoke(callback, TranslatePriority(priority), token);
        }
        catch (TaskCanceledException) { return default; }
    }

    public Task InvokeAsync(Action callback, DispatcherPriority priority, CancellationToken token)
    {
        try
        {
            return _wpfDispatcher.InvokeAsync(callback, TranslatePriority(priority), token).Task;
        }
        catch (TaskCanceledException) { return Task.CompletedTask; }
    }

    public Task<T>? InvokeAsync<T>(Func<T> callback, DispatcherPriority priority, CancellationToken token)
    {
        try
        {
            return _wpfDispatcher.InvokeAsync(callback, TranslatePriority(priority), token).Task;
        }
        catch (TaskCanceledException) { return null; }
    }

    private static WpfDispatcherPriority TranslatePriority(DispatcherPriority priority)
    {
        return priority switch
        {
            DispatcherPriority.Low => WpfDispatcherPriority.ContextIdle,
            DispatcherPriority.Background => WpfDispatcherPriority.Background,
            DispatcherPriority.BackgroundHigh => WpfDispatcherPriority.Render,
            DispatcherPriority.Normal => WpfDispatcherPriority.Normal,
            DispatcherPriority.High => WpfDispatcherPriority.Send,
            _ => throw new NotImplementedException(),
        };
    }
}

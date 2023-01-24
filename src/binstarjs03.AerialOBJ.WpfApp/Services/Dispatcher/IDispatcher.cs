using System;
using System.Threading;
using System.Threading.Tasks;

namespace binstarjs03.AerialOBJ.WpfApp.Services.Dispatcher;
public interface IDispatcher
{
    bool CheckAccess();

    void Invoke(Action callback, DispatcherPriority priority, CancellationToken token);
    T? Invoke<T>(Func<T> callback, DispatcherPriority priority, CancellationToken token);

    Task InvokeAsync(Action callback, DispatcherPriority priority, CancellationToken token);
    Task<T>? InvokeAsync<T>(Func<T> callback, DispatcherPriority priority, CancellationToken token);
}

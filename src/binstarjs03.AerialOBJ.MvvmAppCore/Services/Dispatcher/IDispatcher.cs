using System;
using System.Threading;
using System.Threading.Tasks;

namespace binstarjs03.AerialOBJ.MvvmAppCore.Services.Dispatcher;
public interface IDispatcher
{
    bool CheckAccess();

    void Invoke(Action callback, DispatcherPriority priority, CancellationToken token);
    T? Invoke<T>(Func<T> callback, DispatcherPriority priority, CancellationToken token);
}

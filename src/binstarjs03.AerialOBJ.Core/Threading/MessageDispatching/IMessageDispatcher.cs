using System;
using System.Threading;

namespace binstarjs03.AerialOBJ.Core.Threading.MessageDispatching;
public interface IMessageDispatcher
{
    event Action<Exception> DispatchingException;
    event Action Reinitialized;
    event Action<CancellationToken> Started;
    event Action Stopped;

    string Name { get; init; }
    bool IsRunning { get; }
    ExceptionBehaviour ExceptionBehaviour { get; set; }
    CancellationToken CancellationToken { get; }

    void Start();
    void StopAndWait();
    void Stop();

    void InvokeSynchronous(Action message, MessageDuplication option);
    T InvokeSynchronous<T>(Func<T> message);
    MessageOperation InvokeAsynchronous(Action message);
    MessageOperation<T> InvokeAsynchronous<T>(Func<T> message);
    void InvokeAsynchronousNoDuplicate(Action message);

    bool CheckAccess();
    void VerifyAccess();
}

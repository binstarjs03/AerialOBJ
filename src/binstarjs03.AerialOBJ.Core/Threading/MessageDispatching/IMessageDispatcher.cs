using System;

namespace binstarjs03.AerialOBJ.Core.Threading.MessageDispatching;
public interface IMessageDispatcher
{
    event Action<Exception> DispatchingException;
    event Action Reinitialized;
    event Action Started;
    event Action Stopped;

    string Name { get; }
    bool IsRunning { get; }
    ExceptionBehaviour ExceptionBehaviour { get; set; }

    void Start();
    void Stop();

    void InvokeSynchronous(Action message, MessageDuplication option);
    T InvokeSynchronous<T>(Func<T> message);
    MessageOperation InvokeAsynchronous(Action message);
    MessageOperation<T> InvokeAsynchronous<T>(Func<T> message);

    bool CheckAccess();
    void VerifyAccess();
}

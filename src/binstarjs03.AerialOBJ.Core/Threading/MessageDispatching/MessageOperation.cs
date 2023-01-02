using System;

namespace binstarjs03.AerialOBJ.Core.Threading.MessageDispatching;
public class MessageOperation
{
    protected readonly Message _message;
    protected readonly Action<Exception?> _thrower;

    public MessageStatus Status => _message.Status;
    public Exception? Exception => _message.Exception;

    public MessageOperation(Message message, Action<Exception?> Thrower)
    {
        _message = message;
        _thrower = Thrower;
    }

    public void Wait()
    {
        // race condition can occur, simply move on if autoresetevent
        // is disposed (means already invoked by the thread messenger)
        try
        {
            if (_message.Status == MessageStatus.Pending)
                _message.InvokeCompletedEvent?.WaitOne();
        }
        catch (ObjectDisposedException) { }
        _thrower(_message.Exception);
    }
}

public class MessageOperation<T> : MessageOperation
{
    public T Result
    {
        get
        {
            Wait();
            return (_message as Message<T>)!.Result;
        }
    }

    public MessageOperation(Message<T> message, Action<Exception?> thrower)
        : base(message, thrower) { }
}
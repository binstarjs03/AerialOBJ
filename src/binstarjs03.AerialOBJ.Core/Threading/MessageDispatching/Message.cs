using System.Threading;
using System;

namespace binstarjs03.AerialOBJ.Core.Threading.MessageDispatching;
public class Message : IMessage
{
    protected readonly Action _message;
    private bool _disposedValue;

    public MessageStatus Status { get; private set; } = MessageStatus.Pending;
    public virtual AutoResetEvent InvokeCompletedEvent { get; } = new AutoResetEvent(false);
    public Exception? Exception { get; private set; } = null;

    public Message(Action message)
    {
        _message = message;
    }

    public void Invoke()
    {
        try
        {
            InvokeMessage();
            Status = MessageStatus.Success;
        }
        catch (Exception ex)
        {
            Status = MessageStatus.Failed;
            Exception = ex;
            throw;
        }
    }

    protected virtual void InvokeMessage()
    {
        _message?.Invoke();
    }

    public void Abandon()
    {
        Exception = new AbandonedMessageException();
        Status = MessageStatus.Abandoned;
        InvokeCompletedEvent.Set();
    }

    protected void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing)
                InvokeCompletedEvent.Dispose();
            _disposedValue = true;
        }
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}

public class Message<T> : Message
{
    protected new readonly Func<T> _message;
    private T? _result;

    public T Result => Status == MessageStatus.Success ?
        _result! : throw new InvalidOperationException();

    public Message(Func<T> message) : base(() => { })
    {
        _message = message;
    }

    protected override void InvokeMessage()
    {
        _result = _message();
    }
}

public class AbandonedMessageException : Exception
{
    public AbandonedMessageException() { }
    public AbandonedMessageException(string message) : base(message) { }
    public AbandonedMessageException(string message, Exception inner) : base(message, inner) { }
}
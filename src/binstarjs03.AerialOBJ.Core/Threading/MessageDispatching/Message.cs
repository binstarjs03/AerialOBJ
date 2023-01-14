using System;
using System.Threading;

namespace binstarjs03.AerialOBJ.Core.Threading.MessageDispatching;
public class Message : IMessage, IEquatable<Message>
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

    public override int GetHashCode()
    {
        return _message.GetHashCode();
    }

    public override bool Equals(object? obj)
    {
        if (obj is Message msg)
            return Equals(msg);
        return false;
    }

    public bool Equals(Message? other)
    {
        return other is not null
            && other._message == _message;
    }

    public static bool operator ==(Message? left, Message? right)
    {
        if (left is not null)
            return left.Equals(right);
        return false;
    }

    public static bool operator !=(Message? left, Message? right)
    {
        return !(left == right);
    }
}

public class Message<T> : Message, IEquatable<Message<T>>
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

    public override int GetHashCode()
    {
        return _message.GetHashCode();
    }

    public override bool Equals(object? obj)
    {
        if (obj is Message<T> msg)
            return Equals(msg);
        return false;
    }

    public bool Equals(Message<T>? other)
    {
        return other is not null
            && other._message == _message;
    }

    public static bool operator ==(Message<T>? left, Message<T>? right)
    {
        if (left is not null)
            return left.Equals(right);
        return false;
    }

    public static bool operator !=(Message<T>? left, Message<T>? right)
    {
        return !(left == right);
    }
}

public class AbandonedMessageException : Exception
{
    public AbandonedMessageException() { }
    public AbandonedMessageException(string message) : base(message) { }
    public AbandonedMessageException(string message, Exception inner) : base(message, inner) { }
}
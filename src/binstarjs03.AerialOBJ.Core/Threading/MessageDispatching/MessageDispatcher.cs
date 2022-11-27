using System;
using System.Collections.Generic;
using System.Threading;

namespace binstarjs03.AerialOBJ.Core.Threading.MessageDispatching;
public class MessageDispatcher : IMessageDispatcher
{
    private Thread _dispatcher = new(() => { });
    private CancellationTokenSource _cts = new();

    private readonly Queue<IMessage> _messageQueue = new(50);
    private readonly AutoResetEvent _messageEvent = new(false);

    public required string Name { get; init; }
    public bool IsRunning => _dispatcher.IsAlive;
    public required ExceptionBehaviour ExceptionBehaviour { get; set; }
    public CancellationToken CancellationToken => _cts.Token;

    public event Action<Exception>? DispatchingException;
    public event Action? Reinitialized;
    public event Action<CancellationToken>? Started;
    public event Action? Stopping;
    public event Action? Stopped;

    public MessageDispatcher() { }

    public void Start()
    {
        if (IsRunning)
            throw new InvalidOperationException($"{nameof(MessageDispatcher)} is already running");
        Reinitialize();
        _cts.Dispose();
        _cts = new CancellationTokenSource();
        _dispatcher = new Thread(ProcessMessage)
        {
            Name = Name,
            IsBackground = true
        };
        _dispatcher.Start();
        Started?.Invoke(CancellationToken);
    }

    public void Stop()
    {
        if (!IsRunning)
            return;
        _cts.Cancel();
        _messageEvent.Set();
        Stopping?.Invoke();
        _dispatcher.Join();
        _cts.Dispose();
        Stopped?.Invoke();
    }

    public bool CheckAccess()
    {
        return Thread.CurrentThread == _dispatcher;
    }

    public void VerifyAccess()
    {
        if (!CheckAccess())
            throw new MemberAccessException("The calling thread is not dispatcher thread");
    }

    #region Invoke methods
    // TODO accept CancellationToken and return, dont wait if message has not processed yet and cancelled
    // TODO too much repetition. Use polymorphism and decorator pattern or similar
    public void InvokeSynchronous(Action message, MessageDuplication duplication = MessageDuplication.AllowDuplicate)
    {
        ValidateThreadState();
        Message msg = new(message);
        PostMessage(msg, MessageDuplication.NoDuplicate);
        msg.InvokeCompletedEvent?.WaitOne();
        msg.InvokeCompletedEvent?.Dispose();
        ThrowIfExceptionNotNullOnPoster(msg.Exception);
        return;
    }

    public T InvokeSynchronous<T>(Func<T> message)
    {
        ValidateThreadState();
        Message<T> msg = new(message);
        PostMessage(msg, MessageDuplication.AllowDuplicate);
        msg.InvokeCompletedEvent.WaitOne();
        msg.InvokeCompletedEvent.Dispose();
        ThrowIfExceptionNotNullOnPoster(msg.Exception);
        return msg.Result;
    }

    public MessageOperation InvokeAsynchronous(Action message)
    {
        ValidateThreadState();
        Message msg = new(message);
        PostMessage(msg, MessageDuplication.AllowDuplicate);
        return new MessageOperation(msg, ThrowIfExceptionNotNullOnPoster);
    }

    public MessageOperation<T> InvokeAsynchronous<T>(Func<T> message)
    {
        ValidateThreadState();
        Message<T> msg = new(message);
        PostMessage(msg, MessageDuplication.AllowDuplicate);
        return new MessageOperation<T>(msg, ThrowIfExceptionNotNullOnPoster);
    }

    public void InvokeAsynchronousNoDuplicate(Action message)
    {
        ValidateThreadState();
        Message msg = new(message);
        PostMessage(msg, MessageDuplication.NoDuplicate);
    }

    #endregion Invoke methods

    #region Internal methods (implementation details)
    private void Reinitialize()
    {
        foreach (IMessage message in _messageQueue)
            message.Dispose();
        _messageQueue.Clear();
        _messageEvent.Reset();
        Reinitialized?.Invoke();
    }

    private void PostMessage(IMessage message, MessageDuplication duplication)
    {
        lock (_messageQueue)
            // dont post message if duplication is not allowed and it exist in the queue
            switch (duplication)
            {
                case MessageDuplication.NoDuplicate:
                    if (_messageQueue.Contains(message))
                        return;
                    else
                        goto case MessageDuplication.AllowDuplicate;
                case MessageDuplication.AllowDuplicate:
                    _messageQueue.Enqueue(message);
                    break;
                default:
                    throw new NotImplementedException();
            }
        _messageEvent.Set();
    }

    private void ValidateThreadState()
    {
        if (_cts.IsCancellationRequested || !IsRunning)
            throw new InvalidOperationException(
                $"{nameof(MessageDispatcher)} is not running or has requested to stop");
    }

    private static void ThrowIfExceptionNotNullOnPoster(Exception? ex)
    {
        if (ex is not null)
            throw ex;
    }

    private void ProcessMessage()
    {
        while (!_cts.IsCancellationRequested)
        {
            IMessage? message = GetMessage();
            if (message is null)
                break;
            try
            {
                message.Invoke();
                message.InvokeCompletedEvent?.Set();
            }
            catch (Exception ex)
            {
                bool breakLoop = false;
                message.Abandon();
                DispatchingException?.Invoke(ex);
                switch (ExceptionBehaviour)
                {
                    case ExceptionBehaviour.Ignore:
                        break;
                    case ExceptionBehaviour.Terminate:
                        breakLoop = true;
                        break;
                    default:
                        throw new NotImplementedException();
                }
                if (breakLoop)
                    break;
            }
        }
        OnTerminate();
    }

    private IMessage? GetMessage()
    {
        IMessage? message = null;
        while (true) // infinite-check message queue in case it still null
        {
            lock (_messageQueue)
                _messageQueue.TryDequeue(out message);
            if (message is null)
            {
                _messageEvent.WaitOne();
                if (_cts.IsCancellationRequested)
                    return null;
                continue;
            }
            return message;
        }
    }

    private void OnTerminate()
    {
        // because our dispatcher is about to dead,
        // we have to let all remaining messages know that they are abandoned
        foreach (IMessage remainingMessage in _messageQueue)
            remainingMessage.Abandon();
    }
    #endregion Internal methods (implementation details)
}

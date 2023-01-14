using System;
using System.Threading;

namespace binstarjs03.AerialOBJ.Core.Threading.MessageDispatching;
public interface IMessage : IDisposable
{
    AutoResetEvent InvokeCompletedEvent { get; }
    MessageStatus Status { get; }
    Exception? Exception { get; }
    void Invoke();
    void Abandon();
}
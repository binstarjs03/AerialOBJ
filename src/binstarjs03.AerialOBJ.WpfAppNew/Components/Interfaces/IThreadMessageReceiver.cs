using System;

namespace binstarjs03.AerialOBJ.WpfAppNew.Components.Interfaces;

public interface IThreadMessageReceiver
{
    void Start();
    void Stop();
    void PostMessage(Action message, MessageOption messageOption);
}


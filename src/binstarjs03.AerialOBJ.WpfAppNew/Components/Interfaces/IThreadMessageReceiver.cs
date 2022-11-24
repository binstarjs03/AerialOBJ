using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace binstarjs03.AerialOBJ.WpfAppNew.Components.Interfaces;
public interface IThreadMessageReceiver
{
    void PostMessage(Action message, MessageOption messageOption);
}


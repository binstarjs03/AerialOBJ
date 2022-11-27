using System;

namespace binstarjs03.AerialOBJ.Core;
public interface ILogging
{
    event Action<string> Logging;
}

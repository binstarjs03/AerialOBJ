using System;

namespace binstarjs03.AerialOBJ.Core;
public interface ILogging
{
    Action<string> LogHandler { get; init; }
}

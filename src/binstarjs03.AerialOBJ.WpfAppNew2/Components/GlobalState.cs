using System;

namespace binstarjs03.AerialOBJ.WpfAppNew2.Components;
public class GlobalState
{
    public static string AppName => "AerialOBJ";
    public DateTime LaunchTime { get; }

    public GlobalState(DateTime launchTime)
    {
        LaunchTime = launchTime;
    }
}

using System;
using System.Linq;

namespace binstarjs03.AerialOBJ.WpfApp;
public class AppInfo
{
    public required string AppName { get; set; }
    public required string Version { get; set; }
    public required DateTime LaunchTime { get; set; }
    public required ConstantPath Path { get; set; }
    public required string[]? Arguments { get; set; }
    public bool IsDebugEnabled => Arguments is not null && Arguments.All(arg=> arg.ToLower() == "debug");
}
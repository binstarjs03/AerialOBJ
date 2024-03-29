﻿using System;
using System.Linq;

namespace binstarjs03.AerialOBJ.MvvmAppCore;

public class AppInfo
{
    public required string AppName { get; init; }
    public required string Version { get; init; }
    public required DateTime LaunchTime { get; init; }
    public required string[]? Arguments { get; init; }

    public bool IsDebugEnabled => Arguments is not null
                               && Arguments.Any(arg => arg.ToLower() == "debug");
}
﻿namespace binstarjs03.AerialOBJ.Core.Primitives;

/// <summary>
/// A class that does exactly nothing but wraps value type around reference type
/// </summary>
public class ReferenceWrap<T>
{
    public required T Value { get; set; }
}

namespace binstarjs03.AerialOBJ.WpfAppNew2.Components;

/// <summary>
/// Tiny class that wraps value type around reference type
/// to write pattern of ValueType - ObjectType (lock) pair cleanly
/// </summary>
/// <typeparam name="T"></typeparam>
public class StructLock<T>
{
    public required T Value { get; set; }
}

namespace binstarjs03.AerialOBJ.WpfApp.Components;

/// <summary>
/// Tiny class that wraps value type around reference type
/// </summary>
public class ReferenceWrap<T>
{
    public required T Value { get; set; }
}

using System.Threading;

namespace binstarjs03.AerialOBJ.WpfAppNew2.Components;
public class ReaderWriterLockSlimWrapper<T>
{
    public ReaderWriterLockSlim Lock { get; } = new();
    public required T Value { get; set; }
}

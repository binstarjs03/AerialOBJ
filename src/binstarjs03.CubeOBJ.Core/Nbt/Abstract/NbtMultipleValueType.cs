namespace binstarjs03.AerialOBJ.Core.Nbt;

public abstract class NbtMultipleValueType : NbtBase
{
    public NbtMultipleValueType() : base() { }

    public NbtMultipleValueType(string name) : base(name) { }

    public abstract int ValueCount { get; }
}

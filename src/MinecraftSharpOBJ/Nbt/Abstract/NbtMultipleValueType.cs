namespace binstarjs03.MinecraftSharpOBJ.Nbt.Abstract;

public abstract class NbtMultipleValueType : NbtBase {
    public NbtMultipleValueType() : base() { }

    public NbtMultipleValueType(string name) : base(name) { }

    public abstract int ValueCount { get; }
}

namespace binstarjs03.MineSharpOBJ.Core.Nbt.Abstract;

public abstract class NbtContainerType : NbtMultipleValueType {
    public NbtContainerType() : base() { }

    public NbtContainerType(string name) : base(name) { }

    public override NbtTypeBase NbtTypeBase => NbtTypeBase.NbtContainerType;

    public override string ToString() {
        return $"{base.ToString()} - tags: {Tags.Length} tags";
    }

    public abstract NbtBase[] Tags { get; }
}

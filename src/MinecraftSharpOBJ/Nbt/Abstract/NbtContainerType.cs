namespace binstarjs03.MinecraftSharpOBJ.Nbt.Abstract;

public abstract class NbtContainerType : NbtMultipleValueType {
    public NbtContainerType() : base() {
        return;
    }

    public NbtContainerType(string name) : base(name) {
        return;
    }

    public override NbtTypeBase NbtTypeBase { 
        get { return NbtTypeBase.NbtContainerType; }
    }

    public override string ToString() {
        string ret = $"{base.ToString()} - tags: {Tags.Length} tags";
        return ret;
    }

    public abstract NbtBase[] Tags {
        get;
    }
}

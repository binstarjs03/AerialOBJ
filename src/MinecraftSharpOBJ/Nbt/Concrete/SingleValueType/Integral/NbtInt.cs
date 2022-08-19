using binstarjs03.MinecraftSharpOBJ.Nbt.Abstract;
namespace binstarjs03.MinecraftSharpOBJ.Nbt.Concrete;

public class NbtInt : NbtSingleValueType<int> {
    public NbtInt() : base() {
        return;
    }

    public NbtInt(string name) : base(name) {
        return;
    }

    public NbtInt(int value) : base(value) {
        return;
    }

    public NbtInt(string name, int value) : base(name, value) {
        return;
    }

    public override NbtType NbtType {
        get { return NbtType.NbtInt; }
    }

    public override string NbtTypeName {
        get { return Nbt.NbtTypeName.NbtInt; }
    }

    protected override char ValuePostfix {
        get { return NbtSingleValueTypePostfix.NbtInt; }
    }

    public override NbtInt Clone() {
        return new(_name, _value);
    }
}

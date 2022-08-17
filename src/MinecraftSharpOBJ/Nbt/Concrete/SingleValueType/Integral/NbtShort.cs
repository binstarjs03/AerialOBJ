using binstarjs03.MinecraftSharpOBJ.Nbt.Abstract;
namespace binstarjs03.MinecraftSharpOBJ.Nbt.Concrete;

public class NbtShort : NbtNumericType<short> {
    public NbtShort() : base() {
        return;
    }

    public NbtShort(string name) : base(name) {
        return;
    }

    public NbtShort(short value) : base(value) {
        return;
    }

    public NbtShort(string name, short value) : base(name, value) {
        return;
    }

    public override NbtType NbtType {
        get { return NbtType.NbtShort; }
    }

    public override string NbtTypeName {
        get { return Nbt.NbtTypeName.NbtShort; }
    }

    protected override char ValuePostfix {
        get { return NbtNumericPostfix.NbtShort; }
    }

    public override NbtShort Clone() {
        return new(_name, _value);
    }
}

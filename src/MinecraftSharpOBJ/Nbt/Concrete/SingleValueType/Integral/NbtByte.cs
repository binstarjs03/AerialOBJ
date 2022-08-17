using binstarjs03.MinecraftSharpOBJ.Nbt.Abstract;
namespace binstarjs03.MinecraftSharpOBJ.Nbt.Concrete;

public class NbtByte : NbtNumericType<sbyte> {
    public NbtByte() : base() {
        return;
    }

    public NbtByte(string name) : base(name) {
        return;
    }

    public NbtByte(sbyte value) : base(value) {
        return;
    }

    public NbtByte(string name, sbyte value) : base(name, value) {
        return;
    }

    public override NbtType NbtType {
        get { return NbtType.NbtByte; }
    }

    public override string NbtTypeName {
        get { return Nbt.NbtTypeName.NbtByte; }
    }

    protected override char ValuePostfix {
        get { return NbtNumericPostfix.NbtByte; }
    }

    public override NbtByte Clone() {
        return new(_name, _value);
    }
}

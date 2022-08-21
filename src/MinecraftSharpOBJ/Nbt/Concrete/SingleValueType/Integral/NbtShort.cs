using binstarjs03.MinecraftSharpOBJ.Nbt.Abstract;
namespace binstarjs03.MinecraftSharpOBJ.Nbt.Concrete;

public class NbtShort : NbtSingleValueType<short> {
    public NbtShort() : base() { return; }

    public NbtShort(string name) : base(name) { return; }

    public NbtShort(short value) : base(value) { return; }

    public NbtShort(string name, short value) : base(name, value) { return; }

    public override NbtType NbtType => NbtType.NbtShort;

    public override string NbtTypeName => Nbt.NbtTypeName.NbtShort;

    protected override char ValuePostfix => NbtSingleValueTypePostfix.NbtShort;

    public override NbtShort Clone() {
        return new(_name, _value);
    }

    protected override void Deserialize(IO.NbtBinaryReader reader) {
        _value = reader.ReadShort();
    }
}

using binstarjs03.MinecraftSharpOBJ.Nbt.Abstract;
namespace binstarjs03.MinecraftSharpOBJ.Nbt.Concrete;

public class NbtInt : NbtSingleValueType<int> {
    public NbtInt() : base() { return; }

    public NbtInt(string name) : base(name) { return; }

    public NbtInt(int value) : base(value) { return; }

    public NbtInt(string name, int value) : base(name, value) { return; }

    public override NbtType NbtType => NbtType.NbtInt;

    public override string NbtTypeName => Nbt.NbtTypeName.NbtInt;

    protected override char ValuePostfix => NbtSingleValueTypePostfix.NbtInt;

    public override NbtInt Clone() {
        return new(_name, _value);
    }

    protected override void Deserialize(IO.NbtBinaryReader reader) {
        _value = reader.ReadInt();
    }
}

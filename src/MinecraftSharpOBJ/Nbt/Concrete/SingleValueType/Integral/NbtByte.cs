using binstarjs03.MinecraftSharpOBJ.Nbt.Abstract;
namespace binstarjs03.MinecraftSharpOBJ.Nbt.Concrete;

public class NbtByte : NbtSingleValueType<sbyte> {
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
        get { return NbtSingleValueTypePostfix.NbtByte; }
    }

    public override NbtByte Clone() {
        return new(_name, _value);
    }

    protected override void Deserialize(IO.NbtBinaryReader reader) {
        _value = reader.ReadSByte();
    }
}

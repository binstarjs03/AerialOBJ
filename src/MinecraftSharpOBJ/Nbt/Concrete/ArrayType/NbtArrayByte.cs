using binstarjs03.MinecraftSharpOBJ.Nbt.Abstract;
namespace binstarjs03.MinecraftSharpOBJ.Nbt.Concrete;

public class NbtArrayByte : NbtArrayType<sbyte> {
    public NbtArrayByte() : base() { return; }

    public NbtArrayByte(string name) : base(name) { return; }

    public NbtArrayByte(sbyte[] values) : base(values) { return; }

    public NbtArrayByte(string name, sbyte[] values) : base(name, values) { return; }

    public override NbtType NbtType => NbtType.NbtArrayByte;

    public override string NbtTypeName => Nbt.NbtTypeName.NbtArrayByte;

    public override NbtArrayByte Clone() {
        return new(_name, _values.ToArray());
    }

    protected override void Deserialize(IO.NbtBinaryReader reader) {
        int elementLength = reader.ReadInt();
        for (int i = 0; i < elementLength; i++) {
            _values.Add(reader.ReadSByte());
        }
    }
}

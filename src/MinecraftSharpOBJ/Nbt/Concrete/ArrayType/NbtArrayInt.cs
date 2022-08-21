using binstarjs03.MinecraftSharpOBJ.Nbt.Abstract;
namespace binstarjs03.MinecraftSharpOBJ.Nbt.Concrete;

public class NbtArrayInt : NbtArrayType<int> {
    public NbtArrayInt() : base() { return; }

    public NbtArrayInt(string name) : base(name) { return; }

    public NbtArrayInt(int[] values) : base(values) { return; }

    public NbtArrayInt(string name, int[] values) : base(name, values) { return; }

    public override NbtType NbtType => NbtType.NbtArrayInt;

    public override string NbtTypeName => Nbt.NbtTypeName.NbtArrayInt;

    public override NbtArrayInt Clone() {
        return new(_name, _values.ToArray());
    }

    protected override void Deserialize(IO.NbtBinaryReader reader) {
        int elementLength = reader.ReadInt();
        for (int i = 0; i < elementLength; i++) {
            _values.Add(reader.ReadInt());
        }
    }
}

using binstarjs03.MineSharpOBJ.Core.Nbt.Abstract;
namespace binstarjs03.MineSharpOBJ.Core.Nbt.Concrete;

public class NbtArrayLong : NbtArrayType<long> {
    public NbtArrayLong() : base() { return; }

    public NbtArrayLong(string name) : base(name) { return; }

    public NbtArrayLong(long[] values) : base(values) { return; }

    public NbtArrayLong(string name, long[] values) : base(name, values) { return; }

    public override NbtType NbtType => NbtType.NbtArrayLong;

    public override NbtArrayLong Clone() {
        return new(_name, _values.ToArray());
    }

    protected override void Deserialize(IO.NbtBinaryReader reader) {
        int elementLength = reader.ReadInt();
        for (int i = 0; i < elementLength; i++) {
            _values.Add(reader.ReadLong());
        }
    }
}

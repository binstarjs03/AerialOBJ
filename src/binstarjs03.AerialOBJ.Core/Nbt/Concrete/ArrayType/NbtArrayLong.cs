namespace binstarjs03.AerialOBJ.Core.Nbt;

public class NbtArrayLong : NbtArrayType<long>
{
    public NbtArrayLong() : base() { }

    public NbtArrayLong(string name) : base(name) { }

    public NbtArrayLong(long[] values) : base(values) { }

    public NbtArrayLong(string name, long[] values) : base(name, values) { }

    public override NbtType NbtType => NbtType.NbtArrayLong;

    public override NbtArrayLong Clone()
    {
        return new(_name, _values.ToArray());
    }

    protected override void Deserialize(IO.NbtBinaryReader reader)
    {
        int elementLength = reader.ReadIntBE();
        for (int i = 0; i < elementLength; i++)
        {
            Values.Add(reader.ReadLongBE());
        }
    }
}

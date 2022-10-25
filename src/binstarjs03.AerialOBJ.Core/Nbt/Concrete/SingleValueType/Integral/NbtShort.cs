namespace binstarjs03.AerialOBJ.Core.Nbt;

public class NbtShort : NbtSingleValueType<short>
{
    public NbtShort() : base() { return; }

    public NbtShort(string name) : base(name) { return; }

    public NbtShort(short value) : base(value) { return; }

    public NbtShort(string name, short value) : base(name, value) { return; }

    public override NbtType NbtType => NbtType.NbtShort;

    public override NbtShort Clone()
    {
        return new(_name, _value);
    }

    protected override void Deserialize(IO.NbtBinaryReader reader)
    {
        _value = reader.ReadShortBE();
    }
}

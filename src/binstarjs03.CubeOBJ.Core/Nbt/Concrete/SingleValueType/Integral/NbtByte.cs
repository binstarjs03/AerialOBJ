namespace binstarjs03.CubeOBJ.Core.Nbt;

public class NbtByte : NbtSingleValueType<sbyte>
{
    public NbtByte() : base() { return; }

    public NbtByte(string name) : base(name) { return; }

    public NbtByte(sbyte value) : base(value) { return; }

    public NbtByte(string name, sbyte value) : base(name, value) { return; }

    public override NbtType NbtType => NbtType.NbtByte;

    public override NbtByte Clone()
    {
        return new(_name, _value);
    }

    protected override void Deserialize(IO.NbtBinaryReader reader)
    {
        _value = reader.ReadSByte();
    }
}

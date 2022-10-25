namespace binstarjs03.AerialOBJ.Core.Nbt;

public class NbtDouble : NbtSingleValueType<double>
{
    public NbtDouble() : base() { return; }

    public NbtDouble(string name) : base(name) { return; }

    public NbtDouble(double value) : base(value) { return; }

    public NbtDouble(string name, double value) : base(name, value) { return; }

    public override NbtType NbtType => NbtType.NbtFloat;

    public override NbtDouble Clone()
    {
        return new(_name, _value);
    }

    protected override void Deserialize(IO.NbtBinaryReader reader)
    {
        _value = reader.ReadDoubleBE();
    }
}

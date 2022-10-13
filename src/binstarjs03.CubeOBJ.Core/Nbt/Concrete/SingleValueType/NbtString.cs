namespace binstarjs03.AerialOBJ.Core.Nbt;

public class NbtString : NbtSingleValueType<NonNullString>
{
    public NbtString()
    {
        _value = NonNullString.Empty;
    }

    public NbtString(string name) : base(name) { }

    public NbtString(string name, string value) : base(name)
    {
        _value = new NonNullString(value);
    }

    public override NbtType NbtType => NbtType.NbtString;

    public new string Value
    {
        get { return _value.Value; }
        set { _value.Value = value; }
    }

    public override NbtString Clone()
    {
        return new(_name, _value.Value);
    }

    protected override void Deserialize(IO.NbtBinaryReader reader)
    {
        _value.Value = reader.ReadString(sizeof(short));
    }
}

using System;

namespace binstarjs03.AerialOBJ.Core.Nbt;

[Obsolete($"Use {nameof(NbtNew)} library instead")]
public class NbtString : NbtSingleValueType<StructString>
{
    public NbtString()
    {
        _value = StructString.Empty;
    }

    public NbtString(string name) : base(name) { }

    public NbtString(string name, string value) : base(name)
    {
        _value = new StructString(value);
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
        _value.Value = reader.ReadStringLengthPrefixed();
    }
}

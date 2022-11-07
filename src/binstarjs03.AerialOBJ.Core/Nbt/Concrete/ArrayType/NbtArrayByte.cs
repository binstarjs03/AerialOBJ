using System;

namespace binstarjs03.AerialOBJ.Core.Nbt;

[Obsolete($"Use {nameof(NbtNew)} library instead")]
public class NbtArrayByte : NbtArrayType<sbyte>
{
    public NbtArrayByte() : base() { }

    public NbtArrayByte(string name) : base(name) { }

    public NbtArrayByte(sbyte[] values) : base(values) { }

    public NbtArrayByte(string name, sbyte[] values) : base(name, values) { }

    public override NbtType NbtType => NbtType.NbtArrayByte;

    public override NbtArrayByte Clone()
    {
        return new(_name, _values.ToArray());
    }

    protected override void Deserialize(IO.NbtBinaryReader reader)
    {
        int elementLength = reader.ReadIntBE();
        for (int i = 0; i < elementLength; i++)
        {
            Values.Add(reader.ReadSByte());
        }
    }
}

using System;

namespace binstarjs03.AerialOBJ.Core.Nbt;

[Obsolete($"Use {nameof(NbtNew)} library instead")]
public class NbtArrayInt : NbtArrayType<int>
{
    public NbtArrayInt() : base() { }

    public NbtArrayInt(string name) : base(name) { }

    public NbtArrayInt(int[] values) : base(values) { }

    public NbtArrayInt(string name, int[] values) : base(name, values) { }

    public override NbtType NbtType => NbtType.NbtArrayInt;

    public override NbtArrayInt Clone()
    {
        return new(_name, _values.ToArray());
    }

    protected override void Deserialize(IO.NbtBinaryReader reader)
    {
        int elementLength = reader.ReadIntBE();
        for (int i = 0; i < elementLength; i++)
        {
            Values.Add(reader.ReadIntBE());
        }
    }
}

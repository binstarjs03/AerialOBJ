using System;

namespace binstarjs03.AerialOBJ.Core.Nbt;

[Obsolete($"Use {nameof(NbtNew)} library instead")]
public abstract class NbtMultipleValueType : NbtBase
{
    public NbtMultipleValueType() : base() { }

    public NbtMultipleValueType(string name) : base(name) { }

    public abstract int ValueCount { get; }
}

﻿using binstarjs03.MinecraftSharpOBJ.Nbt.Abstract;
namespace binstarjs03.MinecraftSharpOBJ.Nbt.Concrete;

public class NbtArrayLong : NbtArrayType<long> {
    public NbtArrayLong() : base() {
        return;
    }

    public NbtArrayLong(string name) : base(name) {
        return;
    }

    public NbtArrayLong(long[] values) : base(values) {
        return;
    }

    public NbtArrayLong(string name, long[] values) : base(name, values) {
        return;
    }

    public override NbtType NbtType {
        get { return NbtType.NbtArrayLong; }
    }

    public override string NbtTypeName {
        get { return Nbt.NbtTypeName.NbtArrayLong; }
    }

    public override NbtArrayLong Clone() {
        return new(_name, _values.ToArray());
    }
}
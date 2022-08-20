﻿using binstarjs03.MinecraftSharpOBJ.Nbt.Abstract;
namespace binstarjs03.MinecraftSharpOBJ.Nbt.Concrete;

public class NbtArrayByte : NbtArrayType<sbyte> {
    public NbtArrayByte() : base() {
        return;
    }

    public NbtArrayByte(string name) : base(name) {
        return;
    }

    public NbtArrayByte(sbyte[] values) : base(values) {
        return;
    }

    public NbtArrayByte(string name, sbyte[] values) : base(name, values) {
        return;
    }

    public override NbtType NbtType {
        get { return NbtType.NbtArrayByte; }
    }

    public override string NbtTypeName {
        get { return Nbt.NbtTypeName.NbtArrayByte; }
    }

    public override NbtArrayByte Clone() {
        return new(_name, _values.ToArray());
    }
}
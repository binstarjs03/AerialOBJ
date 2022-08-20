﻿using binstarjs03.MinecraftSharpOBJ.Nbt.Abstract;
namespace binstarjs03.MinecraftSharpOBJ.Nbt.Concrete;

public class NbtLong : NbtSingleValueType<long> {
    public NbtLong() : base() {
        return;
    }

    public NbtLong(string name) : base(name) {
        return;
    }

    public NbtLong(long value) : base(value) {
        return;
    }

    public NbtLong(string name, long value) : base(name, value) {
        return;
    }

    public override NbtType NbtType {
        get { return NbtType.NbtLong; }
    }

    public override string NbtTypeName {
        get { return Nbt.NbtTypeName.NbtLong; }
    }

    protected override char ValuePostfix {
        get { return NbtSingleValueTypePostfix.NbtLong; }
    }

    public override NbtLong Clone() {
        return new(_name, _value);
    }

    protected override void Deserialize(IO.NbtBinaryReader reader) {
        _value = reader.ReadLong();
    }
}

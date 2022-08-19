using System;
namespace binstarjs03.MinecraftSharpOBJ.Nbt.Abstract;

public abstract class NbtSingleValueType<T> : NbtBase where T : struct {
    protected T _value;

    public NbtSingleValueType() : base() {
        _value = default;
    }

    public NbtSingleValueType(string name) : base(name) {
        _value = default;
    }

    public NbtSingleValueType(T value) : base() {
        _value = value;
    }

    public NbtSingleValueType(string name, T value) : base(name) {
        _value = value;
    }

    public T Value {
        get { return _value; }
        set { _value = value; }
    }

    protected abstract char ValuePostfix {
        get;
    }

    public override string ToString() {
        string ret = $"{base.ToString()} - value: {_value}";
        return ret;
    }
}

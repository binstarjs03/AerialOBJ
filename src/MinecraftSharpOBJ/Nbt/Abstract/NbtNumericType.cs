namespace binstarjs03.MinecraftSharpOBJ.Nbt.Abstract;

public abstract class NbtNumericType<T> : NbtBase where T : struct {
    protected T _value = default;

    public NbtNumericType() : base() {
        _value = default;
    }

    public NbtNumericType(string name) : base(name) {
        _value = default;
    }

    public NbtNumericType(T value) : base() {
        _value = value;
    }

    public NbtNumericType(string name, T value) : base(name) {
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

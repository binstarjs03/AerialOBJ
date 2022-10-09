namespace binstarjs03.CubeOBJ.Core.Nbt;

public abstract class NbtSingleValueType : NbtBase
{
    public NbtSingleValueType() : base() { }

    public NbtSingleValueType(string name) : base(name) { }

    public abstract string ValueStringized { get; }
}

public abstract class NbtSingleValueType<T> : NbtSingleValueType where T : struct
{
    protected T _value;

    public NbtSingleValueType() : base()
    {
        _value = default;
    }

    public NbtSingleValueType(string name) : base(name)
    {
        _value = default;
    }

    public NbtSingleValueType(T value) : base()
    {
        _value = value;
    }

    public NbtSingleValueType(string name, T value) : base(name)
    {
        _value = value;
    }

    public override NbtTypeBase NbtTypeBase => NbtTypeBase.NbtSingleValueType;

    public override string ToString()
    {
        return $"{base.ToString()} - value: {Value}";
    }

    public T Value
    {
        get { return _value; }
        set { _value = value; }
    }

    public override string ValueStringized
    {
        get
        {
            string? ret = _value.ToString();
            return ret is null ? string.Empty : ret;
        }
    }
}

using System.Collections.Generic;
namespace binstarjs03.MinecraftSharpOBJ.Nbt.Abstract;

public abstract class NbtArrayType<T> : NbtBase where T : struct {
    protected List<T> _values = new();

    public NbtArrayType() : base() {
        _values = new List<T>();
    }

    public NbtArrayType(string name) : base(name) {
        _values = new List<T>();
    }

    public NbtArrayType(T[] values) : base() {
        _values = new List<T>(values);
    }

    public NbtArrayType(string name, T[] values) : base(name) {
        _values = new List<T>(values);
    }

    public List<T> Values {
        get { return _values; }
        set { _values = value; }
    }

    public override string ToString() {
        string ret = $"{base.ToString()} - values: {_values.Count} items";
        return ret;
    }
}

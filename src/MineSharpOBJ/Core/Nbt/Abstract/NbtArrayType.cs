using System.Collections.Generic;
namespace binstarjs03.MineSharpOBJ.Core.Nbt.Abstract;

public abstract class NbtArrayType : NbtMultipleValueType {
    public NbtArrayType(string name) : base(name) { }

    public NbtArrayType() : base() { }

    public abstract string[] ValuesStringized { get; }
}

public abstract class NbtArrayType<T> : NbtArrayType where T : struct {
    protected List<T> _values;

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

    public override NbtTypeBase NbtTypeBase => NbtTypeBase.NbtArrayType;       

    public override string ToString() {
        return $"{base.ToString()} - values: {_values.Count} items";
    }

    public List<T> Values {
        get { return _values; }
        set { _values = value; }
    }

    public override string[] ValuesStringized {
        get { 
            string[] ret = new string[_values.Count];
            for (int i = 0; i < ret.Length; i++) {
                string? v = _values[i].ToString();
                ret[i] = v is null? string.Empty : v;
            }
            return ret;
        }
    }

    public override int ValueCount => _values.Count;
}

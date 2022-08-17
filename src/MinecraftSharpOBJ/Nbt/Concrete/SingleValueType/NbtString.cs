using binstarjs03.MinecraftSharpOBJ.Nbt.Abstract;
namespace binstarjs03.MinecraftSharpOBJ.Nbt.Concrete;

public class NbtString : NbtBase {
    private string _value;

    public NbtString() : base() {
        _value = "";
    }

    public NbtString(string value) : base() {
        _value = value;
    }

    public NbtString(string name, string value) : base(name) {
        _value = value;
    }

    public override NbtType NbtType {
        get { return NbtType.NbtString; }
    }

    public override string NbtTypeName {
        get { return Nbt.NbtTypeName.NbtString; }
    }

    public string Value {
        get { return _value; }
        set { _value = value; }
    }

    public override NbtString Clone() {
        return new(_name, _value);
    }

    public override string ToString() {
        string ret = $"{base.ToString} - value: {_value}";
        return ret;
    }
}

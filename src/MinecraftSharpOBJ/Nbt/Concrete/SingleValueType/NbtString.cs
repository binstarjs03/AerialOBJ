using binstarjs03.MinecraftSharpOBJ.Nbt.Abstract;
namespace binstarjs03.MinecraftSharpOBJ.Nbt.Concrete;

public class NbtString : NbtSingleValueType<NonNullString> {
    public NbtString() {
        _value = NonNullString.Empty;
    }

    public NbtString(string value) {
        _value = new NonNullString(value);
    }

    public NbtString(string name, string value) : base(name) {
        _value = new NonNullString(value);
    }

    public override NbtType NbtType {
        get { return NbtType.NbtString; }
    }

    public override string NbtTypeName {
        get { return Nbt.NbtTypeName.NbtString; }
    }

    public new string Value {
        get { return _value.Value; }
        set { _value.Value = value; }
    }

    protected override char ValuePostfix {
        get { return '\0'; }
    }

    public override NbtString Clone() {
        return new(_name, _value.Value);
    }
}

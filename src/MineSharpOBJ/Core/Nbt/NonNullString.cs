namespace binstarjs03.MineSharpOBJ.Core.Nbt;

public struct NonNullString {
    private string _value;

    public NonNullString() {
        _value = string.Empty;
    }

    public NonNullString(string value) {
        _value = value;
    }

    public static NonNullString Empty => new();

    public string Value {
        get { return _value is null? string.Empty : _value; }
        set { _value = value is null? string.Empty : value; }
    }

    public override string ToString() {
        return Value;
    }
}

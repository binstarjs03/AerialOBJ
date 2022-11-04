namespace binstarjs03.AerialOBJ.Core;

public struct StructString
{
    private string _value;

    public StructString()
    {
        _value = string.Empty;
    }

    public StructString(string value)
    {
        _value = value;
    }

    public static StructString Empty => new();

    public string Value
    {
        get { return _value is null ? string.Empty : _value; }
        set { _value = value is null ? string.Empty : value; }
    }

    public override string ToString()
    {
        return Value;
    }
}

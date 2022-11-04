namespace binstarjs03.AerialOBJ.Core.NbtNew;

public class NbtFloat : INbtValue<float>
{
    public string Name { get; set; }
    public static NbtType Type => NbtType.NbtFloat;
    public float Value { get; set; }

    public NbtFloat(string name, float value)
    {
        Name = name;
        Value = value;
    }

    public override string ToString()
    {
        return $"<{Type}> {Name} - Value: {Value}";
    }
}

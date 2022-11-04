namespace binstarjs03.AerialOBJ.Core.NbtNew;

public class NbtFloat : INbtValue<float>
{
    public string Name { get; set; }
    public float Value { get; set; }

    public NbtFloat(string name, float value)
    {
        Name = name;
        Value = value;
    }
}

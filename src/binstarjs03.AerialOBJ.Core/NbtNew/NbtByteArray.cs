namespace binstarjs03.AerialOBJ.Core.NbtNew;

public class NbtByteArray : INbtArray<sbyte>
{
    public string Name { get; set; }
    public sbyte[] Values { get; set; }

    public NbtByteArray(string name, sbyte[] values)
    {
        Name = name;
        Values = values;
    }
}

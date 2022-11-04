namespace binstarjs03.AerialOBJ.Core.NbtNew;

public class NbtLongArray : INbtArray<long>
{
    public string Name { get; set; }
    public long[] Values { get; set; }

    public NbtLongArray(string name, long[] values)
    {
        Name = name;
        Values = values;
    }
}

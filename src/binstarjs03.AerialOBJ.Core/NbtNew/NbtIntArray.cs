namespace binstarjs03.AerialOBJ.Core.NbtNew;

public class NbtIntArray : INbtArray<int>
{
    public string Name { get; set; }
    public int[] Values { get; set; }

    public NbtIntArray(string name, int[] values)
    {
        Name = name;
        Values = values;
    }
}

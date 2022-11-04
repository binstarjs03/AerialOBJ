using System.Collections.Generic;

namespace binstarjs03.AerialOBJ.Core.NbtNew;

public class NbtList<T> : List<T>, INbtCollection where T : class, INbt
{
    public string Name { get; }
    public NbtType Type => NbtType.NbtList;

    public NbtList(string name)
    {
        Name = name;
    }

    public override string ToString()
    {
        return $"<{GetType}> {Name} - Nbts: {Count} nbts";
    }
}

using System.Collections.Generic;

namespace binstarjs03.AerialOBJ.Core.NbtNew;

public class NbtList<T> : List<T>, INbtCollection where T : class, INbt
{
    public string Name { get; }

    public NbtList(string name)
    {
        Name = name;
    }
}

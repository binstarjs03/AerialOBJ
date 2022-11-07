using System.Collections.Generic;
using System.Diagnostics;

namespace binstarjs03.AerialOBJ.Core.NbtNew;

[DebuggerDisplay("{this.ToString(),nq}")]
public class NbtCompound : Dictionary<string, INbt>, INbtCollection
{
    public string Name { get; }
	public NbtType Type => NbtType.NbtCompound;

	public NbtCompound(string name)
	{
		Name = name;
	}

	public void Add(INbt nbt)
	{
		Add(nbt.Name, nbt);
	}

	public INbt Get(string nbtName)
	{
		return this[nbtName];
	}

	public T Get<T>(string nbtName) where T : class, INbt
	{
		if (this[nbtName] is not T ret)
			throw new System.InvalidCastException();
		return ret;
	}

    public override string ToString()
    {
        return $"{Type} - \"{Name}\": {Count} nbt(s)";
    }
}

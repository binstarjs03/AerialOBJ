using System.Collections.Generic;

namespace binstarjs03.AerialOBJ.Core.NbtNew;

public class NbtCompound : Dictionary<string, INbt>, INbtCollection
{
    public string Name { get; }

	public NbtCompound(string name)
	{
		Name = name;
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
}

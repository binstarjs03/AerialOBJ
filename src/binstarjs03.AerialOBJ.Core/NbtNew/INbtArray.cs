namespace binstarjs03.AerialOBJ.Core.NbtNew;

public interface INbtArray<T> : INbt
{
    public T[] Values { get; }
}

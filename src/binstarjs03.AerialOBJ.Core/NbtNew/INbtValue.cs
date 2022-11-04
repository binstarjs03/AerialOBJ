namespace binstarjs03.AerialOBJ.Core.NbtNew;

public interface INbtValue<T> : INbt
{
    public T Value { get; }
}

namespace binstarjs03.AerialOBJ.Core.MinecraftWorld;
public interface IChunkHighestBlockInfo
{
    string[,] Names { get; }
    int[,] Heights { get; }
}

public struct Foo
{
    public int x;
}

public class Bar
{
    public Foo[] Foos { get; set; }
}

public class Baz
{
    public void Method(Foo[] foos)
    {
        ref Foo foo = ref foos[0];

    }
}
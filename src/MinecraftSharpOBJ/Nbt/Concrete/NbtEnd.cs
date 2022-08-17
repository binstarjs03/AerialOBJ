using binstarjs03.MinecraftSharpOBJ.Nbt.Abstract;
namespace binstarjs03.MinecraftSharpOBJ.Nbt.Concrete;

public class NbtEnd : NbtBase {
    public override NbtType NbtType {
        get { return NbtType.NbtEnd; }
    }

    public override string NbtTypeName {
        get { return Nbt.NbtTypeName.NbtEnd; }
    }

    public override NbtEnd Clone() {
        return new();
    }
}

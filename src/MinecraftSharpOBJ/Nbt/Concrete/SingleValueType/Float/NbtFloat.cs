using binstarjs03.MinecraftSharpOBJ.Nbt.Abstract;
namespace binstarjs03.MinecraftSharpOBJ.Nbt.Concrete;

public class NbtFloat : NbtNumericType<float> {
    public NbtFloat() : base() {
        return;
    }

    public NbtFloat(string name) : base(name) {
        return;
    }

    public NbtFloat(float value) : base(value) {
        return;
    }

    public NbtFloat(string name, float value) : base(name, value) {
        return;
    }

    public override NbtType NbtType {
        get { return NbtType.NbtFloat; }
    }

    public override string NbtTypeName {
        get { return Nbt.NbtTypeName.NbtFloat; }
    }

    protected override char ValuePostfix {
        get { return NbtNumericPostfix.NbtFloat; }
    }

    public override NbtFloat Clone() {
        return new(_name, _value);
    }
}

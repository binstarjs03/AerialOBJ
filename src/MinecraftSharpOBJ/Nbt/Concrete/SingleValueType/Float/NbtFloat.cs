using binstarjs03.MinecraftSharpOBJ.Nbt.Abstract;
namespace binstarjs03.MinecraftSharpOBJ.Nbt.Concrete;

public class NbtFloat : NbtSingleValueType<float> {
    public NbtFloat() : base() {  }

    public NbtFloat(string name) : base(name) {  }

    public NbtFloat(float value) : base(value) {  }

    public NbtFloat(string name, float value) : base(name, value) {  }

    public override NbtType NbtType => NbtType.NbtFloat;

    public override string NbtTypeName => Nbt.NbtTypeName.NbtFloat;

    protected override char ValuePostfix => NbtSingleValueTypePostfix.NbtFloat;

    public override NbtFloat Clone() {
        return new(_name, _value);
    }

    protected override void Deserialize(IO.NbtBinaryReader reader) {
        _value = reader.ReadFloat();
    }
}

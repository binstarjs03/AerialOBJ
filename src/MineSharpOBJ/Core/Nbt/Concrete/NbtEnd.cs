using binstarjs03.MineSharpOBJ.Core.Nbt.Abstract;
using binstarjs03.MineSharpOBJ.Core.Nbt.IO;

namespace binstarjs03.MineSharpOBJ.Core.Nbt.Concrete;

public class NbtEnd : NbtBase {
    public override NbtType NbtType => NbtType.NbtEnd;

    public override NbtTypeBase NbtTypeBase => NbtTypeBase.NbtBase;

    public override NbtEnd Clone() {
        return new();
    }

    protected override void Deserialize(NbtBinaryReader reader) { }
}

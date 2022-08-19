using System;
namespace binstarjs03.MinecraftSharpOBJ.Nbt;

public static class NbtTypeName {
    public static readonly string NbtEnd = nameof(Concrete.NbtEnd);
    public static readonly string NbtByte = nameof(Concrete.NbtByte);
    public static readonly string NbtShort = nameof(Concrete.NbtShort);
    public static readonly string NbtInt = nameof(Concrete.NbtInt);
    public static readonly string NbtLong = nameof(Concrete.NbtLong);
    public static readonly string NbtFloat = nameof(Concrete.NbtFloat);
    public static readonly string NbtDouble = nameof(Concrete.NbtDouble);
    public static readonly string NbtArrayByte = nameof(Concrete.NbtArrayByte);
    public static readonly string NbtString = nameof(Concrete.NbtString);
    public static readonly string NbtList = nameof(Concrete.NbtList);
    public static readonly string NbtCompound = nameof(Concrete.NbtCompound);
    public static readonly string NbtArrayInt = nameof(Concrete.NbtArrayInt);
    public static readonly string NbtArrayLong = nameof(Concrete.NbtArrayLong);

    public static string FromEnum(NbtType type) {
        return type switch {
            NbtType.NbtEnd => NbtEnd,
            NbtType.NbtByte => NbtByte,
            NbtType.NbtShort => NbtShort,
            NbtType.NbtInt => NbtInt,
            NbtType.NbtLong => NbtLong,
            NbtType.NbtFloat => NbtFloat,
            NbtType.NbtDouble => NbtDouble,
            NbtType.NbtArrayByte => NbtArrayByte,
            NbtType.NbtString => NbtString,
            NbtType.NbtList => NbtList,
            NbtType.NbtCompound => NbtCompound,
            NbtType.NbtArrayInt => NbtArrayInt,
            NbtType.NbtArrayLong => NbtArrayLong,
            _ => throw new Exception("Undefined Nbt Type"),
        };
    }
}

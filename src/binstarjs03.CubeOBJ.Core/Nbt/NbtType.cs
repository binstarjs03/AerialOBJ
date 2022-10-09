namespace binstarjs03.CubeOBJ.Core.Nbt;

/// <summary>
/// Enumerations of different type of nbt. Instead of using reflection to
/// determine the type, we can just reference property NbtType then it will 
/// return enumeration instance of <see cref="NbtType"/>
/// </summary>
public enum NbtType
{
    NbtEnd = 0,
    NbtByte = 1,
    NbtShort = 2,
    NbtInt = 3,
    NbtLong = 4,
    NbtFloat = 5,
    NbtDouble = 6,
    NbtArrayByte = 7,
    NbtString = 8,
    NbtList = 9,
    NbtCompound = 10,
    NbtArrayInt = 11,
    NbtArrayLong = 12,
}

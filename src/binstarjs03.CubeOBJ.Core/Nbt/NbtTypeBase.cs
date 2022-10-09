namespace binstarjs03.CubeOBJ.Core.Nbt;

/// <summary>
/// Enumerations of different type of nbt bases. Instead of using reflection to
/// determine which class instance is derived from, we can just reference 
/// property NbtTypeBase then it will return enumeration instance of 
/// <see cref="NbtTypeBase"/>
/// </summary>
public enum NbtTypeBase
{
    NbtBase = 0,
    NbtContainerType = 1,
    NbtArrayType = 2,
    NbtSingleValueType = 3,
}

using System;

using binstarjs03.AerialOBJ.Core.CoordinateSystem;

namespace binstarjs03.AerialOBJ.Core.Nbt;

public struct NbtTypeNamePair : IEquatable<NbtTypeNamePair>
{
    public NbtType Type { get; set; }
    public string Name { get; set; }

    public NbtTypeNamePair(NbtType type, string name)
    {
        Name = name;
        Type = type;
    }

    public override string ToString()
    {
        return $"({Type} - \"{Name}\")";
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Type, Name);
    }

    public override bool Equals(object? obj)
    {
        if (obj is NbtTypeNamePair o)
            return Equals(o);
        else
            return false;
    }

    public bool Equals(NbtTypeNamePair other)
    {
        return Name == other.Name && Type == other.Type;
    }

    public static bool operator ==(NbtTypeNamePair left, NbtTypeNamePair right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(NbtTypeNamePair left, NbtTypeNamePair right)
    {
        return !(left == right);
    }
}

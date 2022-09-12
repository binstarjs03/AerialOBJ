// for why implementing IEquatable<T>, see below link:
// https://stackoverflow.com/questions/2734914/whats-the-difference-between-iequatable-and-just-overriding-object-equals

// for why GetHashCode overridden, see below link:
// https://stackoverflow.com/questions/371328/why-is-it-important-to-override-gethashcode-when-equals-method-is-overridden

using System;
namespace binstarjs03.MineSharpOBJ.Core.Utils;

public class CoordsOutOfRangeException : ArgumentOutOfRangeException {
    public CoordsOutOfRangeException() { }
    public CoordsOutOfRangeException(string message) : base(message) { }
    public CoordsOutOfRangeException(string message, Exception innerException) : base(message, innerException) { }
}

public struct Coords2 : IEquatable<Coords2> {
    public int X;
    public int Z;

    public Coords2(int x, int z) {
        X = x;
        Z = z;
    }

    public static Coords2 Origin => new(0, 0);

    public override string ToString() {
        return $"({X}, {Z})";
    }

    public bool Equals(Coords2 other) {
        return (X == other.X) && (Z == other.Z);
    }

    public override bool Equals(object? other) {
        return (
            other is Coords2 otherCoords 
            && otherCoords == this
        );
    }

    public static bool operator ==(Coords2 left, Coords2 right){
        return (left.X == right.X) && (left.Z == right.Z);
    }

    public static bool operator !=(Coords2 left, Coords2 right) {
        return !(left == right);
    }

    public override int GetHashCode() => (X, Z).GetHashCode();
}

public struct Coords3 {
    public int X;
    public int Y;
    public int Z;

    public Coords3(int x, int y, int z) {
        X = x;
        Y = y;
        Z = z;
    }

    public static Coords3 Origin => new(0, 0, 0);

    public override string ToString() {
        return $"({X}, {Y}, {Z})";
    }

    public bool Equals(Coords3 other) {
        return (X == other.X) && (Y == other.Y) && (Z == other.Z);
    }

    public override bool Equals(object? other) {
        return (
            other is Coords3 otherCoords 
            && otherCoords == this
        );
    }
    
    public static bool operator ==(Coords3 left, Coords3 right){
        return (left.X == right.X) && (left.Y == right.Y) && (left.Z == right.Z);
    }

    public static bool operator !=(Coords3 left, Coords3 right) {
        return !(left == right);
    }

    public override int GetHashCode() => (X, Y, Z).GetHashCode();
}

public struct Coords2Range : IEquatable<Coords2Range> {
    public Coords2 Min;
    public Coords2 Max;

    public Coords2Range(Coords2 min, Coords2 max) {
        Min = min;
        Max = max;
    }

    public Coords2Range(int minX, int maxX, int minZ, int maxZ) {
        Min = new Coords2(minX, minZ);
        Max = new Coords2(maxX, maxZ);
    }

    public bool IsOutside(Coords2 other) {
        bool outX = other.X < Min.X || other.X > Max.X;
        bool outZ = other.Z < Min.Z || other.Z > Max.Z;
        return outX || outZ;
    }

    public bool IsOutside(Coords2 other, out string message) {
        bool outX = (other.X < Min.X || other.X > Max.X);
        bool outZ = (other.Z < Min.Z || other.Z > Max.Z);
        string outMsgX = $"X:{other.X} is outside range ({Min.X}, {Max.X})";
        string outMsgZ = $"Z:{other.Z} is outside range ({Min.Z}, {Max.Z})";
        message = "";
        if (outX)
            message += outMsgX + '\n';
        if (outZ)
            message += outMsgZ + '\n';
        return outX || outZ;
    }

    /// <exception cref="CoordsOutOfRangeException"></exception>
    public void CheckOutside(Coords2 other) {
        if (IsOutside(other, out string message))
            throw new CoordsOutOfRangeException(message);
    }

    public override string ToString() {
        return $"(({Min.X}, {Max.X}), ({Min.Z}, {Max.Z}))";
    }

    public bool Equals(Coords2Range other) {
        return Min.Equals(other.Min) && Max.Equals(other.Max);
    }

    public override bool Equals(object? other) {
        return (
            other is Coords2Range otherCoordsRange 
            && otherCoordsRange == this
        );
    }

    public static bool operator ==(Coords2Range left, Coords2Range right) {
        return (left.Min == right.Min) && (left.Max == right.Max);
    }
    
    public static bool operator !=(Coords2Range left, Coords2Range right) {
        return !(left == right);
    }

    public override int GetHashCode() => (Min, Max).GetHashCode();
}

public struct Coords3Range : IEquatable<Coords3Range> {
    public Coords3 Min;
    public Coords3 Max;

    public Coords3Range(Coords3 min, Coords3 max) {
        Min = min;
        Max = max;
    }

    public Coords3Range(int minX, int maxX, int minY, int maxY, int minZ, int maxZ) {
        Min = new Coords3(minX, minY, minZ);
        Max = new Coords3(maxX, maxY, maxZ);
    }

    public bool IsOutside(Coords3 other) {
        bool outX = other.X < Min.X || other.X > Max.X;
        bool outY = other.Y < Min.Y || other.Y > Max.Y;
        bool outZ = other.Z < Min.Z || other.Z > Max.Z;
        return outX || outY || outZ;
    }

    public bool IsOutside(Coords3 other, out string message) {
        bool outX = other.X < Min.X || other.X > Max.X;
        bool outY = other.Y < Min.Y || other.Y > Max.Y;
        bool outZ = other.Z < Min.Z || other.Z > Max.Z;
        string outMsgX = $"X:{other.X} is outside range ({Min.X}, {Max.X})";
        string outMsgY = $"Y:{other.Y} is outside range ({Min.Y}, {Max.Y})";
        string outMsgZ = $"Z:{other.Z} is outside range ({Min.Z}, {Max.Z})";
        message = "";
        if (outX)
            message += outMsgX + '\n';
        if (outY)
            message += outMsgY + '\n';
        if (outZ)
            message += outMsgZ + '\n';
        return outX || outY || outZ;
    }

    /// <exception cref="CoordsOutOfRangeException"></exception>
    public void CheckOutside(Coords3 other) {
        if (IsOutside(other, out string message))
            throw new CoordsOutOfRangeException(message);
    }

    public override string ToString() {
        return $"(({Min.X}, {Max.X}), ({Min.Y}, {Max.Y}), ({Min.Z}, {Max.Z}))";
    }

    public bool Equals(Coords3Range other) {
        return Min.Equals(other.Min) && Max.Equals(other.Max);
    }

    public override bool Equals(object? other) {
        return (
            other is Coords3Range otherCoordsRange
            && otherCoordsRange == this
        );
    }
    
    public static bool operator ==(Coords3Range left, Coords3Range right) {
        return (left.Min == right.Min) && (left.Max == right.Max);
    }
    
    public static bool operator !=(Coords3Range left, Coords3Range right) {
        return !(left == right);
    }

    public override int GetHashCode() => (Min, Max).GetHashCode();
}

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
    public int x;
    public int z;

    public Coords2(int x, int z) {
        this.x = x;
        this.z = z;
    }

    public static Coords2 Origin => new(0, 0);

    public override string ToString() {
        return $"({x}, {z})";
    }

    public bool Equals(Coords2 other) {
        return (x == other.x) && (z == other.z);
    }

    public override bool Equals(object? other) {
        return (
            other is Coords2 otherCoords 
            && otherCoords == this
        );
    }

    public static bool operator ==(Coords2 left, Coords2 right){
        return (left.x == right.x) && (left.z == right.z);
    }

    public static bool operator !=(Coords2 left, Coords2 right) {
        return !(left == right);
    }

    public override int GetHashCode() => (x, z).GetHashCode();
}

public struct Coords3 {
    public int x;
    public int y;
    public int z;

    public Coords3(int x, int y, int z) {
        this.x = x;
        this.y = y;
        this.z = z;
    }

    public static Coords3 Origin => new(0, 0, 0);

    public override string ToString() {
        return $"({x}, {y}, {z})";
    }

    public bool Equals(Coords3 other) {
        return (x == other.x) && (y == other.y) && (z == other.z);
    }

    public override bool Equals(object? other) {
        return (
            other is Coords3 otherCoords 
            && otherCoords == this
        );
    }
    
    public static bool operator ==(Coords3 left, Coords3 right){
        return (left.x == right.x) && (left.y == right.y) && (left.z == right.z);
    }

    public static bool operator !=(Coords3 left, Coords3 right) {
        return !(left == right);
    }

    public override int GetHashCode() => (x, y, z).GetHashCode();
}

public struct Coords2Range : IEquatable<Coords2Range> {
    public Coords2 min;
    public Coords2 max;

    public Coords2Range(Coords2 min, Coords2 max) {
        this.min = min;
        this.max = max;
    }

    public Coords2Range(int minX, int maxX, int minZ, int maxZ) {
        min = new Coords2(minX, minZ);
        max = new Coords2(maxX, maxZ);
    }

    public bool IsOutside(Coords2 other) {
        bool outX = other.x < min.x || other.x > max.x;
        bool outZ = other.z < min.z || other.z > max.z;
        return outX || outZ;
    }

    public bool IsOutside(Coords2 other, out string message) {
        bool outX = (other.x < min.x || other.x > max.x);
        bool outZ = (other.z < min.z || other.z > max.z);
        string outMsgX = $"X:{other.x} is outside range ({min.x}, {max.x})";
        string outMsgZ = $"Z:{other.z} is outside range ({min.z}, {max.z})";
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
        return $"(({min.x}, {max.x}), ({min.z}, {max.z}))";
    }

    public bool Equals(Coords2Range other) {
        return min.Equals(other.min) && max.Equals(other.max);
    }

    public override bool Equals(object? other) {
        return (
            other is Coords2Range otherCoordsRange 
            && otherCoordsRange == this
        );
    }

    public static bool operator ==(Coords2Range left, Coords2Range right) {
        return (left.min == right.min) && (left.max == right.max);
    }
    
    public static bool operator !=(Coords2Range left, Coords2Range right) {
        return !(left == right);
    }

    public override int GetHashCode() => (min, max).GetHashCode();
}

public struct Coords3Range : IEquatable<Coords3Range> {
    public Coords3 min;
    public Coords3 max;

    public Coords3Range(Coords3 min, Coords3 max) {
        this.min = min;
        this.max = max;
    }

    public Coords3Range(int minX, int maxX, int minY, int maxY, int minZ, int maxZ) {
        min = new Coords3(minX, minY, minZ);
        max = new Coords3(maxX, maxY, maxZ);
    }

    public bool IsOutside(Coords3 other) {
        bool outX = other.x < min.x || other.x > max.x;
        bool outY = other.y < min.y || other.y > max.y;
        bool outZ = other.z < min.z || other.z > max.z;
        return outX || outY || outZ;
    }

    public bool IsOutside(Coords3 other, out string message) {
        bool outX = other.x < min.x || other.x > max.x;
        bool outY = other.y < min.y || other.y > max.y;
        bool outZ = other.z < min.z || other.z > max.z;
        string outMsgX = $"X:{other.x} is outside range ({min.x}, {max.x})";
        string outMsgY = $"Y:{other.y} is outside range ({min.y}, {max.y})";
        string outMsgZ = $"Z:{other.z} is outside range ({min.z}, {max.z})";
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
        return $"(({min.x}, {max.x}), ({min.y}, {max.y}), ({min.z}, {max.z}))";
    }

    public bool Equals(Coords3Range other) {
        return min.Equals(other.min) && max.Equals(other.max);
    }

    public override bool Equals(object? other) {
        return (
            other is Coords3Range otherCoordsRange
            && otherCoordsRange == this
        );
    }
    
    public static bool operator ==(Coords3Range left, Coords3Range right) {
        return (left.min == right.min) && (left.max == right.max);
    }
    
    public static bool operator !=(Coords3Range left, Coords3Range right) {
        return !(left == right);
    }

    public override int GetHashCode() => (min, max).GetHashCode();
}

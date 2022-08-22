using System;
namespace binstarjs03.MinecraftSharpOBJ.Utils;

public class CoordsOutOfRangeException : ArgumentOutOfRangeException {
    public CoordsOutOfRangeException() { }
    public CoordsOutOfRangeException(string message) : base(message) { }
    public CoordsOutOfRangeException(string message, Exception innerException) : base(message, innerException) { }
}

public struct Coords2 {
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
}

public struct Coords2Range {
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
        bool outX = (other.x < min.x || other.x > max.x);
        bool outZ = (other.z < min.z || other.z > max.z);
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
}

public struct Coords3Range {
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
        bool outX = (other.x < min.x || other.x > max.x);
        bool outY = (other.y < min.y || other.y > max.y);
        bool outZ = (other.z < min.z || other.z > max.z);
        return outX || outY || outZ;
    }

    public bool IsOutside(Coords3 other, out string message) {
        bool outX = (other.x < min.x || other.x > max.x);
        bool outY = (other.y < min.y || other.y > max.y);
        bool outZ = (other.z < min.z || other.z > max.z);
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
}

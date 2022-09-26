using System;
namespace binstarjs03.MineSharpOBJ.Core.Utils;

/// <summary>
/// High-precision Point using <see cref="double"/>
/// </summary>
public struct PointF : IEquatable<PointF> {
    public PointF(double size) {
        X = size;
        Y = size;
    }
    
    public PointF(double x, double y) {
        X = x;
        Y = y;
    }

    public double X { get; set; }
    public double Y { get; set; }

    public static PointF Origin => new(0);

    public PointF Floor => new(Math.Floor(X), Math.Floor(Y));

    public PointF Negative => new(-X, -Y);

    public static implicit operator PointF(Point point) {
        return new PointF(point.X, point.Y);
    }

    #region Implement Equality

    public override bool Equals(object? other) {
        if (other is null)
            return false;
        if (other is PointF o)
            return X == o.X && Y == o.Y;
        return false;
    }

    public bool Equals(PointF other) {
        return X == other.X && Y == other.Y;
    }

    public override int GetHashCode() {
        return HashCode.Combine(X, Y);
    }

    public static bool operator ==(PointF left, PointF right) {
        return left.Equals(right);
    }

    public static bool operator !=(PointF left, PointF right) {
        return !(left == right);
    }

    #endregion

    #region PointF operator overloads

    public static PointF operator +(PointF left, PointF right) {
        return new PointF(left.X + right.X, left.Y + right.Y);
    }
    public static PointF operator -(PointF left, PointF right) {
        return new PointF(left.X - right.X, left.Y - right.Y);
    }
    public static PointF operator *(PointF left, PointF right) {
        return new PointF(left.X * right.X, left.Y * right.Y);
    }
    public static PointF operator /(PointF left, PointF right) {
        return new PointF(left.X / right.X, left.Y / right.Y);
    }

    #endregion

    #region double operator overloads

    public static PointF operator +(PointF left, double right) {
        return new PointF(left.X + right, left.Y + right);
    }
    public static PointF operator -(PointF left, double right) {
        return new PointF(left.X - right, left.Y - right);
    }
    public static PointF operator *(PointF left, double right) {
        return new PointF(left.X * right, left.Y * right);
    }
    public static PointF operator /(PointF left, double right) {
        return new PointF(left.X / right, left.Y / right);
    }

    #endregion
}

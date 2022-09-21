using System;
namespace binstarjs03.MineSharpOBJ.Core.Utils;

/// <summary>
/// Imitate <see cref="System.Drawing.Point"/> 
/// but with a lot of operator overloads 
/// (since we cannot inherit or extend from struct, we recreate it instead)
/// </summary>
public struct Point : IEquatable<Point> {
    public Point(int size) {
        X = size;
        Y = size;
    }

    public Point(int x, int y) {
        X = x;
        Y = y;
    }

    public int X { get; set; }
    public int Y { get; set; }

    public static Point Origin => new(0);

    public static explicit operator Point(PointF point) {
        return new Point((int)point.X, (int)point.Y);
    }

    public static explicit operator Point(Coords2 coords) {
        return new Point(coords.X, coords.Z);
    }

    #region Implement Equality

    public override bool Equals(object? other) {
        if (other is null)
            return false;
        if (other is Point o)
            return X == o.X && Y == o.Y;
        return false;
    }

    public bool Equals(Point other) {
        return X == other.X && Y == other.Y;
    }

    public override int GetHashCode() {
        return HashCode.Combine(X, Y);
    }

    public static bool operator ==(Point left, Point right) {
        return left.Equals(right);
    }

    public static bool operator !=(Point left, Point right) {
        return !(left == right);
    }

    #endregion

    #region Point operator overloads

    public static Point operator +(Point left, Point right) {
        return new Point(left.X + right.X, left.Y + right.Y);
    }
    public static Point operator -(Point left, Point right) {
        return new Point(left.X - right.X, left.Y - right.Y);
    }
    public static Point operator *(Point left, Point right) {
        return new Point(left.X * right.X, left.Y * right.Y);
    }
    public static Point operator /(Point left, Point right) {
        return new Point(left.X / right.X, left.Y / right.Y);
    }

    #endregion

    #region int operator overloads

    public static Point operator +(Point left, int right) {
        return new Point(left.X + right, left.Y + right);
    }
    public static Point operator -(Point left, int right) {
        return new Point(left.X - right, left.Y - right);
    }
    public static Point operator *(Point left, int right) {
        return new Point(left.X * right, left.Y * right);
    }
    public static Point operator /(Point left, int right) {
        return new Point(left.X / right, left.Y / right);
    }

    #endregion
}

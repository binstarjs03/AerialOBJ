using System;
using System.Numerics;

namespace binstarjs03.AerialOBJ.Core.Primitives;

public struct Rangeof<TNumber> : 
    IEquatable<Rangeof<TNumber>>, 
    IEqualityOperators<Rangeof<TNumber>, Rangeof<TNumber>, bool>
    where TNumber : struct, INumber<TNumber>
{
    private TNumber _min;
    private TNumber _max;

    public TNumber Min
    {
        get => _min;
        set
        {
            if (value > _max)
                throw new InvalidRangeException($"{nameof(Min)} cannot be set higher than {nameof(Max)}");
            _min = value;
        }
    }

    public TNumber Max
    {
        get => _max;
        set
        {
            if (value < _min)
                throw new InvalidRangeException($"{nameof(Max)} cannot be set lower than {nameof(Min)}");
            _max = value;
        }
    }

    public TNumber Sum => Max - Min + TNumber.One;

    public Rangeof()
    {
        _min = TNumber.Zero;
        _max = TNumber.One;
    }

    public Rangeof(TNumber min, TNumber max)
    {
        if (max < min)
            throw new InvalidRangeException($"{nameof(max)} cannot be set lower than {nameof(min)} or vice versa");
        _min = min;
        _max = max;
    }

    public bool IsInside(TNumber num)
    {
        return num >= Min && num <= Max;
    }

    public bool IsOutside(TNumber num)
    {
        return !IsInside(num);
    }

    public void ThrowIfOutside(TNumber num)
    {
        if (IsOutside(num))
            throw new ArgumentOutOfRangeException(nameof(num), $"""value "{num}" is outside range {ToString()}""");
    }

    public override string ToString()
    {
        return $"{Min} ↔ {Max}";
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Min, Max);
    }

    public override bool Equals(object? obj)
    {
        if (obj is Rangeof<TNumber> o)
            return Equals(o);
        return false;
    }

    public bool Equals(Rangeof<TNumber> other)
    {
        return Min == other.Min
            && Max == other.Max;
    }

    public static bool operator ==(Rangeof<TNumber> left, Rangeof<TNumber> right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(Rangeof<TNumber> left, Rangeof<TNumber> right)
    {
        return !(left == right);
    }
}

/// <summary>
/// The exception that is thrown when creating an instance of <see cref="Range"/> that has invalid
/// argument (e.g argument max is smaller than min or vice versa) or an attempt on setting 
/// <see cref="Range.Max"/> that is lower than <see cref="Range.Min"/> or vice versa
/// </summary>
public class InvalidRangeException : Exception
{
    public InvalidRangeException() { }
    public InvalidRangeException(string message) : base(message) { }
    public InvalidRangeException(string message, Exception inner) : base(message, inner) { }
}

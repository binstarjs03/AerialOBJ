using System;

namespace binstarjs03.MineSharpOBJ.Core;

/// <summary>
/// Represent range of number. Handy for testing whether if value is 
/// inside or outside certain range
/// </summary>
public struct Range
{
    private int _min = int.MinValue;
    private int _max = int.MaxValue;

    public Range() { }

    /// <exception cref="InvalidRangeException"></exception>
    public Range(int min, int max)
    {
        if (max < min)
            throw new InvalidRangeException("max cannot be set lower than min or vice versa");
        Min = min;
        Max = max;
    }

    /// <exception cref="InvalidRangeException"/>
    public int Min
    {
        get => _min;
        set
        {
            if (value > Max)
                throw new InvalidRangeException("min cannot be set higher than max");
            _min = value;
        }
    }

    /// <exception cref="InvalidRangeException"/>
    public int Max
    {
        get => _max;
        set
        {
            if (value < Min)
                throw new InvalidRangeException("max cannot be set lower than min");
            _max = value;
        }
    }



    #region Object Overrides

    public override string ToString()
    {
        return $"{Min} ↔ {Max}";
    }

    public override bool Equals(object? obj)
    {
        if (obj is Range pr)
            return Min == pr.Min && Max == pr.Max;
        else
            return false;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Min, Max);
    }

    #endregion



    #region Equality Operators

    public static bool operator ==(Range left, Range right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(Range left, Range right)
    {
        return !(left == right);
    }

    #endregion



    #region Methods

    public bool IsInside(int num)
    {
        return num >= Min && num <= Max;
    }

    /// <exception cref="ArgumentOutOfRangeException"/>
    public void ThrowIfOutside(int num)
    {
        if (!IsInside(num))
            throw new ArgumentOutOfRangeException(nameof(num), "value passed is outside range");
    }

    #endregion
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
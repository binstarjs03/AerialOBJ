/*
Copyright (c) 2022, Bintang Jakasurya
All rights reserved. 

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/

using System;

namespace binstarjs03.AerialOBJ.Core;

/// <summary>
/// Represent range of number. Handy for testing whether if value is 
/// inside or outside certain range
/// </summary>
public struct Range : IEquatable<Range>
{
    private int _min = int.MinValue;
    private int _max = int.MaxValue;

    /// <exception cref="InvalidRangeException"/>
    public int Min
    {
        get => _min;
        set
        {
            if (value > _max)
                throw new InvalidRangeException($"{nameof(Min)} cannot be set higher than {nameof(Max)}");
            _min = value;
        }
    }

    /// <exception cref="InvalidRangeException"/>
    public int Max
    {
        get => _max;
        set
        {
            if (value < _min)
                throw new InvalidRangeException($"{nameof(Max)} cannot be set lower than {nameof(Min)}");
            _max = value;
        }
    }

    public int Sum => _max - _min + 1;

    public Range() { }

    public Range(int min, int max)
    {
        Min = min;
        Max = max;
    }

    public bool IsInside(int num)
    {
        return num >= Min && num <= Max;
    }

    public bool IsOutside(int num)
    {
        return !IsInside(num);
    }

    /// <exception cref="ArgumentOutOfRangeException"/>
    public void ThrowIfOutside(int num)
    {
        if (IsOutside(num))
            throw new ArgumentOutOfRangeException(nameof(num), $"value {num} is outside range {ToString()}");
    }

    #region Object Overrides

    public override string ToString()
    {
        return $"{Min} ↔ {Max}";
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Min, Max);
    }

    #endregion

    #region Equality Operators

    public override bool Equals(object? obj)
    {
        if (obj is Range o)
            return Equals(o);
        else
            return false;
    }

    public bool Equals(Range other)
    {
        return Min == other.Min && Max == other.Max;
    }

    public static bool operator ==(Range left, Range right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(Range left, Range right)
    {
        return !(left == right);
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

using System;
using System.Collections.Generic;

namespace binstarjs03.AerialOBJ.Core.ArrayPooling;
public class ArrayPool1<T>
{
    private readonly int _length;
    private readonly Queue<T[]> _pool = new();

    public ArrayPool1(int length)
    {
        _length = length;
    }

    public T[] Rent()
    {
        lock (_pool)
            if (_pool.TryDequeue(out T[]? result))
                return result;
        return new T[_length];
    }

    public void Return(T[] array)
    {
        if (array.Length != _length)
            throw new ArgumentException($"Returned array length is not equal to {_length}", nameof(array));
        lock (_pool)
            _pool.Enqueue(array);
    }
}

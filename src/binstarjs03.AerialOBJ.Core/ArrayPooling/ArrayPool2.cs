using System;
using System.Collections.Generic;

namespace binstarjs03.AerialOBJ.Core.ArrayPooling;
public class ArrayPool2<T>
{
    private readonly int _length1;
    private readonly int _length2;
    private readonly Queue<T[,]> _pool = new();

    public ArrayPool2(int length1, int length2)
    {
        _length1 = length1;
        _length2 = length2;
    }

    public T[,] Rent()
    {
        lock (_pool)
            if (_pool.TryDequeue(out T[,]? result))
                return result;
        return new T[_length1, _length2];
    }

    public void Return(T[,] array)
    {
        if (array.GetLength(0) != _length1 || array.GetLength(1) != _length2)
            throw new ArgumentException($"Returned array length is not equal to [{_length1}, {_length2}]", nameof(array));
        lock (_pool)
            _pool.Enqueue(array);
    }
}

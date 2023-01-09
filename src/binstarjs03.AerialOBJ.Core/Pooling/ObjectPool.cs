using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace binstarjs03.AerialOBJ.Core.Pooling;
public class ObjectPool<T> where T : class
{
    private readonly Queue<T> _pool = new();

    public bool Rent([NotNullWhen(true)] out T? result)
    {
        lock (_pool)
            return _pool.TryDequeue(out result);
    }

    public void Return(T obj)
    {
        lock (_pool)
            _pool.Enqueue(obj);
    }
}

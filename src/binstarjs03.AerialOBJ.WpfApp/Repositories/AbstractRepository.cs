using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace binstarjs03.AerialOBJ.WpfApp.Repositories;

public class AbstractRepository<T> : IRepository<T>
{
    private readonly Dictionary<string, T> _mapping = new();
    private readonly List<T> _items = new();

    public AbstractRepository(T @default)
    {
        Default = @default;
    }

    public IReadOnlyCollection<T> Items => _items;
    public T Default { get; }

    public void Register(string key, T item)
    {
        _mapping.Add(key, item);
        _items.Add(item);
    }

    public bool TryGet(string key, [NotNullWhen(true)] out T? item)
    {
        return _mapping.TryGetValue(key, out item!);
    }
}
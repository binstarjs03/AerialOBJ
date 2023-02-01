using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace binstarjs03.AerialOBJ.WpfApp.Repositories;

public interface IRepository<T>
{
    IReadOnlyCollection<T> Items { get; }
    T Default { get; }

    void Register(string key, T item);
    bool TryGet(string key, [NotNullWhen(true)] out T? item);
}

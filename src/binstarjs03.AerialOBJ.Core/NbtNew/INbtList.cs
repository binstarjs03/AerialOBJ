using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace binstarjs03.AerialOBJ.Core.NbtNew;

/// <summary>
/// Ultimate Polymorphism for undetermined argument type of <see cref="NbtList{T}"/>
/// </summary>
public interface INbtList : INbtCollection
{
    public NbtType ListType { get; }
    public int Count { get; }
    public IEnumerator GetEnumerator();
}

using System;
using System.Linq;
using System.Collections.Generic;
using binstarjs03.MineSharpOBJ.Core.Nbt.Abstract;
namespace binstarjs03.MineSharpOBJ.Core.Nbt.Concrete;

public class NbtCompound : NbtContainerType {
    private readonly Dictionary<string, NbtBase> _tags = new();

    public NbtCompound() : base() {
        _tags = new Dictionary<string, NbtBase>();
    }

    public NbtCompound(string name) : base(name) {
        _tags = new Dictionary<string, NbtBase>();
    }

    public NbtCompound(NbtBase[] tags) : base() {
        _tags = new Dictionary<string, NbtBase>();
        foreach (NbtBase tag in tags) {
            _tags.Add(tag.Name, tag);
        }
    }

    public NbtCompound(string name, NbtBase[] tags) : base(name) {
        _tags = new Dictionary<string, NbtBase>();
        foreach (NbtBase tag in tags) {
            _tags.Add(tag.Name, tag);
        }
    }

    public override NbtType NbtType => NbtType.NbtCompound;

    public override NbtBase[] Tags => _tags.Values.ToArray();

    public override NbtCompound Clone() {
        List<NbtBase> tags = new();
        foreach (NbtBase tag in _tags.Values) {
            tags.Add(tag.Clone());
        }
        return new NbtCompound(_name, tags.ToArray());
    }

    protected override void Deserialize(IO.NbtBinaryReader reader) {
        while (true) {
            NbtBase nbt = NewFromStream(reader);
            if (nbt.NbtType == NbtType.NbtEnd) {
                return;
            }
            Add(nbt);
        }
    }

    public override int ValueCount => _tags.Count;

    public int Length => _tags.Count;

    public NbtBase Get(string name) {
        if (!HasTag(name)) {
            string msg = $"Tag '{name}' not found";
            throw new KeyNotFoundException(msg);
        }
        return _tags[name];
    }

    public T Get<T>(string name) where T : NbtBase {
        return (T)Get(name);
    }

    public bool TryGet(string name, out NbtBase? tag) {
        if (_tags.TryGetValue(name, out tag))
            return true;
        return false;
    }

    public bool TryGet<T>(string name, out T? tag) where T : NbtBase {
        if (TryGet(name, out NbtBase? retout)) {
            tag = (T?)retout;
            return true;
        }
        tag = null;
        return false;
    }

    public void Set(NbtBase tag) {
        string key = tag.Name;
        _tags[key] = tag;
    }

    public void Add(NbtBase tag) {
        string key = tag.Name;
        if (HasTag(key)) {
            string msg = $"Cannot add tag '{tag.Name}' because '{tag.Name}' already exist "
                       + $"in compound tag of '{Name}'";
            throw new ArgumentException(msg);
        }
        _tags.Add(key, tag);
    }

    public bool Remove(string name) {
        if (HasTag(name)) {
            _tags.Remove(name);
            return true;
        }
        return false;
    }

    public void Clear() {
        _tags.Clear();
    }

    public bool HasTag(string name) {
        return _tags.ContainsKey(name);
    }

    public bool HasTag(NbtBase tag) {
        return HasTag(tag.Name);
    }
}

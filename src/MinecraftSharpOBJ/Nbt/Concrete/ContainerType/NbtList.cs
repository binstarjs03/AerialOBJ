using System;
using System.Linq;
using System.Collections.Generic;
using binstarjs03.MinecraftSharpOBJ.Nbt.Abstract;
namespace binstarjs03.MinecraftSharpOBJ.Nbt.Concrete;

public class NbtList : NbtContainerType {
    private readonly List<NbtBase> _tags = new();
    private NbtType? _listType;

    public NbtList() : base() {
        _tags = new List<NbtBase>();
    }

    public NbtList(string name) : base(name) {
        _tags = new List<NbtBase>();
    }

    public NbtList(NbtBase[] tags) : base() {
        ValidateTagTypes(tags);
        _tags = new List<NbtBase>(tags);
    }

    public NbtList(string name, NbtBase[] tags) : base(name) {
        ValidateTagTypes(tags);
        _tags = new List<NbtBase>(tags);
    }

    public override NbtType NbtType {
        get { return NbtType.NbtList; }
    }

    public override string NbtTypeName {
        get { return Nbt.NbtTypeName.NbtList; }
    }

    public NbtBase[] Tags {
        get { return _tags.ToArray(); }
    }

    public NbtType? ListType {
        get { return _listType; }
    }

    public override NbtList Clone() {
        List<NbtBase> tags = new();
        foreach (NbtBase tag in _tags)
            tags.Add((NbtBase)tag.Clone());
        return new NbtList(_name, tags.ToArray());
    }

    public override string ToString() {
        string ret = $"{base.ToString()} - tags: {_tags.Count} tags";
        return ret;
    }

    private void ValidateTagType(NbtBase tag) {
        if (_listType is null) {
            SetListType(tag.NbtType);
            return;
        }
        if (tag.NbtType != _listType)
            throw new ArgumentException("All tag type is not same");
    }

    private void ValidateTagTypes(NbtBase[] tags) {
        foreach (NbtBase tag in tags)
            ValidateTagType(tag);
    }

    public void SetListType(NbtType type) {
        foreach (NbtBase tag in _tags)
            if (tag.NbtType != type) {
                string msg = $"Cannot set TagList of '{_name}' type to '{type}'"
                           + $"because one of its tag type is not '{type}' (was '{tag.NbtType}')";
                throw new ArgumentException(msg);
            }
        _listType = type;
    }

    public int Length() {
        return _tags.Count;
    }

    public NbtBase Get(int index) {
        return _tags[index];
    }

    public T Get<T>(int index) where T : NbtBase {
        return (T)_tags[index];
    }

    public void Set(int index, NbtBase tag) {
        ValidateTagType(tag);
        _tags[index] = tag;
    }

    public void Add(NbtBase tag) {
        ValidateTagType(tag);
        _tags.Add(tag);
    }

    public void Add(NbtBase[] tags) {
        ValidateTagTypes(tags);
        foreach (NbtBase tag in tags)
            Add(tag);
    }

    public void Remove(int index) {
        _tags.RemoveAt(index);
    }

    public void Clear() {
        _tags.Clear();
    }

    public void Overwrite(NbtBase[] tags) {
        Clear();
        Add(tags);
    }
}

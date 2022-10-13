using System;
using System.Collections.Generic;

namespace binstarjs03.AerialOBJ.Core.Nbt;

public class NbtList : NbtContainerType
{
    private readonly List<NbtBase> _tags = new();
    private NbtType? _listType;

    public NbtList() : base()
    {
        _tags = new List<NbtBase>();
    }

    public NbtList(string name) : base(name)
    {
        _tags = new List<NbtBase>();
    }

    public NbtList(NbtBase[] tags) : base()
    {
        ValidateTagTypes(tags);
        _tags = new List<NbtBase>(tags);
    }

    public NbtList(string name, NbtBase[] tags) : base(name)
    {
        ValidateTagTypes(tags);
        _tags = new List<NbtBase>(tags);
    }

    public override NbtType NbtType => NbtType.NbtList;

    public override NbtBase[] Tags => _tags.ToArray();

    public override int ValueCount => _tags.Count;

    public NbtType? ListType => _listType;

    public override NbtList Clone()
    {
        List<NbtBase> tags = new();
        foreach (NbtBase tag in _tags)
            tags.Add(tag.Clone());
        return new NbtList(_name, tags.ToArray());
    }

    protected override void Deserialize(IO.NbtBinaryReader reader)
    {
        NbtType listType = (NbtType)reader.ReadByte();
        int elementLength = reader.ReadInt();
        for (int i = 0; i < elementLength; i++)
        {
            NbtBase tag = NewFromStream(reader, isInsideList: true, type: listType);
            Add(tag);
        }
    }

    private void ValidateTagType(NbtBase tag)
    {
        if (_listType is null)
        {
            SetListType(tag.NbtType);
            return;
        }
        if (tag.NbtType != _listType)
            throw new ArgumentException("All tag type is not same");
    }

    private void ValidateTagTypes(NbtBase[] tags)
    {
        foreach (NbtBase tag in tags)
            ValidateTagType(tag);
    }

    public void SetListType(NbtType type)
    {
        foreach (NbtBase tag in _tags)
            if (tag.NbtType != type)
            {
                string msg = $"Cannot set TagList of '{_name}' type to '{type}'"
                           + $"because one of its tag type is not '{type}' (was '{tag.NbtType}')";
                throw new ArgumentException(msg);
            }
        _listType = type;
    }

    public int Length => _tags.Count;

    public NbtBase Get(int index)
    {
        return _tags[index];
    }

    public T Get<T>(int index) where T : NbtBase
    {
        return (T)_tags[index];
    }

    public void Set(int index, NbtBase tag)
    {
        ValidateTagType(tag);
        _tags[index] = tag;
    }

    public void Add(NbtBase tag)
    {
        ValidateTagType(tag);
        _tags.Add(tag);
    }

    public void Add(NbtBase[] tags)
    {
        ValidateTagTypes(tags);
        foreach (NbtBase tag in tags)
            Add(tag);
    }

    public void Remove(int index)
    {
        _tags.RemoveAt(index);
        if (_tags.Count == 0)
            _listType = null;
    }

    public void Clear()
    {
        _tags.Clear();
        _listType = null;
    }

    public void Overwrite(NbtBase[] tags)
    {
        Clear();
        Add(tags);
    }
}

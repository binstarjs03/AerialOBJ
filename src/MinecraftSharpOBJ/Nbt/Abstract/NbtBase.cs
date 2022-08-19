﻿namespace binstarjs03.MinecraftSharpOBJ.Nbt.Abstract;

public abstract class NbtBase {
    protected string _name = "";
    protected NbtContainerType? _parent; // currently unused


    public NbtBase() {
        _name = "";
    }

    public NbtBase(string name) {
        _name = name;
    }

    public string Name {
        get { return _name; }
        set { _name = value; }
    }

    public abstract NbtType NbtType {
        get;
    }

    public virtual NbtTypeBase NbtTypeBase {
        get { return NbtTypeBase.NbtBase; }
    }

    public abstract string NbtTypeName {
        get;
    }

    public abstract NbtBase Clone();

    public override string ToString() {
        string classname = GetType().Name;
        string ret = $"<{classname}> {_name}";
        return ret;
    }

    public string GetTree() {
        NbtTree nbtTree = new(this);
        return nbtTree.Compile();
    }
}

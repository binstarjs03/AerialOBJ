﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace binstarjs03.MinecraftSharpOBJ.Nbt;

public struct NonNullString {
    private string _value;

    public NonNullString() {
        _value = string.Empty;
    }

    public NonNullString(string value) {
        _value = value;
    }

    public static NonNullString Empty {
        get { return new NonNullString(); }
    }

    public string Value {
        get { return _value is null? string.Empty : _value; }
        set { _value = value is null? string.Empty : value; }
    }

    public override string ToString() {
        return Value;
    }
}
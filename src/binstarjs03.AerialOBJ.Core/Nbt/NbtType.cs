/*
Copyright (c) 2022, Bintang Jakasurya
All rights reserved. 

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/

namespace binstarjs03.AerialOBJ.Core.Nbt;

public enum NbtType
{
    NbtEnd = 0,
    NbtByte = 1,
    NbtShort = 2,
    NbtInt = 3,
    NbtLong = 4,
    NbtFloat = 5,
    NbtDouble = 6,
    NbtByteArray = 7,
    NbtString = 8,
    NbtList = 9,
    NbtCompound = 10,
    NbtIntArray = 11,
    NbtLongArray = 12,

    // AerialOBJ specific enumeration (still unused up until now, maybe we should remove it?)
    InvalidOrUnknown = -1,
    AnyNbtType = -10,
    AnyNbtArrayType = -11,
    AnyNbtCollectionType = -12,
    AnyNbtValueType = -13,
}
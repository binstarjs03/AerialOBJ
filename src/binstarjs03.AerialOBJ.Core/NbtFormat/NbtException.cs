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

using System;

namespace binstarjs03.AerialOBJ.Core.NbtFormat;

/// <summary>
/// Root exception for nbt-related exceptions. instead catching <see cref="Exception"/> , 
/// we can just catch <see cref="NbtException"/> to catch nbt-related exceptions
/// </summary>
public class NbtException : Exception
{
    public NbtException() { }
    public NbtException(string message) : base(message) { }
    public NbtException(string message, Exception innerException) : base(message, innerException) { }
}

/// <summary>
/// The exception that is thrown when parsing nbt from data stream compression 
/// format is unrecognized
/// </summary>
public class NbtUnknownCompressionMethodException : NbtException
{
    public NbtUnknownCompressionMethodException() { }
    public NbtUnknownCompressionMethodException(string message) : base(message) { }
    public NbtUnknownCompressionMethodException(string message, Exception inner) : base(message, inner) { }
}

/// <summary>
/// The exception that is thrown when parsing nbt from data stream encountered 
/// byte data that is unrecognized nbt type
/// </summary>
public class NbtIllegalTypeException : NbtException
{
    public NbtIllegalTypeException() { }
    public NbtIllegalTypeException(string message) : base(message) { }
    public NbtIllegalTypeException(string message, Exception inner) : base(message, inner) { }
}

/// <summary>
/// The exception that is thrown when there is an exception while parsing nbt 
/// from data stream
/// </summary>
public class NbtDeserializationError : NbtException
{
    public NbtDeserializationError() { }
    public NbtDeserializationError(string message) : base(message) { }
    public NbtDeserializationError(string message, Exception inner) : base(message, inner) { }
}

public class NbtIllegalOperationException : NbtException
{
    public NbtIllegalOperationException() { }
    public NbtIllegalOperationException(string message) : base(message) { }
    public NbtIllegalOperationException(string message, Exception inner) : base(message, inner) { }
}


public class NbtNotFoundException : NbtException
{
    public NbtNotFoundException() { }
    public NbtNotFoundException(string message) : base(message) { }
    public NbtNotFoundException(string message, Exception inner) : base(message, inner) { }
}
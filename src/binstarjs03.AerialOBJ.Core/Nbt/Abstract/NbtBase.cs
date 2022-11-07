using System;
using System.IO;
using System.Text;

using binstarjs03.AerialOBJ.Core.Nbt.IO;
using binstarjs03.AerialOBJ.Core.Nbt.Utils;

namespace binstarjs03.AerialOBJ.Core.Nbt;

// TODO: current implementation of loading binary nbt
// stability haven't proved yet, need unit testing
[Obsolete($"Use {nameof(NbtNew)} library instead")]
public abstract class NbtBase
{
    protected string _name = "";
    //protected NbtContainerType? _parent;

    public NbtBase()
    {
        _name = "";
    }

    public NbtBase(string name)
    {
        _name = name;
    }

    public string Name
    {
        get { return _name; }
        set { _name = value; }
    }

    public abstract NbtType NbtType { get; }

    public abstract NbtTypeBase NbtTypeBase { get; }

    public abstract NbtBase Clone();

    public override string ToString()
    {
        return $"<{GetType().Name}> {_name}";
    }

    public string GetTree()
    {
        NbtTree nbtTree = new(this);
        return nbtTree.Compile();
    }

    protected abstract void Deserialize(NbtBinaryReader reader);

    // TODO do we even need something like this? long-list of indirect exceptions (from nested called methods)
    // does it helps us maintaining the code or does it makes it less readable and such?

    /// <exception cref="NbtUnknownCompressionMethodException"></exception>
    /// <exception cref="NbtNoDataException"></exception>
    /// <exception cref="NbtDeserializationError"></exception>
    /// <exception cref="InvalidDataException"></exception>
    /// <exception cref="EndOfStreamException"></exception>
    public static NbtBase ReadDisk(string path)
    {
        return ReadDisk(new FileInfo(path));
    }

    /// <exception cref="NbtUnknownCompressionMethodException"></exception>
    /// <exception cref="NbtNoDataException"></exception>
    /// <exception cref="NbtDeserializationError"></exception>
    /// <exception cref="InvalidDataException"></exception>
    /// <exception cref="EndOfStreamException"></exception>
    public static NbtBase ReadDisk(FileInfo fileInfo)
    {
        if (fileInfo.Length == 0)
            throw new NbtNoDataException("Data length is zero. No nbt data exist");
        using (MemoryStream ms = new(File.ReadAllBytes(fileInfo.FullName)))
        {
            return ReadStream(ms, NbtCompression.Method.AutoDetect);
        }
    }

    // TODO: bypass decompression if compression method is uncompressed
    /// <exception cref="NbtUnknownCompressionMethodException"></exception>
    /// <exception cref="NbtDeserializationError"></exception>
    /// <exception cref="InvalidDataException"></exception>
    /// <exception cref="EndOfStreamException"></exception>
    public static NbtBase ReadStream(Stream stream, NbtCompression.Method compressionMethod)
    {
        using (MemoryStream decompressed = NbtCompression.DecompressStream(stream, compressionMethod))
        using (NbtBinaryReader reader = new(decompressed))
        {
            NbtBase nbt;
            try
            {
                try
                {
                    nbt = NewFromStream(reader);
                }
                catch (EndOfStreamException e)
                { // throw specific Eos exception message
                    string msg = "Unexpected end of stream. "
                               + "There still more tags to be parsed but stream data is exhausted. ";
                    throw new EndOfStreamException(msg, e);
                }
            }
            catch (Exception e)
            {
                StringBuilder sb = new();
                sb.AppendLine("An error occured while trying to deserialize nbt stream from raw binary data");
                try
                {
                    sb.AppendLine(reader.GetReadingErrorStackAsString());
                }
                catch { }
                sb.AppendLine($"Error type: {e.GetType()}");
                sb.Append($"Error message: {e.Message}");
                throw new NbtDeserializationError(sb.ToString(), e);
                // throw exception representing whole subroutine error of this method
            }
            checkReturnedNbt(nbt, decompressed);
            return nbt;
        }

        static void checkReturnedNbt(NbtBase nbt, Stream stream)
        {
            if (nbt is NbtEnd)
            {
                string msg = "Bare end tag got returned. "
                           + "End tag should only exist inside Compound tag.";
                throw new InvalidDataException(msg);
            }
            if (stream.Position < stream.Length)
            {
                string msg = "Stream is expected to be at its end. "
                           + $"Successfully parsed an Nbt tag until position {stream.Position} "
                           + "but there still more data to be parsed. "
                           + "Mixing binary Nbt data with foreign data (or multiple binary Nbt tag) is not allowed.";
                throw new InvalidDataException(msg);
            }
        }
    }

    /// <exception cref="NullReferenceException"></exception>
    /// <exception cref="NotImplementedException"></exception>
    protected static NbtBase NewFromStream(NbtBinaryReader reader, bool isInsideList = false, NbtType? type = null)
    {
        string name = "";
        if (isInsideList && type is null)
        {
            string msg = "Nbt type cannot null if tag is inside list";
            throw new NullReferenceException(msg);
            // Theoretically, above exception should never thrown 
            // because we always pass the nbt type if its inside list
        }
        if (!isInsideList)
        {
            type = reader.ReadTagType();
            if (type != NbtType.NbtEnd)
                name = reader.ReadStringLengthPrefixed();
        }

        NbtBase nbt = type switch
        {
            NbtType.NbtEnd => new NbtEnd(),
            NbtType.NbtByte => new NbtByte(name),
            NbtType.NbtShort => new NbtShort(name),
            NbtType.NbtInt => new NbtInt(name),
            NbtType.NbtLong => new NbtLong(name),
            NbtType.NbtFloat => new NbtFloat(name),
            NbtType.NbtDouble => new NbtDouble(name),
            NbtType.NbtString => new NbtString(name),
            NbtType.NbtArrayByte => new NbtArrayByte(name),
            NbtType.NbtArrayInt => new NbtArrayInt(name),
            NbtType.NbtArrayLong => new NbtArrayLong(name),
            NbtType.NbtCompound => new NbtCompound(name),
            NbtType.NbtList => new NbtList(name),
            _ => throw new NotImplementedException($"Nbt type {type} deserialization has not been implemented yet"),
        };
        reader.NbtTagStack.Push(nbt);
        nbt.Deserialize(reader);
        reader.NbtTagStack.Pop();
        return nbt;
    }
}

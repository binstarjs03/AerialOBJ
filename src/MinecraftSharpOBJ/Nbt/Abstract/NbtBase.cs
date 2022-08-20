using System;
using System.IO;
using System.Text;
using binstarjs03.MinecraftSharpOBJ.Utils.IO;
using binstarjs03.MinecraftSharpOBJ.Nbt.IO;
using binstarjs03.MinecraftSharpOBJ.Nbt.Concrete;
namespace binstarjs03.MinecraftSharpOBJ.Nbt.Abstract;

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

    protected abstract void Deserialize(NbtBinaryReader reader);

    /// <exception cref="NbtDeserializationError"></exception>
    public static NbtBase ReadDisk(string path, ByteOrder byteOrder) {
        using (MemoryStream ms = new(File.ReadAllBytes(path))) {
            NbtBase nbt = ReadStream(ms, byteOrder, NbtCompression.Method.AutoDetect);
            return nbt;
        }
    }

    /// <exception cref="NbtDeserializationError"></exception>
    public static NbtBase ReadStream(Stream stream, ByteOrder byteOrder, NbtCompression.Method compressionMethod) {
        using (MemoryStream decompressed = NbtCompression.DecompressStream(stream, compressionMethod))
        using (NbtBinaryReader reader = new(decompressed, byteOrder)) {
            NbtBase nbt;
            try {
                try {
                    nbt = NewFromStream(reader);
                }
                catch (EndOfStreamException e) {
                    string msg = "Unexpected end of stream. "
                               + "There still more tags to be parsed but stream data is exhausted. ";
                    throw new InvalidDataException(msg, e);
                }
                catch (DecoderFallbackException e) {
                    string msg = "Failed to decode binary Nbt string "
                               + $"at steam position {decompressed.Position}. ";
                    throw new InvalidDataException(msg, e);
                }
            }
            catch (Exception e) {
                Console.Error.WriteLine(reader.GetReadingErrorStack());
                Console.Error.WriteLine($"Error type: {e.GetType()}");
                Console.Error.WriteLine($"Error message: {e.Message}");
                throw new NbtDeserializationError("An error occured while trying to deserialize nbt data from raw binary data", e);
            }
            if (nbt is NbtEnd) {
                string msg = "Bare end tag got returned. "
                           + "End tag should only exist inside Compound tag.";
                throw new InvalidDataException(msg);
            }
            if (decompressed.Position < decompressed.Length) {
                string msg = "Stream is expected to be at its end. "
                           + $"Successfully parsed an Nbt tag until position {decompressed.Position} "
                           + "but there still more data to be parsed. "
                           + "Mixing binary Nbt data with foreign data (or multiple binary Nbt tag) is not allowed.";
                throw new InvalidDataException(msg);
            }
            return nbt;
        }
    }

    // TODO: write exception xml docstrings
    protected static NbtBase NewFromStream(NbtBinaryReader reader, bool isInsideList = false, NbtType? type = null) {
        string name = "";
        if (isInsideList && type is null) {
            throw new NullReferenceException("Nbt type cannot null if tag is inside list");
        }
        else if (!isInsideList) {
            type = reader.ReadTagType();
            if (type != NbtType.NbtEnd) {
                name = reader.ReadString(sizeof(short));
            }
        }

        NbtBase nbt = type switch {
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
            _ => throw new NbtUnknownTagTypeException($"Undefined Nbt type"),
        };
        reader.NbtStackReadingState.Push(nbt);
        nbt.Deserialize(reader);
        reader.NbtStackReadingState.Pop();
        return nbt;
    }
}

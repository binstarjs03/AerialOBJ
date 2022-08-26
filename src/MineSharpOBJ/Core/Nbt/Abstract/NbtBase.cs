using System;
using System.IO;
using System.Text;
using binstarjs03.MineSharpOBJ.Core.Utils.IO;
using binstarjs03.MineSharpOBJ.Core.Nbt.IO;
using binstarjs03.MineSharpOBJ.Core.Nbt.Concrete;
using binstarjs03.MineSharpOBJ.Core.Nbt.Utils;
namespace binstarjs03.MineSharpOBJ.Core.Nbt.Abstract;

// TODO: current implementation of loading binary nbt is unstable
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

    public abstract NbtType NbtType { get; }

    public virtual NbtTypeBase NbtTypeBase => NbtTypeBase.NbtBase;

    public abstract NbtBase Clone();

    public override string ToString() {
        return $"<{GetType().Name}> {_name}";
    }

    public string GetTree() {
        NbtTree nbtTree = new(this);
        return nbtTree.Compile();
    }

    protected abstract void Deserialize(NbtBinaryReader reader);

    // TODO: Wraps File.ReadAllBytes exceptions into single exception
    /// <exception cref="NbtDeserializationError"></exception>
    /// <exception cref="InvalidDataException"></exception>
    public static NbtBase ReadDisk(string path, ByteOrder byteOrder) {
        return ReadDisk(new FileInfo(path), byteOrder);
    }

    // TODO: Wraps File.ReadAllBytes exceptions into single exception
    /// <exception cref="NbtDeserializationError"></exception>
    /// <exception cref="InvalidDataException"></exception>
    public static NbtBase ReadDisk(FileInfo fileInfo, ByteOrder byteOrder) { 
        if (fileInfo.Length == 0)
            throw new ArgumentOutOfRangeException( nameof(fileInfo), "File size is equal to zero. No nbt data exist");
        using (MemoryStream ms = new(File.ReadAllBytes(fileInfo.FullName))) {
            return ReadStream(ms, byteOrder, NbtCompression.Method.AutoDetect);
        }
    }

    // TODO: bypass decompression if compression method is uncompressed
    /// <exception cref="NbtDeserializationError"></exception>
    /// <exception cref="InvalidDataException"></exception>
    public static NbtBase ReadStream(Stream stream, ByteOrder byteOrder, NbtCompression.Method compressionMethod) {    
        using (MemoryStream decompressed = NbtCompression.DecompressStream(stream, compressionMethod))
        using (NbtBinaryReader reader = new(decompressed, byteOrder)) {
            NbtBase nbt;
            try {
                try {
                    nbt = NewFromStream(reader);
                }
                catch (EndOfStreamException e) { // throw specific Eos exception message
                    string msg = "Unexpected end of stream. "
                               + "There still more tags to be parsed but stream data is exhausted. ";
                    throw new EndOfStreamException(msg, e);
                }
            }
            catch (Exception e) { // throw exception representing whole subroutine error
                StringBuilder sb = new();
                sb.AppendLine("An error occured while trying to deserialize nbt stream from raw binary data");
                try { 
                    sb.AppendLine(reader.GetReadingErrorStack());
                }
                catch { }
                sb.AppendLine($"Error type: {e.GetType()}");
                sb.AppendLine($"Error message: {e.Message}");
                throw new NbtDeserializationError(sb.ToString(), e);
            }
            checkReturnedNbt(nbt, decompressed);
            return nbt;
        }

        static void checkReturnedNbt(NbtBase nbt, Stream stream) {
            if (nbt is NbtEnd) {
                string msg = "Bare end tag got returned. "
                           + "End tag should only exist inside Compound tag.";
                throw new InvalidDataException(msg);
            }
            if (stream.Position < stream.Length) {
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
    /// <exception cref="NbtDeserializationError"></exception>
    protected static NbtBase NewFromStream(NbtBinaryReader reader, bool isInsideList = false, NbtType? type = null) {
        string name = "";
        if (isInsideList && type is null) {
            string msg = $"Nbt type cannot null if tag is inside list";
            throw new NullReferenceException(msg);
        }
        else if (!isInsideList) {
            type = reader.ReadTagType();
            if (type != NbtType.NbtEnd)
                name = reader.ReadString(sizeof(short));
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
            _ => throw new NotImplementedException($"Nbt type {type} deserialization has not been implemented yet"),
        };
        reader.NbtTagStack.Push(nbt);
        try {
            nbt.Deserialize(reader);
        }
        catch (Exception e) { // throw exception representing current nbt
            string msg = $"Failed to deserialize Nbt data {name} of {type}";
            throw new NbtDeserializationError(msg, e);
        }
        reader.NbtTagStack.Pop();
        return nbt;
    }
}

#include "../IO/BinaryReader.h"
#include "Nbt.h"

Nbt ReadNbtSwitch(BinaryReader& reader, Endianness endian, NbtType type, bool insideList);

NbtType ReadNbtType(BinaryReader& reader) {
    int8_t type = reader.ReadInt8();
    if (type >= 0 && type <= 12)
        return (NbtType)type;
    else {
        std::string errmsg = "Unknown Nbt type ";
        errmsg.push_back((char)type);
        throw errmsg;
    }
}

NbtByte ReadNbtByte(BinaryReader& reader, std::string name) {
    NbtByte result(name);
    result.Value = reader.ReadInt8();
    return result;
}

NbtShort ReadNbtShort(BinaryReader& reader, std::string name, Endianness endian) {
    NbtShort result(name);
    result.Value = reader.ReadInt16(endian);
    return result;
}

NbtInt ReadNbtInt(BinaryReader& reader, std::string name, Endianness endian) {
    NbtInt result(name);
    result.Value = reader.ReadInt32(endian);
    return result;
}

NbtLong ReadNbtLong(BinaryReader& reader, std::string name, Endianness endian) {
    NbtLong result(name);
    result.Value = reader.ReadInt64(endian);
    return result;
}

NbtFloat ReadNbtFloat(BinaryReader& reader, std::string name, Endianness endian) {
    NbtFloat result(name);
    result.Value = reader.ReadFloatSingle(endian);
    return result;
}

NbtDouble ReadNbtDouble(BinaryReader& reader, std::string name, Endianness endian) {
    NbtDouble result(name);
    result.Value = reader.ReadFloatDouble(endian);
    return result;
}

NbtString ReadNbtString(BinaryReader& reader, std::string name, Endianness endian) {
    NbtString result(name);
    int16_t length = reader.ReadInt16(endian);
    result.Value = reader.ReadStringUTF8(length);
    return result;
}

NbtByteArray ReadNbtByteArray(BinaryReader& reader, std::string name, Endianness endian) {
    NbtByteArray result(name);
    int32_t length = reader.ReadInt32(endian);
    for (int32_t i = 0; i < length; i++)
        result.Values.push_back(reader.ReadInt8());
    return result;
}

NbtIntArray ReadNbtIntArray(BinaryReader& reader, std::string name, Endianness endian) {
    NbtIntArray result(name);
    int32_t length = reader.ReadInt32(endian);
    for (int32_t i = 0; i < length; i++)
        result.Values.push_back(reader.ReadInt32(endian));
    return result;
}

NbtLongArray ReadNbtLongArray(BinaryReader& reader, std::string name, Endianness endian) {
    NbtLongArray result(name);
    int32_t length = reader.ReadInt32(endian);
    for (int32_t i = 0; i < length; i++)
        result.Values.push_back(reader.ReadInt64(endian));
    return result;
}

NbtCompound ReadNbtCompound(BinaryReader& reader, std::string name, Endianness endian) {
    NbtCompound result(name);
    NbtType type = ReadNbtType(reader);
    while (type != NbtTypeEnd)
    {
        Nbt nbt = ReadNbtSwitch(reader, endian, type, false);
        result.Nbts.insert(std::make_pair(nbt.Name, nbt));
        type = ReadNbtType(reader);
    }
    return result;
}

NbtList ReadNbtList(BinaryReader& reader, std::string name, Endianness endian) {
    NbtList result(name);
    NbtType listType = ReadNbtType(reader);
    int32_t length = reader.ReadInt32(endian);
    for (int32_t i = 0; i < length; i++)
    {
        Nbt nbt = ReadNbtSwitch(reader, endian, listType, true);
        result.Nbts.push_back(nbt);
    }
    return result;
}

Nbt ReadNbtSwitch(BinaryReader& reader, Endianness endian, NbtType type, bool insideList) {
    std::string name;
    if (insideList)
        name = "";
    else {
        uint16_t length = reader.ReadUInt16(endian);
        name = reader.ReadStringUTF8(length);
    }

    switch (type)
    {
        case NbtTypeEnd:
            throw "Invalid state, trying to instantiate NbtEnd";
            break;
        case NbtTypeByte:
            return ReadNbtByte(reader, name);
        case NbtTypeShort:
            return ReadNbtShort(reader, name, endian);
        case NbtTypeInt:
            return ReadNbtInt(reader, name, endian);
        case NbtTypeLong:
            return ReadNbtLong(reader, name, endian);
        case NbtTypeFloat:
            return ReadNbtFloat(reader, name, endian);
        case NbtTypeDouble:
            return ReadNbtDouble(reader, name, endian);
        case NbtTypeByteArray:
            return ReadNbtByteArray(reader, name, endian);
        case NbtTypeString:
            return ReadNbtString(reader, name, endian);
        case NbtTypeList:
            return ReadNbtList(reader, name, endian);
        case NbtTypeCompound:
            return ReadNbtCompound(reader, name, endian);
        case NbtTypeIntArray:
            return ReadNbtIntArray(reader, name, endian);
        case NbtTypeLongArray:
            return ReadNbtLongArray(reader, name, endian);
        default:
            throw "Invalid state, unknown type";
            break;
    }
}

Nbt Parse(BinaryReader& reader, Endianness endian) {
    NbtType type = ReadNbtType(reader);
    return ReadNbtSwitch(reader, endian, type, false);
}

Nbt Deserialize(uint8_t* data, uint32_t length, Endianness endian) {
    BinaryReader reader(data, length);
    return Parse(reader, endian);
}

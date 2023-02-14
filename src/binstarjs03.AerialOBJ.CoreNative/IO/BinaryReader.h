#pragma once
#include <cstdint>
#include <string>
#include "Endianness.h"

class BinaryReader {
public:
    BinaryReader(uint8_t* data, uint32_t length);
    bool IsEofReached();
    uint8_t ReadUInt8();
    int8_t ReadInt8();
    uint16_t ReadUInt16(Endianness endian);
    int16_t ReadInt16(Endianness endian);
    int32_t ReadInt32(Endianness endian);
    uint32_t ReadUInt32(Endianness endian);
    int64_t ReadInt64(Endianness endian);
    uint64_t ReadUInt64(Endianness endian);
    float ReadFloatSingle(Endianness endian);
    double ReadFloatDouble(Endianness endian);
    /*std::string ReadStringLengthPrefixed(Endianness endian);*/
private:
    uint8_t* data;
    uint32_t length;
    uint32_t pos;
};
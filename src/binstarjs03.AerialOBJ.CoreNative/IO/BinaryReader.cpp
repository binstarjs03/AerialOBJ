#include <cstdint>
#include <string>
#include "Endianness.h"
#include "BinaryReader.h"

BinaryReader::BinaryReader(uint8_t* data, uint32_t length) {
    this->data = data;
    this->length = length;
    this->pos = 0;
}

bool BinaryReader::IsEofReached() {
    return this->pos >= this->length;
}

int8_t BinaryReader::ReadInt8() {
    uint8_t result = *(data + pos);
    pos++;
    return (int8_t)result;
}

uint8_t BinaryReader::ReadUInt8() {
    uint8_t result = *(data + pos);
    pos++;
    return result;
}

void FillBuffer(uint8_t* buff, uint8_t* data, uint32_t length, uint32_t offset, Endianness endian) {
    if (endian == LittleEndian)
        for (uint32_t i = 0; i < length; i++)
            buff[i] = *(data + offset + i);
    else
        for (uint32_t i = 0; i < length; i++)
            buff[i] = *(data + offset + length - 1 - i);
}

int16_t BinaryReader::ReadInt16(Endianness endian) {
    int16_t result = 0;
    constexpr uint32_t length = 2;
    uint8_t buff[length];
    
    FillBuffer(buff, this->data, length, this->pos, endian);
    std::memcpy(&result, buff, length);

    pos += length;
    return result;
}

uint16_t BinaryReader::ReadUInt16(Endianness endian) {
    uint16_t result = 0;
    constexpr uint32_t length = 2;
    uint8_t buff[length];

    FillBuffer(buff, this->data, length, this->pos, endian);
    std::memcpy(&result, buff, length);

    pos += length;
    return result;
}

int32_t BinaryReader::ReadInt32(Endianness endian) {
    int32_t result = 0;
    constexpr uint32_t length = 4;
    uint8_t buff[length];

    FillBuffer(buff, this->data, length, this->pos, endian);
    std::memcpy(&result, buff, length);

    pos += length;
    return result;
}

uint32_t BinaryReader::ReadUInt32(Endianness endian) {
    uint32_t result = 0;
    constexpr uint32_t length = 4;
    uint8_t buff[length];

    FillBuffer(buff, this->data, length, this->pos, endian);
    std::memcpy(&result, buff, length);

    pos += length;
    return result;
}

int64_t BinaryReader::ReadInt64(Endianness endian) {
    int64_t result = 0;
    constexpr uint32_t length = 8;
    uint8_t buff[length];

    FillBuffer(buff, this->data, length, this->pos, endian);
    std::memcpy(&result, buff, length);

    pos += length;
    return result;
}

uint64_t BinaryReader::ReadUInt64(Endianness endian) {
    uint64_t result = 0;
    constexpr uint32_t length = 8;
    uint8_t buff[length];

    FillBuffer(buff, this->data, length, this->pos, endian);
    std::memcpy(&result, buff, length);

    pos += length;
    return result;
}

float BinaryReader::ReadFloatSingle(Endianness endian) {
    float result = 0;
    constexpr uint32_t length = 4;
    uint8_t buff[length];

    FillBuffer(buff, this->data, length, this->pos, endian);
    std::memcpy(&result, buff, length);

    pos += length;
    return result;
}

double BinaryReader::ReadFloatDouble(Endianness endian) {
    double result = 0;
    constexpr uint32_t length = 8;
    uint8_t buff[length];

    FillBuffer(buff, this->data, length, this->pos, endian);
    std::memcpy(&result, buff, length);

    pos += length;
    return result;
}

std::string BinaryReader::ReadStringUTF8(uint16_t length) {
    uint8_t* start = data + pos; // start string
    
    const char* begin = reinterpret_cast<const char*>(start);
    const char* end = reinterpret_cast<const char*>(start + length);

    pos += length;
    return std::string(begin, end);
}
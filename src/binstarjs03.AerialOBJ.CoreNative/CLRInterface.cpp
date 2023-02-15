#include <iostream>
#include "IO/Endianness.h"
#include "IO/BinaryReader.h"
#include "NbtFormat/NbtReader.h"

extern "C" __declspec(dllexport) void HelloWorld() {
    std::cout << "Hello World from Native!" << std::endl;
}

extern "C" __declspec(dllexport) void PrintString(uint8_t * data, uint32_t length) {
    BinaryReader reader(data, length);
    auto strLength = reader.ReadUInt16(LittleEndian);
    std::string str = reader.ReadStringUTF8(strLength);
    std::cout << str;
}

extern "C" __declspec(dllexport) void ParseNbt(uint8_t * data, uint32_t length) {
    Nbt* nbt = Deserialize(data, length, BigEndian);
}
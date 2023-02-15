#include <iostream>
#include <chrono>
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
    auto start_time = std::chrono::high_resolution_clock::now();

    // this is the code we want to measure
    Nbt nbt = Deserialize(data, length, BigEndian);

    auto end_time = std::chrono::high_resolution_clock::now();
    auto elapsed_time = std::chrono::duration_cast<std::chrono::microseconds>(end_time - start_time);


    std::cout << "Finished reading nbt managed. Duration: " << elapsed_time.count() << " us" << std::endl;

}
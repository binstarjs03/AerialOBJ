#include "Nbt.h"
#include "NbtType.h"

// Nbt Abstract

Nbt::Nbt(std::string name) {
    this->Name = name;
}

NbtType Nbt::GetType() {
    return NbtType::NbtTypeEnd;
}



// Nbt Byte

NbtByte::NbtByte(std::string name) : Nbt(name) {
    this->Value = 0;
}

NbtType NbtByte::GetType() {
    return NbtType::NbtTypeByte;
}



// Nbt Short

NbtShort::NbtShort(std::string name) : Nbt(name) {
    this->Value = 0;
}

NbtType NbtShort::GetType() {
    return NbtType::NbtTypeShort;
}



// Nbt Int

NbtInt::NbtInt(std::string name) : Nbt(name) {
    this->Value = 0;
}

NbtType NbtInt::GetType() {
    return NbtType::NbtTypeInt;
}



// Nbt Long

NbtLong::NbtLong(std::string name) : Nbt(name) {
    this->Value = 0;
}

NbtType NbtLong::GetType() {
    return NbtType::NbtTypeLong;
}



// Nbt Float

NbtFloat::NbtFloat(std::string name) : Nbt(name) {
    this->Value = 0;
}

NbtType NbtFloat::GetType() {
    return NbtType::NbtTypeFloat;
}



// Nbt Double

NbtDouble::NbtDouble(std::string name) : Nbt(name) {
    this->Value = 0;
}

NbtType NbtDouble::GetType() {
    return NbtType::NbtTypeDouble;
}



// Nbt String

NbtString::NbtString(std::string name) : Nbt(name) {
    this->Value = "";
}

NbtType NbtString::GetType() {
    return NbtType::NbtTypeString;
}



// Nbt Byte Array

NbtByteArray::NbtByteArray(std::string name) : Nbt(name) {
    this->Values = std::vector<int8_t>();
}

NbtType NbtByteArray::GetType() {
    return NbtType::NbtTypeByteArray;
}



// Nbt Int Array

NbtIntArray::NbtIntArray(std::string name) : Nbt(name) {
    this->Values = std::vector<int32_t>();
}

NbtType NbtIntArray::GetType() {
    return NbtType::NbtTypeIntArray;
}



// Nbt Long Array

NbtLongArray::NbtLongArray(std::string name) : Nbt(name) {
    this->Values = std::vector<int64_t>();
}

NbtType NbtLongArray::GetType() {
    return NbtType::NbtTypeLongArray;
}



// Nbt Compound

NbtCompound::NbtCompound(std::string name) : Nbt(name) {
    this->Nbts = std::map<std::string, Nbt*>();
}

NbtType NbtCompound::GetType() {
    return NbtType::NbtTypeCompound;
}



// Nbt List

NbtList::NbtList(std::string name) : Nbt(name) {
    this->Nbts = std::vector<Nbt*>();
}

NbtType NbtList::GetType() {
    return NbtType::NbtTypeList;
}
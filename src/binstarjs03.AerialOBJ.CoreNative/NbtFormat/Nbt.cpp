#include "Nbt.h"
#include "NbtType.h"

// Nbt Abstract

NbtType Nbt::GetType() {
    return NbtType::NbtEnd;
}



// Nbt Byte

NbtByte::NbtByte() {
    this->Value = 0;
}

NbtType NbtByte::GetType() {
    return NbtType::NbtByte;
}



// Nbt Short

NbtShort::NbtShort() {
    this->Value = 0;
}

NbtType NbtShort::GetType() {
    return NbtType::NbtShort;
}



// Nbt Int

NbtInt::NbtInt() {
    this->Value = 0;
}

NbtType NbtInt::GetType() {
    return NbtType::NbtInt;
}



// Nbt Long

NbtLong::NbtLong() {
    this->Value = 0;
}

NbtType NbtLong::GetType() {
    return NbtType::NbtLong;
}



// Nbt Float

NbtFloat::NbtFloat() {
    this->Value = 0;
}

NbtType NbtFloat::GetType() {
    return NbtType::NbtFloat;
}



// Nbt Double

NbtDouble::NbtDouble() {
    this->Value = 0;
}

NbtType NbtDouble::GetType() {
    return NbtType::NbtDouble;
}



// Nbt String

NbtString::NbtString() {
    this->Value = "";
}

NbtType NbtString::GetType() {
    return NbtType::NbtString;
}



// Nbt Byte Array

NbtByteArray::NbtByteArray() {
    this->Values = std::vector<int8_t>();
}

NbtType NbtByteArray::GetType() {
    return NbtType::NbtByteArray;
}



// Nbt Int Array

NbtIntArray::NbtIntArray() {
    this->Values = std::vector<int32_t>();
}

NbtType NbtIntArray::GetType() {
    return NbtType::NbtIntArray;
}



// Nbt Long Array

NbtLongArray::NbtLongArray() {
    this->Values = std::vector<int64_t>();
}

NbtType NbtLongArray::GetType() {
    return NbtType::NbtLongArray;
}



// Nbt Compound

NbtCompound::NbtCompound() {
    this->Nbts = std::map<std::string, Nbt>();
}

NbtType NbtCompound::GetType() {
    return NbtType::NbtCompound;
}



// Nbt List

NbtList::NbtList() {
    this->Nbts = std::vector<Nbt>();
}

NbtType NbtList::GetType() {
    return NbtType::NbtList;
}
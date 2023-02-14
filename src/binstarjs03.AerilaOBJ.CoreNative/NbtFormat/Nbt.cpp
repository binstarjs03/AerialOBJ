#include "Nbt.h"
#include "NbtType.h"

NbtType Nbt::GetType() {
    return NbtType::NbtEnd;
}

NbtType NbtByte::GetType() {
    return NbtType::NbtByte;
}

NbtType NbtShort::GetType() {
    return NbtType::NbtShort;
}

NbtType NbtInt::GetType() {
    return NbtType::NbtInt;
}

NbtType NbtLong::GetType() {
    return NbtType::NbtLong;
}

NbtType NbtFloat::GetType() {
    return NbtType::NbtFloat;
}

NbtType NbtDouble::GetType() {
    return NbtType::NbtDouble;
}

NbtType NbtString::GetType() {
    return NbtType::NbtString;
}

NbtType NbtByteArray::GetType() {
    return NbtType::NbtByteArray;
}

NbtType NbtLongArray::GetType() {
    return NbtType::NbtLongArray;
}

NbtType NbtIntArray::GetType() {
    return NbtType::NbtIntArray;
}

NbtType NbtCompound::GetType() {
    return NbtType::NbtCompound;
}

NbtType NbtList::GetType() {
    return NbtType::NbtList;
}
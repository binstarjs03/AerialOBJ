#pragma once
#include <string>
#include <vector>
#include <map>
#include "NbtType.h"

class Nbt
{
public:
    std::string Name;
    virtual NbtType GetType();
protected:
};

class NbtByte : public Nbt {
public:
    NbtByte();
    std::int8_t Value;
    NbtType GetType() override;
};

class NbtShort : public Nbt {
public:
    NbtShort();
    std::int16_t Value;
    NbtType GetType() override;
};

class NbtInt : public Nbt {
public:
    NbtInt();
    std::int32_t Value;
    NbtType GetType() override;
};

class NbtLong : public Nbt {
public:
    NbtLong();
    std::int64_t Value;
    NbtType GetType() override;
};

class NbtFloat : public Nbt {
public:
    NbtFloat();
    float Value;
    NbtType GetType() override;
};

class NbtDouble : public Nbt {
public:
    NbtDouble();
    double Value;
    NbtType GetType() override;
};

class NbtString : public Nbt {
public:
    NbtString();
    std::string Value;
    NbtType GetType() override;
};

class NbtByteArray : public Nbt {
public:
    NbtByteArray();
    std::vector<int8_t> Values;
    NbtType GetType() override;
};

class NbtIntArray : public Nbt {
public:
    NbtIntArray();
    std::vector<int32_t> Values;
    NbtType GetType() override;
};

class NbtLongArray : public Nbt {
public:
    NbtLongArray();
    std::vector<int64_t> Values;
    NbtType GetType() override;
};

class NbtCompound : public Nbt {
public:
    NbtCompound();
    std::map<std::string, Nbt> Nbts;
    NbtType GetType() override;
};

class NbtList : public Nbt {
public:
    NbtList();
    std::vector<Nbt> Nbts;
    NbtType GetType() override;
};
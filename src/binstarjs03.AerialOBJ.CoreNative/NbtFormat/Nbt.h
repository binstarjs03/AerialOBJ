#pragma once
#include <string>
#include <vector>
#include <map>
#include "NbtType.h"

class Nbt
{
public:
    Nbt(std::string name);
    virtual NbtType GetType();
    std::string Name;
};

class NbtByte : public Nbt {
public:
    NbtByte(std::string name);
    NbtType GetType() override;
    std::int8_t Value;
};

class NbtShort : public Nbt {
public:
    NbtShort(std::string name);
    NbtType GetType() override;
    std::int16_t Value;
};

class NbtInt : public Nbt {
public:
    NbtInt(std::string name);
    NbtType GetType() override;
    std::int32_t Value;
};

class NbtLong : public Nbt {
public:
    NbtLong(std::string name);
    NbtType GetType() override;
    std::int64_t Value;
};

class NbtFloat : public Nbt {
public:
    NbtFloat(std::string name);
    NbtType GetType() override;
    float Value;
};

class NbtDouble : public Nbt {
public:
    NbtDouble(std::string name);
    NbtType GetType() override;
    double Value;
};

class NbtString : public Nbt {
public:
    NbtString(std::string name);
    NbtType GetType() override;
    std::string Value;
};

class NbtByteArray : public Nbt {
public:
    NbtByteArray(std::string name);
    NbtType GetType() override;
    std::vector<int8_t> Values;
};

class NbtIntArray : public Nbt {
public:
    NbtIntArray(std::string name);
    NbtType GetType() override;
    std::vector<int32_t> Values;
};

class NbtLongArray : public Nbt {
public:
    NbtLongArray(std::string name);
    NbtType GetType() override;
    std::vector<int64_t> Values;
};

class NbtCompound : public Nbt {
public:
    NbtCompound(std::string name);
    NbtType GetType() override;
    std::map<std::string, Nbt> Nbts;
};

class NbtList : public Nbt {
public:
    NbtList(std::string name);
    NbtType GetType() override;
    std::vector<Nbt> Nbts;
};
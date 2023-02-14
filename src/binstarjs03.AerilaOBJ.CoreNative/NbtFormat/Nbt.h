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
};

class NbtByte : public Nbt {
public:
    std::int8_t Value;
    NbtType GetType() override;
};

class NbtShort : public Nbt {
public:
    std::int16_t Value;
    NbtType GetType() override;
};

class NbtInt : public Nbt {
public:
    std::int32_t Value;
    NbtType GetType() override;
};

class NbtLong : public Nbt {
public:
    std::int64_t Value;
    NbtType GetType() override;
};

class NbtFloat : public Nbt {
public:
    float Value;
    NbtType GetType() override;
};

class NbtDouble : public Nbt {
public:
    double Value;
    NbtType GetType() override;
};

class NbtString : public Nbt {
public:
    std::string Value;
    NbtType GetType() override;
};

class NbtByteArray : public Nbt {
public:
    std::vector<int8_t> Values;
    NbtType GetType() override;
};

class NbtIntArray : public Nbt {
public:
    std::vector<int32_t> Values;
    NbtType GetType() override;
};

class NbtLongArray : public Nbt {
public:
    std::vector<int64_t> Values;
    NbtType GetType() override;
};

class NbtCompound : public Nbt {
public:
    std::map<std::string, Nbt> Nbts;
    NbtType GetType() override;
};

class NbtList : public Nbt {
public:
    std::vector<Nbt> Nbts;
    NbtType GetType() override;
};
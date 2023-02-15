#pragma once
#include "../IO/Endianness.h"
#include "Nbt.h"

Nbt* Deserialize(uint8_t* data, uint32_t length, Endianness endian);
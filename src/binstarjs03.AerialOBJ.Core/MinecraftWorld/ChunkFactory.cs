using System;

using binstarjs03.AerialOBJ.Core.Nbt;

namespace binstarjs03.AerialOBJ.Core.MinecraftWorld;
public static class ChunkFactory
{
    public static IChunk CreateInstance(NbtCompound chunkNbt)
    {
        int dataVersion = chunkNbt.Get<NbtInt>("DataVersion").Value;
        if (dataVersion >= 2860)
            return new Chunk2860(chunkNbt);
        else
            throw new NotImplementedException();
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace binstarjs03.AerialOBJ.Core.MinecraftWorld;

public class ChunkHighestBlockInfo
{
    public string[,] Names { get; }
    public int[,] Heights { get; }
    public ChunkHighestBlockInfo()
    {
        Names = new string[Section.BlockCount, Section.BlockCount];
        Heights = new int[Section.BlockCount, Section.BlockCount];

        for (int x = 0; x < Section.BlockCount; x++)
            for (int z = 0; z < Section.BlockCount; z++)
                Names[x, z] = Block.AirBlockName;
    }
}

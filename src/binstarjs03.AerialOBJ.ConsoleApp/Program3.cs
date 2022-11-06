using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using binstarjs03.AerialOBJ.Core.CoordinateSystem;
using binstarjs03.AerialOBJ.Core.MinecraftWorld;

namespace binstarjs03.AerialOBJ.ConsoleApp;
public class Program3
{
    static void MainDisabled(string[] args)
    {
        string path = @"C:\Users\Bin\AppData\Roaming\.minecraft\saves\1_18 World\region\r.-1.-1.mca";
        Region region = Region.Open(path);
        
        Coords2 chunkCoordsAbs = new(-9, -2);
        Chunk chunk = region.GetChunk(chunkCoordsAbs, false);
        //Block[,] blocks = chunk.GetBlockTopmost(new string[] { "minecraft:air" });

        //Coords3 blockCoordsAbs = new(-144, -52, 64);
        //Block block = chunk.GetBlock(blockCoordsAbs, false);
    }
}

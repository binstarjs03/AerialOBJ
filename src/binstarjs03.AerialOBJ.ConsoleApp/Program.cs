using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using binstarjs03.AerialOBJ.Core;
using binstarjs03.AerialOBJ.Core.CoordinateSystem;
using binstarjs03.AerialOBJ.Core.MinecraftWorld;
using binstarjs03.AerialOBJ.Core.Nbt;

namespace binstarjs03.AerialOBJ.ConsoleApp;

public class Program
{
    private static void Main(string[] args)
    {
        string path = @"C:\Users\Bin\AppData\Roaming\.minecraft\saves\Terralith 2\region\r.0.0.mca";
        Region region = Region.Open(path);
        List<Coords2> generatedChunks = new(region.GetGeneratedChunksAsCoordsRel().generatedChunksList);
        List<INbt> nbts = new(generatedChunks.Count);

        Task[] tasks = new Task[Environment.ProcessorCount];
        for (int i = 0; i < tasks.Length; i++)
        {
            int id = i;
            tasks[i] = new Task(() => ReadChunk(region, generatedChunks, nbts, id, tasks.Length), TaskCreationOptions.LongRunning);
        }
        for (int i = 0; i < tasks.Length; i++)
            tasks[i].Start();
        Task.WaitAll(tasks);
        Console.WriteLine();
    }

    private static void ReadChunk(Region region, List<Coords2> list, List<INbt> nbts, int id, int taskCount)
    {
        int partition = list.Count / taskCount;
        int min = partition * id;
        int max = min + partition;
        for (int i = min; i < max; i++)
        {
            Coords2 chunkCoords = list[i];
            nbts.Add(region.GetChunkNbt(chunkCoords, true));
        }
    }
}

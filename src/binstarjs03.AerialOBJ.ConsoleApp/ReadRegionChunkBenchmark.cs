using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Threading;
using System.Windows;

using binstarjs03.AerialOBJ.Core.CoordinateSystem;
using binstarjs03.AerialOBJ.Core.Nbt;
using binstarjs03.AerialOBJ.Core.WorldRegion;

using Region = binstarjs03.AerialOBJ.Core.WorldRegion.Region;

namespace binstarjs03.AerialOBJ.ConsoleApp;

internal class ReadRegionChunkBenchmark
{
    public class ChunkWrapper
    {
        public Chunk? Chunk;
        public Bitmap? ChunkImage;
    }

    public class ChunkJob
    {
        public Queue<Coords2> PendingReadingChunks = new();
        public Queue<Coords2> PendingMergingChunkImages = new();
        public ChunkWrapper[,] ChunkWrappers = new ChunkWrapper[Region.ChunkCount, Region.ChunkCount];
    }

    private static void Main(string[] args)
    {
        int testCount = 10;
        string path = @"C:\Users\Bin\AppData\Roaming\.minecraft\saves\1_18 World\region\r.0.0.mca";
        Region region = Region.Open(path);
        Coords2[] generatedChunks = GetGeneratedChunksAsCoords(region);
        Block[,] buffer = new Block[Section.BlockCount, Section.BlockCount];
        //List<NbtCompound> chunkNbts = new();

        List<TimeSpan> durations = new();
        for (int i = 0; i < testCount; i++)
        {
            TimeSpan duration = Run2(region, generatedChunks, buffer);
            durations.Add(duration);
            Console.WriteLine($"Test {i + 1} took {duration.TotalSeconds} s");

        }

        CalculateAverageTime(durations, testCount);
    }

    private static TimeSpan Run2(Region region, Coords2[] generatedChunks, Block[,] buffer)
    {
        DateTime startTime = DateTime.Now;
        foreach (Coords2 chunkCoords in generatedChunks)
        {
            Chunk chunk = region.GetChunk(chunkCoords, relative: true);
            chunk.GetBlockTopmost(buffer);
        }
        return DateTime.Now - startTime;
    }

    private static Coords2[] GetGeneratedChunksAsCoords(Region region)
    {
        List<Coords2> generatedChunks = new();
        for (int x = 0; x < Region.ChunkCount; x++)
        {
            for (int z = 0; z < Region.ChunkCount; z++)
            {
                Coords2 chunkCoords = new(x, z);
                if (region.HasChunkGenerated(chunkCoords))
                {
                    generatedChunks.Add(chunkCoords);
                }
                else
                    Console.WriteLine($"Skipped chunk {chunkCoords}, not generated yet");
            }
        }
        return generatedChunks.ToArray();
    }

    private static void CalculateAverageTime(List<TimeSpan> durations, int testCount)
    {
        TimeSpan average = TimeSpan.Zero;
        double std = 0;
        foreach (TimeSpan duration in durations)
        {
            average += duration;
            std += duration.TotalSeconds * duration.TotalSeconds;
        }
        average = average / testCount;
        Console.WriteLine($"Average run time: {average.TotalSeconds}");
    }

    private static Bitmap GenerateChunkImage(Chunk chunk, Block[,] buffer)
    {
        chunk.GetBlockTopmost(buffer);
        Bitmap bitmap = new(Section.BlockCount, Section.BlockCount, PixelFormat.Format32bppArgb);
        for (int x = 0; x < Section.BlockCount; x++)
        {
            for (int z = 0; z < Section.BlockCount; z++)
            {
                Color color = BlockToColor.Convert(buffer[x, z]);
                bitmap.SetPixel(x, z, color);
            }
        }
        return bitmap;
    }
}

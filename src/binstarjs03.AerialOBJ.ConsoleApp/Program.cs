using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Threading;

using binstarjs03.AerialOBJ.Core.CoordinateSystem;
using binstarjs03.AerialOBJ.Core.MinecraftWorld;

using Region = binstarjs03.AerialOBJ.Core.MinecraftWorld.Region;

namespace binstarjs03.AerialOBJ.ConsoleApp;

internal class Program
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
        DateTime startTime;
        TimeSpan duration;

        string path = @"C:\Users\Bin\AppData\Roaming\.minecraft\saves\1_18 World\region\r.0.0.mca";
        Region region = Region.Open(path);

        ChunkJob chunkJob = GetGeneratedChunksAsCoords(region);

        // create new threads up to cpu thread count
        //int threadCount = Environment.ProcessorCount;
        int threadCount = 1;
        Thread[] threads = new Thread[threadCount];
        for (int i = 0; i < threadCount; i++)
        {
            Thread thread = new(RunChunkJobThreaded)
            {
                Name = $"ChunkWorker-{i}"
            };
            threads[i] = thread;
        }
        Console.WriteLine($"Successfully created {threadCount} thread(s)");

        Console.WriteLine($"Reading all chunks in single Region file...");
        // create new timer
        startTime = DateTime.Now;

        // start all threads
        object arg = (region, chunkJob);
        foreach (Thread thread in threads)
        {
            thread.Start(arg);
            Console.WriteLine($"Dispatched {thread.Name}");
        }

        // wait for all threads to finish
        foreach (Thread thread in threads)
        {
            thread.Join();
        }

        // calculate duration
        duration = DateTime.Now - startTime;

        Console.WriteLine($"Finished reading all chunks in single Region file. Operation took {duration.TotalSeconds} s");

        //Console.WriteLine($"Merging all chunk images...");
        //// create new timer
        //startTime = DateTime.Now;

        //Bitmap finalImage = MergeChunkImage(chunkJob);

        //// calculate duration
        //duration = DateTime.Now - startTime;
        //Console.WriteLine($"Finished merging all chunk images. Operation took {duration.TotalMilliseconds} ms");

        //string savePath = @"D:\testing.png";
        //finalImage.Save(savePath, ImageFormat.Png);
        //Console.WriteLine($"Saved final image to {savePath}");
    }

    private static ChunkJob GetGeneratedChunksAsCoords(Region region)
    {
        ChunkJob chunkJob = new();
        for (int x = 0; x < Region.ChunkCount; x++)
        {
            for (int z = 0; z < Region.ChunkCount; z++)
            {
                Coords2 chunkCoords = new(x, z);
                if (region.HasChunkGenerated(chunkCoords))
                {
                    chunkJob.ChunkWrappers[x, z] = new ChunkWrapper();
                    chunkJob.PendingReadingChunks.Enqueue(chunkCoords);
                    chunkJob.PendingMergingChunkImages.Enqueue(chunkCoords);
                }
                else
                    Console.WriteLine($"Skipped chunk {chunkCoords}, not generated yet");
            }
        }
        return chunkJob;
    }

    private static void RunChunkJobThreaded(object? arg)
    {
        if (arg is null)
            throw new NullReferenceException();
        (Region region, ChunkJob chunkJob) = ((Region, ChunkJob))arg;

        Block[,] buffer = new Block[Section.BlockCount, Section.BlockCount];
        while (true)
        {
            Coords2 chunkCoords;
            lock (chunkJob.PendingReadingChunks)
            {
                if (chunkJob.PendingReadingChunks.Count > 0)
                    chunkCoords = chunkJob.PendingReadingChunks.Dequeue();
                else
                    break;
            }
            Chunk chunk = region.GetChunk(chunkCoords, relative: true);
            //chunk.GetBlockTopmost(buffer);
            //Bitmap chunkImage = GenerateChunkImage(chunk, buffer);

            //lock (chunkJob.ChunkWrappers)
            //{
            //    ChunkWrapper chunkWrapper = chunkJob.ChunkWrappers[chunkCoords.X, chunkCoords.Z];
            //    chunkWrapper.Chunk = chunk;
            //    chunkWrapper.ChunkImage = chunkImage;
            //}
        }
        Console.WriteLine($"Terminated thread {Thread.CurrentThread.Name}");
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

    private static Bitmap MergeChunkImage(ChunkJob chunkJob)
    {
        int length = Section.BlockCount * Region.ChunkCount;
        Bitmap bitmap = new(length, length, PixelFormat.Format32bppArgb);
        using (Graphics g = Graphics.FromImage(bitmap))
        {
            while (chunkJob.PendingMergingChunkImages.Count > 0)
            {
                Coords2 chunkCoords = chunkJob.PendingMergingChunkImages.Dequeue();
                Bitmap? chunkImage = chunkJob.ChunkWrappers[chunkCoords.X, chunkCoords.Z].ChunkImage;
                if (chunkImage is null)
                    continue;
                Point drawPoint = new(chunkCoords.X * Section.BlockCount, chunkCoords.Z * Section.BlockCount);
                g.DrawImage(chunkImage, drawPoint);
            }
        }
        return bitmap;
    }
}

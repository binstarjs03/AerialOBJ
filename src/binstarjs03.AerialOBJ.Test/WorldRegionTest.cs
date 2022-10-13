using binstarjs03.AerialOBJ.Core.CoordinateSystem;
using binstarjs03.AerialOBJ.Core.WorldRegion;

namespace binstarjs03.AerialOBJ.Test;

[TestClass]
public class WorldRegionTest
{
    [TestMethod]
    public void TestDataSourceStream()
    {
        string path = @"C:\Users\Bin\AppData\Roaming\.minecraft\saves\Terralith 2\region\r.0.0.mca";
        Region region = Region.Open(path, useStream: true);
        Chunk chunk1 = region.GetChunk(new Coords2(0, 0), true);
        Chunk chunk2 = region.GetChunk(new Coords2(1, 0), true);
    }

    [TestMethod]
    public void TestDataSourceNonStream()
    {
        string path = @"C:\Users\Bin\AppData\Roaming\.minecraft\saves\Terralith 2\region\r.0.0.mca";
        Region region = Region.Open(path, useStream: false);
        Chunk chunk1 = region.GetChunk(new Coords2(0, 0), true);
        Chunk chunk2 = region.GetChunk(new Coords2(1, 0), true);
    }
}

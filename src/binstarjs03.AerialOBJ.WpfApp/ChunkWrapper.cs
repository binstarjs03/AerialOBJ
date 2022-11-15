/*
Copyright (c) 2022, Bintang Jakasurya
All rights reserved. 

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/

using binstarjs03.AerialOBJ.Core.CoordinateSystem;
using binstarjs03.AerialOBJ.Core.MinecraftWorld;

namespace binstarjs03.AerialOBJ.WpfApp;

public class ChunkWrapper
{
    private readonly Chunk _chunk;
    private readonly ChunkHighestBlockInfo _highestBlocks = new();
    private readonly ChunkCache? _cache;
    private int _highestBlockHeightLimit;

    public Coords2 ChunkCoordsAbs => _chunk.ChunkCoordsAbs;
    public Coords2 ChunkCoordsRel => _chunk.ChunkCoordsRel;
    public ChunkHighestBlockInfo HighestBlocks => _highestBlocks;
    public int HighestBlockHeightLimit => _highestBlockHeightLimit;

    public ChunkWrapper(Chunk chunk, int heightLimit)
    {
        _chunk = chunk;
        if (App.Current.State.PerformanceProfile == AppStateEnums.PerformanceProfile.MaximumPerformance)
            _cache = new ChunkCache(chunk);

        // guard ourself from same value, will not invoke
        // the method if both are the same value
        _highestBlockHeightLimit = heightLimit + 1; 
        GetHighestBlock(heightLimit);
    }

    public void GetHighestBlock(int heightLimit)
    {
        if (_highestBlockHeightLimit == heightLimit)
            return;
        if (_cache is not null)
            _cache.GetHighestBlock(_highestBlocks, heightLimit);
        else
            _chunk.GetHighestBlock(_highestBlocks, heightLimit: heightLimit);
        _highestBlockHeightLimit = heightLimit;
    }
}

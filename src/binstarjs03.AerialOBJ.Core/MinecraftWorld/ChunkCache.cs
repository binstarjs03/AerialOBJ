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

using System.Collections.Generic;
using System.Linq;

using binstarjs03.AerialOBJ.Core.CoordinateSystem;

namespace binstarjs03.AerialOBJ.Core.MinecraftWorld;

public class ChunkCache
{
    private readonly List<int>[,] _highestBlockHeights = new List<int>[Section.BlockCount, Section.BlockCount];
    private readonly List<string>[,] _highestBlockNames = new List<string>[Section.BlockCount, Section.BlockCount];

    public ChunkCache(Chunk chunk)
    {
        for (int x = 0; x < Section.BlockCount; x++)
            for (int z = 0; z < Section.BlockCount; z++)
            {
                /* Concept:
                 * check if last height level block is different than current,
                 * if so, then the heighest block starts differently.
                 * 
                 * for example, we have 16 blocks stacked on top each other:
                 * - block 0 through 4 is filled with stone,
                 * - block 5 through 9 is filled with dirt,
                 * - and then block 10 through 15 is filled with diorite,
                 * then our highest block entry at whatever XZ sorted dict
                 * may looks like below:
                 * - Entry 0: 4 -> stone
                 * - Entry 1: 9 -> dirt
                 * - Entry 2: 15 -> dirt
                */

                List<int> highestBlockHeights = new();
                List<string> highestBlockNames = new();


                // initialize last block to whatever block is at lowest section at lowest Y level
                Section lowestSection = chunk.GetSectionAt(0);
                Block lowestBlock = lowestSection.GetBlockPalette(new Coords3(x, 0, z));
                string lastBlockName = lowestBlock.Name;
                
                int highestHeight = chunk.GetSectionAt(chunk.SectionsYPos.Length - 1).SectionCoordsAbs.Y * Section.BlockCount + Section.BlockCount - 1;

                // iterate through all sections from bottom to top
                for (int index = 0; index < chunk.SectionsYPos.Length; index++)
                {
                    int sectionPos = chunk.SectionsYPos[index];
                    Section section = chunk.GetSection(sectionPos);

                    // iterate through all section height levels from bottom to top
                    // (which has 16 height levels)
                    for (int y = 0; y < Section.BlockCount; y++)
                    {
                        Coords3 blockCoordsRel = new(x, y, z);
                        Block block = section.GetBlockPalette(blockCoordsRel);

                        int currentHeightLevel = section.SectionCoordsAbs.Y * Section.BlockCount + y;


                        // if we don't do this statement, the highest-most block will not be added
                        // as mostly above it is an air. if it was plains biome, most likely the last
                        // block is grass
                        if (currentHeightLevel == highestHeight)
                        {
                            highestBlockHeights.Add(highestHeight);
                            highestBlockNames.Add(lastBlockName);
                            break;
                        }
                        else if (block.Name == lastBlockName || Block.IsAir(block))
                            continue;

                        int lastHeightLevel = currentHeightLevel - 1;
                        // block is different, make new cache entry
                        highestBlockHeights.Add(lastHeightLevel);
                        highestBlockNames.Add(lastBlockName);
                        lastBlockName = block.Name;
                    }
                }
                _highestBlockHeights[x, z] = highestBlockHeights;
                _highestBlockNames[x, z] = highestBlockNames;
            }
    }

    public void GetHighestBlock(ChunkHighestBlockInfo highestBlock, int? heightLimit = null)
    {
        int limit = (int)(heightLimit is null ? int.MaxValue : heightLimit);
        for (int x = 0; x < Section.BlockCount; x++)
            for (int z = 0; z < Section.BlockCount; z++)
            {
                List<int> highestBlockHeights = _highestBlockHeights[x,z];
                List<string> highestBlockNames = _highestBlockNames[x,z];

                // iterate through all index, get the height level at that index,
                // and compare if it is higher than limit (or exhausted).
                // If so, select the block at that index as the highest block

                // TODO start from middle, and then determine either goes up or down
                // depends on if middle is lower or above than limit
                // 
                // find which index is the highest block
                int index = 0;
                for (; index < highestBlockHeights.Count - 1; index++)
                {
                    int height = highestBlockHeights[index];
                    if (height >= limit)
                        break;
                }

                // set the highest block at height limit to whatever block is at index
                highestBlock.Names[x, z] = highestBlockNames[index];
                highestBlock.Heights[x, z] = highestBlockHeights[index];
            }
    }
}

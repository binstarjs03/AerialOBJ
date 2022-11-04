using System.Collections.Generic;
using System.Linq;

using binstarjs03.AerialOBJ.Core.CoordinateSystem;

namespace binstarjs03.AerialOBJ.Core.MinecraftWorld;
public class ChunkCache
{
    private readonly SortedDictionary<int, string>[,] _highestBlocks
        = new SortedDictionary<int, string>[Section.BlockCount, Section.BlockCount];

    public ChunkCache(Chunk chunk)
    {
        // this is where we build our actual cache of highest block
        for (int x = 0; x < Section.BlockCount; x++)
            for (int z = 0; z < Section.BlockCount; z++)
            {
                SortedDictionary<int, string> highestBlockEntry = new();
                _highestBlocks[x, z] = highestBlockEntry;

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

                // iterate through all sections then iterate through
                // all sections height levels (which has 16 height levels)

                // initialize last block to whatever block is at lowest section at lowest Y level
                Section lowestSection = chunk.GetSectionAt(0);
                Block lowestBlock = lowestSection.GetBlockPalette(new Coords3(x, 0, z));
                int highestHeight = chunk.GetSectionAt(chunk.SectionsYPos.Length - 1).SectionCoordsAbs.Y * Section.BlockCount + Section.BlockCount - 1;
                string lastBlockName = lowestBlock.Name;

                for (int index = 0; index < chunk.SectionsYPos.Length; index++)
                {
                    int sectionPosition = chunk.SectionsYPos[index];
                    Section section = chunk.GetSection(sectionPosition);
                    for (int y = 0; y < Section.BlockCount; y++)
                    {
                        Coords3 blockCoordsRel = new(x, y, z);
                        Block block = section.GetBlockPalette(blockCoordsRel);

                        int currentHeightLevel = section.SectionCoordsAbs.Y * Section.BlockCount + y;
                        int lastHeightLevel = currentHeightLevel - 1;

                        // if we don't do this statement, the highest-most block will not be added
                        // as mostly above it is an air. if it was plains biome, most likely the last
                        // block is grass
                        if (currentHeightLevel == highestHeight)
                        {
                            highestBlockEntry.Add(highestHeight, lastBlockName);
                            break;
                        }
                        else if (block.Name == lastBlockName || Block.IsAir(block))
                            continue;

                        highestBlockEntry.Add(lastHeightLevel, lastBlockName);
                        lastBlockName = block.Name;
                    }
                }
            }
    }

    public void GetHighestBlock(string[,] buffer, int? heightLimit = null)
    {
        int limit = (int)(heightLimit is null ? int.MaxValue : heightLimit);
        for (int x = 0; x < Section.BlockCount; x++)
            for (int z = 0; z < Section.BlockCount; z++)
            {
                SortedDictionary<int, string> highestBlockEntry = _highestBlocks[x, z];

                // iterate through all highest block heights until it is
                // higher than limit (or exhausted), then select the block
                // at that height as the highest block

                // initialize lastHeight to lowest block
                int lastHeight = highestBlockEntry.Keys.First();
                foreach (int height in highestBlockEntry.Keys)
                {
                    lastHeight = height;
                    if (height > limit)
                        break;
                }

                // set the highest block at height limit to whatever block is at lastHeight
                buffer[x, z] = highestBlockEntry[lastHeight];
            }
    }
}

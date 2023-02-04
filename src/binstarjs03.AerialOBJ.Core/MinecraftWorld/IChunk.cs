using System;
using System.Collections.Generic;

using binstarjs03.AerialOBJ.Core.Definitions;
using binstarjs03.AerialOBJ.Core.Primitives;

namespace binstarjs03.AerialOBJ.Core.MinecraftWorld;
public interface IChunk : IDisposable
{
    const int BlockCount = 16;
    const int BlockRange = BlockCount - 1;

    int DataVersion { get; }
    string ReleaseVersion { get; } // example: "1.18.2"
    PointZ<int> CoordsAbs { get; }
    PointZ<int> CoordsRel { get; }

    void GetHighestBlockSlim(ViewportDefinition vd, BlockSlim[,] buffer, int heightLimit, List<string>? exclusions);
}
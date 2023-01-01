using System.IO;

namespace binstarjs03.AerialOBJ.WpfAppNew2.Components;
public class SavegameLoadInfo
{
    public required string WorldName { get; init; }
    public required string SavegameDirectoryPath { get; init; }
    public required int DataVersion { get; init; }
}

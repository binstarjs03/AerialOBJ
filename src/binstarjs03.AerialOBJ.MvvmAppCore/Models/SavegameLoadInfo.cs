namespace binstarjs03.AerialOBJ.MvvmAppCore.Models;
public class SavegameLoadInfo
{
    public required string WorldName { get; init; }
    public required string SavegameDirectoryPath { get; init; }
    public required int DataVersion { get; init; }
    public required int HighHeightLimit { get; init; }
    public required int LowHeightLimit { get; init; }
}

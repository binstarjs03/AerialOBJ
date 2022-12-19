namespace binstarjs03.AerialOBJ.WpfAppNew2.Services;
public readonly struct SaveFileDialogResult
{
    public required bool Result { get; init; }
    public required string SelectedFilePath { get; init; }
}

public readonly struct FolderBrowserDialogResult
{
    public required bool Result { get; init; }
    public required string SelectedDirectoryPath { get; init; }
}
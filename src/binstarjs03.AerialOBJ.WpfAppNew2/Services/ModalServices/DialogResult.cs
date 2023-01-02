namespace binstarjs03.AerialOBJ.WpfApp.Services.ModalServices;
public readonly struct FileDialogResult
{
    public required bool Result { get; init; }
    public required string SelectedFilePath { get; init; }
}

public readonly struct FolderDialogResult
{
    public required bool Result { get; init; }
    public required string SelectedDirectoryPath { get; init; }
}
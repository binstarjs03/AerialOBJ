namespace binstarjs03.AerialOBJ.WpfAppNew2.Services;
public interface IModalService
{
    void ShowMessageBox(MessageBoxArg dialogArg);
    void ShowErrorMessageBox(MessageBoxArg dialogArg);
    void ShowAbout();
    SaveFileDialogResult ShowSaveFileDialog(SaveFileDialogArg dialogArg);
}

public readonly struct MessageBoxArg
{
    public required string Message { get; init; }
    public required string Caption { get; init; }
}

public readonly struct SaveFileDialogArg
{
    public required string FileName { get; init; }
    public required string FileExtension { get; init; }
    public required string FileExtensionFilter { get; init; }
}

public readonly struct SaveFileDialogResult
{
    public required bool Result { get; init; }
    public required string Path { get; init; }
}
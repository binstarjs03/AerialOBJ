namespace binstarjs03.AerialOBJ.WpfApp;

public delegate void SavegameLoadStateHandler(SavegameLoadState state);

public enum SavegameLoadState
{
    Opened,
    Closed,
}
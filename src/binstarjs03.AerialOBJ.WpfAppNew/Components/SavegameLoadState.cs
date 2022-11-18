namespace binstarjs03.AerialOBJ.WpfAppNew.Components;

public delegate void SavegameLoadStateHandler(SavegameLoadState state);

public enum SavegameLoadState
{
    Opened,
    Closed,
}
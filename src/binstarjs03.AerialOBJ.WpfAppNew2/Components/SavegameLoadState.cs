namespace binstarjs03.AerialOBJ.WpfAppNew2.Components;
public enum SavegameLoadState
{
    Opened,
    Closed,
}

public delegate void SavegameLoadStateHandler(SavegameLoadState state);
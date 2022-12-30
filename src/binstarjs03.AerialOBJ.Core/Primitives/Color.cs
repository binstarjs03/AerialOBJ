namespace binstarjs03.AerialOBJ.Core.Primitives;
public struct Color : IColor
{
    public required byte Red { get; set; }
    public required byte Green { get; set; }
    public required byte Blue { get; set; }
    public required byte Alpha { get; set; }

    public override string ToString()
    {
        return $"A:{Alpha}, R:{Red}, G:{Green}, B:{Blue}";
    }
}

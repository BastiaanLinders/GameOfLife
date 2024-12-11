namespace GameOfLife.Services;

public record GameOfLifeOptions
{
    public int FieldWidth { get; init; } = 25;
    public int FieldHeight { get; init; } = 20;
    public int Gps { get; init; } = 3;
}

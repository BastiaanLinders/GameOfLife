using GameOfLife.Services.Mechanics;

namespace GameOfLife.Services.Abstractions;

public interface IField
{
    public int Width { get; }
    public int Height { get; }
    public Square[][] Grid { get; }
}

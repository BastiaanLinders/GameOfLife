using GameOfLife.Services.Abstractions;

namespace GameOfLife.Services;

public class FieldFactory(ParallelOptions parallelOptions) : IFieldFactory
{
    public Field Create(int width, int height)
    {
        return new Field(width, height, parallelOptions);
    }
}

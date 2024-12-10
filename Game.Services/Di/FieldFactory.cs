namespace Game.Services.Di;

public class FieldFactory(ParallelOptions parallelOptions) : IFieldFactory
{
    public Field Create(int width, int height)
    {
        return new Field(width, height, parallelOptions);
    }
}

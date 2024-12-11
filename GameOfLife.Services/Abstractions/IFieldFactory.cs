namespace GameOfLife.Services.Abstractions;

public interface IFieldFactory
{
    Field Create(int width, int height);
}

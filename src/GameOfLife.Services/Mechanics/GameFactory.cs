using GameOfLife.Services.Abstractions;

namespace GameOfLife.Services.Mechanics;

public class GameFactory(IFieldFactory fieldFactory) : IGameFactory
{
    public IGame Create()
    {
        return new Game(fieldFactory);
    }
}

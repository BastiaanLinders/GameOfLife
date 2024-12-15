using GameOfLife.Services.Abstractions;

namespace GameOfLife.Services.Mechanics;

public class Game(IFieldFactory fieldFactory) : IGame
{
    public event EventHandler OnAdvanced = delegate { };
    public IField Field => _field;


    private Field _field = null!;
    private int _generation;

    public void Init(int fieldWidth, int fieldHeight)
    {
        _field = fieldFactory.Create(fieldWidth, fieldHeight);
    }

    public void AdvanceGame()
    {
        _field!.Advance();
        _generation++;

        OnAdvanced(this, EventArgs.Empty);
    }

    public void Clear()
    {
        _generation = 0;
        _field!.Clear();
    }
}

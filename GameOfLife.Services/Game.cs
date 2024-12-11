using GameOfLife.Services.Abstractions;
using StackExchange.Profiling;

namespace GameOfLife.Services;

public class Game(IFieldFactory fieldFactory) : IGame
{
    public event EventHandler OnAdvanced = delegate { };

    private Field _field = null!;
    private int _generation;

    public void Init(int fieldWidth, int fieldHeight)
    {
        _field = fieldFactory.Create(fieldWidth, fieldHeight);
    }

    public void LetThereBeLife(CancellationToken cancellationToken)
    {
        while (cancellationToken.IsCancellationRequested == false)
        {
            var loopProfiler = MiniProfiler.StartNew("Game loop");
            AdvanceGame();
            OnAdvanced(this, EventArgs.Empty);
            Console.WriteLine(loopProfiler.RenderPlainText());
        }
    }

    public void AdvanceGame()
    {
        _field!.Advance();
        _generation++;
    }

    public void Clear()
    {
        _generation = 0;
        _field!.Clear();
    }
}

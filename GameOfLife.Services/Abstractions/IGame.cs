using GameOfLife.Services.Mechanics;

namespace GameOfLife.Services.Abstractions;

public interface IGame
{
    event EventHandler OnAdvanced;

    void Init(int fieldWidth, int fieldHeight);
    void AdvanceGame();

    IField Field { get; }
}

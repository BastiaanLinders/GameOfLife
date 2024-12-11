namespace GameOfLife.Services;

public interface IGame
{
    event EventHandler OnAdvanced;

    void Init(int fieldWidth, int fieldHeight);
    void AdvanceGame();
}

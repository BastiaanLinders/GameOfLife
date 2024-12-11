namespace GameOfLife.Services;

public interface IGameController
{
    void Initialize(GameOfLifeOptions options);
    void Start();
    void Stop();

    event EventHandler OnStarted;
    event EventHandler OnAdvanced;
    event EventHandler OnStopped;
}

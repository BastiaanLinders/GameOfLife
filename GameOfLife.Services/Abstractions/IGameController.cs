namespace GameOfLife.Services.Abstractions;

public interface IGameController
{
    void Initialize(GameOfLifeOptions options);
    void Start();
    void Stop();
    void ChangeGps(int gps);

    event EventHandler<IField> OnInitialized;
    event EventHandler OnStarted;
    event EventHandler<int> OnAdvanced;
    event EventHandler OnStopped;
    event EventHandler<int> OnGpsChanged;
}

using System.Windows.Input;

namespace GameOfLife.Ui;

public class GameStateViewModel : ViewModelBase
{
    private readonly GameState _gameState = new();

    public bool IsRunning
    {
        get => _gameState.IsRunning;
        set
        {
            _gameState.IsRunning = value;
            OnPropertyChanged(nameof(IsRunning));
        }
    }

    public int Generation
    {
        get => _gameState.Generation;
        set
        {
            _gameState.Generation = value;
            OnPropertyChanged(nameof(Generation));
        }
    }

    public int UpdateGridCount
    {
        get => _gameState.UpdateGridCount;
        set
        {
            _gameState.UpdateGridCount = value;
            OnPropertyChanged(nameof(UpdateGridCount));
        }
    }

    public int Gps
    {
        get => _gameState.Gps;
        set
        {
            _gameState.Gps = value;
            OnPropertyChanged(nameof(Gps));
        }
    }

    private class GameState
    {
        public bool IsRunning { get; set; }
        public int Generation { get; set; }
        public int UpdateGridCount { get; set; }
        public int Gps { get; set; }
    }
}

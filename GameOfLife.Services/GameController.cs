using System.Diagnostics;
using GameOfLife.Services.Abstractions;
using StackExchange.Profiling;

namespace GameOfLife.Services;

public class GameController(IGameFactory gameFactory) : IGameController
{
    public event EventHandler<IField> OnInitialized = delegate { };
    public event EventHandler OnStarted = delegate { };
    public event EventHandler OnAdvanced = delegate { };
    public event EventHandler OnStopped = delegate { };

    private CancellationTokenSource _cancellationTokenSource = new();
    private bool _isInitialized = false;
    private bool _isRunning = false;

    private IGame _game = null!;
    private int _gps = 30;

    public void Initialize(GameOfLifeOptions options)
    {
        if (!TrySafeToggle(ref _isInitialized, true))
        {
            throw new NotSupportedException("Game is already initialized.");
        }

        _game = gameFactory.Create();
        _game.OnAdvanced += OnAdvanced;
        _gps = options.Gps;

        _game.Init(options.FieldWidth, options.FieldHeight);

        OnInitialized.Invoke(this, _game.Field);
    }

    public void Start()
    {
        if (_isInitialized == false) throw new NotSupportedException("Game is not initialized.");
        if (!TrySafeToggle(ref _isRunning, true))
        {
            return;
        }

        _cancellationTokenSource = new CancellationTokenSource();
        Task.Run(async () => await LetThereBeLife(_cancellationTokenSource.Token));
        OnStarted(this, EventArgs.Empty);
    }

    public void Stop()
    {
        if (!TrySafeToggle(ref _isRunning, false))
        {
            return;
        }

        _cancellationTokenSource.Cancel();
        OnStopped(this, EventArgs.Empty);
    }

    public void ChangeGps(int gps)
    {
        _gps = gps;
    }

    private async Task LetThereBeLife(CancellationToken cancellationToken)
    {
        var gpsSw = new Stopwatch();
        while (cancellationToken.IsCancellationRequested == false)
        {
            var loopProfiler = MiniProfiler.StartNew("Game loop");
            _game!.AdvanceGame();
            OnAdvanced(this, EventArgs.Empty);
            Console.WriteLine(loopProfiler.RenderPlainText());

            await DelayToMatchGps(gpsSw);
            gpsSw.Restart();
        }
    }

    private async Task DelayToMatchGps(Stopwatch gpsSw)
    {
        var tickSpeedInMs = 1000 / _gps;
        var delay = tickSpeedInMs - (int) gpsSw.ElapsedMilliseconds;
        if (delay < tickSpeedInMs / 2)
        {
            Console.WriteLine("Warning: Tick took more than half the gps speed.");
        }

        if (delay > 0)
        {
            await Task.Delay(delay);
        }
    }

    private bool TrySafeToggle(ref bool toggle, bool value)
    {
        lock (this)
        {
            if (toggle == value)
            {
                return false;
            }

            toggle = value;
            return true;
        }
    }
}

using System.Diagnostics;
using StackExchange.Profiling;

namespace Game.Services;

public class Game
{
    public delegate void OnTick();

    private readonly ParallelOptions _parallelOptions = new() { MaxDegreeOfParallelism = Environment.ProcessorCount / 2 };

    private CancellationTokenSource _cancellationTokenSource = new();
    private int _generation;
    private MiniProfiler _profiler = MiniProfiler.StartNew("Game")!;

    private int _tickSpeedInMs = 350;

    public Field? Field { get; set; }
    public event OnTick? OnTickEvent;

    public Task Init()
    {
        Console.WriteLine("Init");

        const int fieldSize = 5000;

        Console.WriteLine($"Creating Field with size {fieldSize}x{fieldSize}");
        Field = new Field(fieldSize, fieldSize);

        Console.WriteLine("Init complete");

        Console.WriteLine(MiniProfiler.Current.RenderPlainText());

        return Task.CompletedTask;
    }

    public async Task Start()
    {
        Console.WriteLine("Start");
        _cancellationTokenSource = new CancellationTokenSource();

        var sw = new Stopwatch();
        while (_cancellationTokenSource.Token.IsCancellationRequested == false)
        {
            var drift = Math.Max(0, (int) sw.ElapsedMilliseconds - _tickSpeedInMs);
            sw.Restart();
            Tick();

            OnTickEvent?.Invoke();

            var delay = _tickSpeedInMs - (int) sw.ElapsedMilliseconds - drift;
            if (delay < _tickSpeedInMs / 2)
            {
                Console.WriteLine($"Warning: Tick took {sw.ElapsedMilliseconds}ms to execute which is more than half the tick speed.");
            }

            await Task.Delay(delay > 0 ? delay : 0);
        }

        Console.WriteLine(MiniProfiler.Current.RenderPlainText());
    }

    public void UpdateGps(int gps)
    {
        Console.WriteLine($"UpdateGps: {gps}");
        _tickSpeedInMs = 1000 / gps;
    }

    public void Stop()
    {
        Console.WriteLine("Stop");
        _cancellationTokenSource.Cancel();
    }

    public void Clear()
    {
        Console.WriteLine("Clear");

        Stop();
        Field!.Clear();
        _generation = 0;
    }


    public void Sprinkle(double probabilityOfLife)
    {
        using var _ = MiniProfiler.Current.Step("Sprinkle");

        var random = new Random(DateTime.Now.Nanosecond);
        foreach (var square in Field!.Squares)
        {
            square.IsAlive = random.NextDouble() < probabilityOfLife;
        }

        Console.WriteLine($"Sprinkle complete. Sprinkled with a probability of {probabilityOfLife:P0}, " +
                          $"{Field.Squares.Count(s => s.IsAlive)} squares are alive");
    }

    private void Tick()
    {
        Field!.Advance();
        _generation++;
    }
}

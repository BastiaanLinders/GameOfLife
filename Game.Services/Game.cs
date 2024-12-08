using System.Diagnostics;
using StackExchange.Profiling;

namespace Game.Services;

public class Game
{
    public delegate void OnTick();

    private readonly ParallelOptions _parallelOptions = new() { MaxDegreeOfParallelism = Environment.ProcessorCount / 2 };

    private CancellationTokenSource _cancellationTokenSource = new();

    private int _tickSpeedInMs = 350;
    private int _generation = 0;

    public Field? Field { get; set; }
    public event OnTick? OnTickEvent;

    public Task Init()
    {
        Console.WriteLine("Init");

        using var _ = MiniProfiler.Current.Step("Init");

        const int fieldSize = 2000;

        Console.WriteLine($"Creating Field with size {fieldSize}x{fieldSize}");
        Field = new Field(fieldSize, fieldSize);

        Console.WriteLine($"Creating lookup for {Field.Squares.Count} squares");
        var squaresLookup = Field.Squares.GroupBy(s => s.Location.Y)
            .ToDictionary(g => g.Key, g => g.ToDictionary(s => s.Location.X, s => s));

        Console.WriteLine($"Making {Field.Squares.Count} squares aware of each other");
        Parallel.ForEach(Field!.Squares, _parallelOptions, square => square.CreateAwareness(squaresLookup));

        Console.WriteLine("Init complete");

        return Task.CompletedTask;
    }

    public async Task Start()
    {
        Console.WriteLine("Start");
        _cancellationTokenSource = new CancellationTokenSource();

        var profiler = MiniProfiler.StartNew("Game")!;

        Stopwatch sw = new();
        while (_cancellationTokenSource.Token.IsCancellationRequested == false && _generation < 100)
        {
            _generation++;
            sw.Reset();

            await Tick();

            OnTickEvent?.Invoke();
            
            //var delay = _tickSpeedInMs - (int)sw.ElapsedMilliseconds;
            //await Task.Delay(delay > 0 ? delay : 0);
        }

        await profiler.StopAsync();
        Console.WriteLine(MiniProfiler.Current.RenderPlainText());
        Console.WriteLine();
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
        foreach (var square in Field!.Squares) square.IsAlive = random.NextDouble() < probabilityOfLife;

        Console.WriteLine($"Sprinkle complete. Sprinkled with a probability of {probabilityOfLife:P0}, " +
                          $"{Field.Squares.Count(s => s.IsAlive)} squares are alive");
    }

    private Task Tick()
    {
        using (MiniProfiler.Current.CustomTiming("Evaluate", ""))
        {
            Parallel.ForEach(Field!.Squares, _parallelOptions, square => square.Evaluate());
        }

        using (MiniProfiler.Current.CustomTiming("Act", ""))
        {
            Parallel.ForEach(Field!.Squares, _parallelOptions, square => square.Advance());
        }

        return Task.CompletedTask;
    }
}
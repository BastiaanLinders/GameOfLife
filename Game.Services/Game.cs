using System.Diagnostics;
using StackExchange.Profiling;

namespace Game.Services;

public class Game
{
    public delegate void OnTick();

    private readonly ParallelOptions _parallelOptions = new() { MaxDegreeOfParallelism = Environment.ProcessorCount / 2 };

    private CancellationTokenSource _cancellationTokenSource = new();
    public int Generation { get; private set; }

    private int _tickSpeedInMs = 350;

    public Field? Field { get; set; }
    public event OnTick? OnTickEvent;

    public Task Init()
    {
        const int fieldWidth = 30;
        const int fieldHeight = 20;

        Console.WriteLine("Init");
        Console.WriteLine($"Creating Field with size {fieldWidth}x{fieldHeight}");
        Field = new Field(fieldWidth, fieldHeight, _parallelOptions);

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

            var loopProfiler = MiniProfiler.StartNew("Game loop");

            Tick();

            OnTickEvent?.Invoke();

            var delay = _tickSpeedInMs - (int) sw.ElapsedMilliseconds - drift;
            if (delay < _tickSpeedInMs / 2)
            {
                Console.WriteLine($"Warning: Tick took {sw.ElapsedMilliseconds}ms to execute which is more than half the tick speed.");
            }

            await Task.Delay(delay > 0 ? delay : 0);

            Console.WriteLine(TimeProvider.System.GetLocalNow().ToString("O"));
            Console.WriteLine(loopProfiler.RenderPlainText());
            Console.WriteLine();
        }
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
        Generation = 0;
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
        Generation++;
    }
}

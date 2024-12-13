using GameOfLife.Services;
using GameOfLife.Services.Abstractions;
using Microsoft.Extensions.Hosting;

namespace GameOfLife;

public class Worker(IGameController gameController) : BackgroundService
{
    public override Task StartAsync(CancellationToken cancellationToken)
    {
        Console.WriteLine("StartAsync");
        gameController.Initialize(new GameOfLifeOptions());
        gameController.Start();
        return Task.CompletedTask;
    }

    public override Task StopAsync(CancellationToken cancellationToken)
    {
        Console.WriteLine("StopAsync");
        gameController.Stop();
        return Task.CompletedTask;
    }

    // Not used when StartAsync/StopAsync is overridden.
    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        return Task.CompletedTask;
    }
}

using GameOfLife.Services.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace GameOfLife.Services.Config;

public static class Config
{
    public static IServiceCollection AddGameOfLifeServices(this IServiceCollection services)
    {
        services.AddSingleton(new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount / 2 });
        services.AddTransient<IGameController, GameController>();
        services.AddTransient<IGameFactory, GameFactory>();
        services.AddTransient<IFieldFactory, FieldFactory>();
        return services;
    }
}

using GameOfLife;
using GameOfLife.Services.Config;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using StackExchange.Profiling;

MiniProfiler.StartNew();

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddGameOfLifeServices();
builder.Services.AddHostedService<Worker>();

using var host = builder.Build();
host.Run();

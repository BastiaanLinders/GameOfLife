﻿// See https://aka.ms/new-console-template for more information

using StackExchange.Profiling;

MiniProfiler.StartNew();

Console.WriteLine("Let's go!");

var cts = new CancellationTokenSource();
var game = new Game.Services.Game();
await game.Init();
game.Sprinkle(0.2);

Console.CancelKeyPress += (_, eventArgs) =>
{
    cts.Cancel();
    eventArgs.Cancel = true;
    game.Stop();
};

await game.Start();

Console.WriteLine("That's all folks!");

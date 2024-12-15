using System.Configuration;
using System.Data;
using System.Windows;
using GameOfLife.Services.Config;
using Microsoft.Extensions.DependencyInjection;

namespace GameOfLife.Ui;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : IDisposable, IAsyncDisposable
{
    private ServiceProvider _serviceProvider = null!;

    protected override void OnStartup(StartupEventArgs e)
    {
        var serviceCollection = new ServiceCollection();
        ConfigureServices(serviceCollection);
        _serviceProvider = serviceCollection.BuildServiceProvider();

        var mainWindow = _serviceProvider.GetRequiredService<MainWindow>();
        mainWindow.Show();
    }

    private static void ConfigureServices(IServiceCollection services)
    {
        services.AddGameOfLifeServices();

        services.AddSingleton<MainWindow>();
    }

    public void Dispose()
    {
        _serviceProvider.Dispose();
    }

    public async ValueTask DisposeAsync()
    {
        await _serviceProvider.DisposeAsync();
    }
}

using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using GameOfLife.Services.Abstractions;
using GameOfLife.Services.Mechanics;
using StackExchange.Profiling;

namespace GameOfLife.Ui;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow
{
    private const int RectSize = 10;

    private readonly IGameController _gameController;
    private readonly Dictionary<Square, (Rectangle Alive, Rectangle Background)> _fieldMap = new();

    public MainWindow(IGameController gameController)
    {
        _gameController = gameController;

        InitializeComponent();

        SetupGameController();
    }

    private void SetupGameController()
    {
        _gameController.OnInitialized += (_, field) => Dispatcher.Invoke(() => OnGameInitialized(field));
        _gameController.OnStarted += (_, _) => Dispatcher.Invoke(OnGameStarted);
        _gameController.OnAdvanced += (_, generation) => Dispatcher.Invoke(() => OnGameAdvanced(generation));
        _gameController.OnStopped += (_, _) => Dispatcher.Invoke(OnGameStopped);
        _gameController.OnGpsChanged += (_, gps) => Dispatcher.Invoke(() => OnGpsChanged(gps));
    }

    private void OnInitClick(object sender, RoutedEventArgs e)
    {
        var width = (int) GameCanvas.ActualWidth / RectSize;
        var height = (int) GameCanvas.ActualHeight / RectSize;

        _gameController.Initialize(new GameOfLifeOptions
        {
            FieldWidth = width,
            FieldHeight = height,
            Gps = 3
        });
    }

    private void OnSprinkleClick(object sender, RoutedEventArgs e)
    {
        _gameController.Sprinkle(25);
        UpdateGrid();
    }

    private void OnStartClick(object sender, RoutedEventArgs e)
    {
        _gameController.Start();
    }

    private void OnStopClick(object sender, RoutedEventArgs e)
    {
        _gameController.Stop();
    }

    private void OnGpsSliderChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
        var gps = Convert.ToInt32(e.NewValue);
        _gameController.ChangeGps(gps);
    }

    private void OnGameInitialized(IField field)
    {
        DrawGrid(field);
    }

    private void OnGameStarted()
    {
        StateControl.ViewModel.IsRunning = true;
    }

    private void OnGameAdvanced(int generation)
    {
        StateControl.ViewModel.Generation = generation;
        UpdateGrid();
    }

    private void OnGameStopped()
    {
        StateControl.ViewModel.IsRunning = false;
    }

    private void OnGpsChanged(int gps)
    {
        StateControl.ViewModel.Gps = gps;
    }

    private void DrawGrid(IField field)
    {
        using var _ = MiniProfiler.Current.Step("DrawGrid");

        GameCanvas.Children.Clear();
        _fieldMap.Clear();

        var grid = field.Grid;
        foreach (var row in grid)
        {
            foreach (var square in row)
            {
                var rBg = new Rectangle
                {
                    Width = RectSize,
                    Height = RectSize,
                    Stroke = new SolidColorBrush(Colors.DimGray),
                    Fill = new SolidColorBrush(Colors.Gray)
                };
                Canvas.SetLeft(rBg, square.Location.X * RectSize);
                Canvas.SetTop(rBg, square.Location.Y * RectSize);
                GameCanvas.Children.Add(rBg);

                var rAlive = new Rectangle
                {
                    Width = RectSize,
                    Height = RectSize,
                    Stroke = new SolidColorBrush(Colors.DarkOrange),
                    Fill = new SolidColorBrush(Colors.Orange),
                    Visibility = square.IsAlive ? Visibility.Visible : Visibility.Hidden
                };
                Canvas.SetLeft(rAlive, square.Location.X * RectSize);
                Canvas.SetTop(rAlive, square.Location.Y * RectSize);
                GameCanvas.Children.Add(rAlive);

                _fieldMap.Add(square, (Alive: rAlive, Background: rBg));
            }
        }
    }

    private void UpdateGrid()
    {
        using var _ = MiniProfiler.Current.Step("UpdateGrid");

        foreach (var (square, (rAlive, _)) in _fieldMap)
        {
            rAlive.Visibility = square.IsAlive ? Visibility.Visible : Visibility.Hidden;
        }

        StateControl.ViewModel.UpdateGridCount++;
    }
}

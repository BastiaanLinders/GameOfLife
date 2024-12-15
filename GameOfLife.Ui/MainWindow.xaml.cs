using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Accessibility;
using GameOfLife.Services.Abstractions;
using GameOfLife.Services.Mechanics;
using MahApps.Metro.Controls;

namespace GameOfLife.Ui;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow
{
    private readonly IGameController _gameController;
    private readonly Dictionary<Square, Rectangle> _fieldMap = new();

    public MainWindow(IGameController gameController)
    {
        _gameController = gameController;

        SetupGameController();
        InitializeComponent();
    }

    private void SetupGameController()
    {
        _gameController.OnInitialized += (_, field) => Dispatcher.Invoke(() => RedrawGrid(field));
        _gameController.OnAdvanced += (_, _) => Dispatcher.Invoke(UpdateGrid);
    }

    private void OnInitClick(object sender, RoutedEventArgs e)
    {
        var width = (int) GameCanvas.ActualWidth / 10;
        var height = (int) GameCanvas.ActualHeight / 10;

        _gameController.Initialize(new GameOfLifeOptions
        {
            FieldWidth = width,
            FieldHeight = height,
            Gps = 3
        });
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
    

    private bool _redrawing;

    private void RedrawGrid(IField field)
    {
        if (_redrawing) return;
        _redrawing = true;

        var random = new Random();
        const int squaresize = 10;

        GameCanvas.Children.Clear();
        _fieldMap.Clear();

        var grid = field.Grid;
        foreach (var row in grid)
        {
            foreach (var square in row)
            {
                // TODO: Temp
                square.IsAlive = random.Next(0, 100) > 80;

                var r = new Rectangle
                {
                    Width = squaresize,
                    Height = squaresize,
                    Stroke = new SolidColorBrush(Colors.Gray),
                    Fill = new SolidColorBrush(Colors.Orange),
                    Visibility = square.IsAlive ? Visibility.Visible : Visibility.Hidden
                };
                Canvas.SetLeft(r, square.Location.X * squaresize);
                Canvas.SetTop(r, square.Location.Y * squaresize);
                GameCanvas.Children.Add(r);
                _fieldMap.Add(square, r);
            }
        }

        _redrawing = false;
    }

    private void UpdateGrid()
    {
        foreach (var (square, rectangle) in _fieldMap)
        {
            rectangle.Visibility = square.IsAlive ? Visibility.Visible : Visibility.Hidden;
            //rectangle.Fill = new SolidColorBrush(square.IsAlive ? Colors.Orange : Colors.LightGray);
        }
    }
}

using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using Game.Services;
using StackExchange.Profiling;

namespace Game.UI;

/// <summary>
///     Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private readonly List<(Square Square, Rectangle Rectangle)> _fieldMap = new();
    private readonly Services.Game _game;

    public MainWindow()
    {
        InitializeComponent();
        _game = new Services.Game();
        _game.Init();

        DrawBoard();
        _game.OnTickEvent += OnTickEvent;
    }

    private void DrawBoard()
    {
        const int squaresize = 20;

        foreach (var s in _game.Field!.Squares)
        {
            var r = new Rectangle
                    {
                        Width = squaresize,
                        Height = squaresize,
                        Stroke = new SolidColorBrush(Colors.Gray),
                        Fill = new SolidColorBrush(s.IsAlive ? Colors.Orange : Colors.LightGray)
                    };

            Canvas.SetLeft(r, s.Location.X * squaresize);
            Canvas.SetTop(r, s.Location.Y * squaresize);
            Canvas.Children.Add(r);

            _fieldMap.Add((s, r));
        }

        Canvas.MouseDown += OnCanvasMouseDown;
    }

    private void OnCanvasMouseDown(object sender, MouseButtonEventArgs e)
    {
        if (e.Source is Rectangle canvasSquare)
        {
            var map = _fieldMap.SingleOrDefault(t => t.Rectangle == canvasSquare);
            if (map.Square is not null)
            {
                map.Square.IsAlive = !map.Square.IsAlive;
            }
        }
        
        Dispatcher.Invoke(RefreshBoard);
    }

    private void OnTickEvent()
    {
        Dispatcher.Invoke(RefreshBoard);
    }

    private void RefreshBoard()
    {
        _fieldMap.ForEach(map =>
        {
            //map.Rectangle.Visibility = map.Square.IsAlive ? Visibility.Visible : Visibility.Hidden;
            map.Rectangle.Fill = new SolidColorBrush(map.Square.IsAlive ? Colors.Orange : Colors.LightGray);
        });
    }

    private void OnStartClick(object sender, RoutedEventArgs e)
    {
        Task.Run(async () => await _game.Start());
    }

    private void OnStopClick(object sender, RoutedEventArgs e)
    {
        _game.Stop();
    }

    private void OnSprinkleClick(object sender, RoutedEventArgs e)
    {
        var probability = SliderSprinkleProbability.Value;
        _game.Sprinkle(probability / 100d);
        RefreshBoard();
    }

    private void OnClearClick(object sender, RoutedEventArgs e)
    {
        _game.Clear();
        RefreshBoard();
    }

    private void OnGpsSliderChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
        var gps = Convert.ToInt32(e.NewValue);
        _game?.UpdateGps(gps);
    }
}
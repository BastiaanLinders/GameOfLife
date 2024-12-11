using StackExchange.Profiling;

namespace GameOfLife.Services;

public class Field
{
    private readonly ParallelOptions _parallelOptions;

    public Square[][] Grid { get; }
    public IEnumerable<Square> Squares { get; }

    public Field(int width, int height, ParallelOptions parallelOptions)
    {
        _parallelOptions = parallelOptions;

        using var _ = MiniProfiler.Current.Step("Field Ctor");

        using (MiniProfiler.Current.Step("Create Grid"))
        {
            Grid = CreateGridSquares(width, height);
            Squares = Grid.SelectMany(row => row);
        }

        using (MiniProfiler.Current.Step("Create awareness"))
        {
            InitializeSquareAwareness();
        }
    }

    private static Square[][] CreateGridSquares(int width, int height)
    {
        var grid = new Square[height][];
        for (var y = 0; y < height; y++)
        {
            grid[y] = new Square[width];
            for (var x = 0; x < width; x++)
            {
                grid[y][x] = new Square
                {
                    Location = new Location { X = x, Y = y },
                    IsAlive = false
                };
            }
        }

        return grid;
    }

    private void InitializeSquareAwareness()
    {
        foreach (var square in Squares)
        {
            square.MakeAware(Grid);
        }
    }

    public void Advance()
    {
        using (MiniProfiler.Current.Step("Evaluate squares"))
        {
            Parallel.ForEach(Squares, _parallelOptions, s => s.Evaluate());
        }

        using (MiniProfiler.Current.Step("Advance squares"))
        {
            Parallel.ForEach(Squares, _parallelOptions, s => s.Advance());
        }
    }

    public void Clear()
    {
        Parallel.ForEach(Squares, _parallelOptions, s => s.Reset());
    }
}

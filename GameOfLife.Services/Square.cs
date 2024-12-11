namespace GameOfLife.Services;

public class Square
{
    private bool? _nextState;
    public required bool IsAlive { get; set; }
    public required Location Location { get; init; }

    private Square[] Neighbours { get; set; } = null!;

    public void MakeAware(Square[][] grid)
    {
        var x = Location.X;
        var y = Location.Y;

        List<Square?> surrounding =
        [
            GetSquare(grid, x - 1, y - 1), GetSquare(grid, x, y - 1), GetSquare(grid, x + 1, y - 1),
            GetSquare(grid, x - 1, y + 0), /*        self          */ GetSquare(grid, x + 1, y + 0),
            GetSquare(grid, x - 1, y + 1), GetSquare(grid, x, y + 1), GetSquare(grid, x + 1, y + 1)
        ];

        Neighbours = surrounding
            .FindAll(s => s is not null)
            .ConvertAll(s => s!)
            .ToArray();
    }

    // ReSharper disable once ParameterTypeCanBeEnumerable.Local : Performance
    private static Square? GetSquare(Square[][] grid, int x, int y)
    {
        return grid.ElementAtOrDefault(y)?.ElementAtOrDefault(x);
    }

    public void Evaluate(bool calledByNeighbour = false)
    {
        if (_nextState.HasValue)
        {
            return;
        }

        if (!IsAlive && !calledByNeighbour)
        {
            return;
        }

        var aliveNeighbours = 0;
        foreach (var neighbour in Neighbours)
        {
            if (neighbour.IsAlive)
            {
                aliveNeighbours++;
            }

            if (!calledByNeighbour)
            {
                neighbour.Evaluate(true);
            }
        }

        _nextState =
            (!IsAlive && aliveNeighbours == 3)
            ||
            (IsAlive && aliveNeighbours is 2 or 3);
    }

    public void Advance()
    {
        IsAlive = _nextState ?? false;
        _nextState = null;
    }

    public void Reset()
    {
        IsAlive = false;
        _nextState = null;
    }
}

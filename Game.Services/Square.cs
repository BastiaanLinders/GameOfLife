namespace Game.Services;

public class Square
{
    private bool? _nextState;
    public required bool IsAlive { get; set; }
    public required Location Location { get; set; }

    private Square[] Neighbours { get; set; } = null!;

    public void CreateAwareness(Square[][] squaresLookup)
    {
        List<Square?> surrounding =
        [
            squaresLookup.ElementAtOrDefault(Location.Y - 1)?.ElementAtOrDefault(Location.X - 1),
            squaresLookup.ElementAtOrDefault(Location.Y - 1)?.ElementAtOrDefault(Location.X),
            squaresLookup.ElementAtOrDefault(Location.Y - 1)?.ElementAtOrDefault(Location.X + 1),

            squaresLookup.ElementAtOrDefault(Location.Y)?.ElementAtOrDefault(Location.X - 1),
            squaresLookup.ElementAtOrDefault(Location.Y)?.ElementAtOrDefault(Location.X + 1),

            squaresLookup.ElementAtOrDefault(Location.Y + 1)?.ElementAtOrDefault(Location.X - 1),
            squaresLookup.ElementAtOrDefault(Location.Y + 1)?.ElementAtOrDefault(Location.X),
            squaresLookup.ElementAtOrDefault(Location.Y + 1)?.ElementAtOrDefault(Location.X + 1)
        ];

        Neighbours = surrounding
            .FindAll(s => s is not null)
            .ConvertAll(s => s!)
            .ToArray();
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

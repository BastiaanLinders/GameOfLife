namespace Game.Services;

public class Square
{
    private bool? _nextState;
    public required Location Location { get; init; }
    public required bool IsAlive { get; set; }

    private Square[] Neighbours { get; set; } = null!;

    public void CreateAwareness(Square[][] squaresLookup)
    {
        var above = squaresLookup.ElementAtOrDefault(Location.Y - 1);
        var equal = squaresLookup.ElementAtOrDefault(Location.Y);
        var below = squaresLookup.ElementAtOrDefault(Location.Y + 1);

        Neighbours = new[]
            {
                above?.ElementAtOrDefault(Location.X - 1), above?.ElementAtOrDefault(Location.X), above?.ElementAtOrDefault(Location.X + 1),
                equal?.ElementAtOrDefault(Location.X - 1), /*              self               */ equal?.ElementAtOrDefault(Location.X + 1),
                below?.ElementAtOrDefault(Location.X - 1), below?.ElementAtOrDefault(Location.X), below?.ElementAtOrDefault(Location.X + 1)
            }
            .Where(s => s is not null)
            .Select(s => s!)
            .ToArray();
    }

    public void Evaluate(bool calledByNeighbour = false)
    {
        if (_nextState.HasValue) return;
        if (!IsAlive && !calledByNeighbour) return;

        var aliveNeighbours = 0;
        foreach (var neighbour in Neighbours)
        {
            if (neighbour.IsAlive) aliveNeighbours++;
            if (!calledByNeighbour) neighbour.Evaluate(true);
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
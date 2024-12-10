using StackExchange.Profiling;

namespace Game.Services;

public class Field
{
    private readonly Square[][] _squareLookup;

    public Field(int width, int height)
    {
        using var _ = MiniProfiler.Current.Step("Field Constructor");

        _squareLookup = new Square[height][];
        for (var y = 0; y < height; y++)
        {
            _squareLookup[y] = new Square[width];
            for (var x = 0; x < width; x++)
                _squareLookup[y][x] = new Square
                                      {
                                          Location = new Location { X = x, Y = y },
                                          IsAlive = false
                                      };
        }

        Squares = _squareLookup.SelectMany(x => x).ToList();
    }

    public List<Square> Squares { get; }

    public void Init()
    {
        using var _ = MiniProfiler.Current.Step("Field Init");

        foreach (var square in Squares) square.CreateAwareness(_squareLookup);
    }

    public void Clear()
    {
        Squares.ForEach(s => { s.Reset(); });
    }
}
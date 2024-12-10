using StackExchange.Profiling;

namespace Game.Services;

public class Field
{
    private readonly Square[][] _squareLookup;

    public Field(int width, int height)
    {
        using var _ = MiniProfiler.Current.Step("Field Constructor");

        using (MiniProfiler.Current.Step("Create squares"))
        {
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
        }

        using (MiniProfiler.Current.Step("Create awareness"))
        {
            foreach (var square in Squares) square.CreateAwareness(_squareLookup);
        }
    }

    public IEnumerable<Square> Squares => _squareLookup.SelectMany(row => row);

    public void Clear()
    {
        foreach (var square in Squares) square.Reset();
    }
}
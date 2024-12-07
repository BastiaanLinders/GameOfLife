namespace Game.Services;

public class Field
{
    public Field(int width, int height)
    {
        Squares = Enumerable.Range(0, width).SelectMany(_ => Enumerable.Range(0, height), (x, y) =>
        {
            var location = new Location { X = x, Y = y };
            return new Square
                   {
                       Location = location,
                       IsAlive = false
                   };
        }).ToList();
    }

    public List<Square> Squares { get; }

    public void Clear()
    {
        Squares.ForEach(s => s.IsAlive = false);
    }
}
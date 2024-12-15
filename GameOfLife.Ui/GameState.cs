using System.Runtime.CompilerServices;

namespace GameOfLife.Ui;

public class GameState
{
    public bool IsRunning { get; set; }
    public int Generation { get; set; }
    public int UpdateGridCount { get; set; }
    public int Gps { get; set; }
}

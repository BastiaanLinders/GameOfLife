﻿namespace Game.Services;

public class Square
{
    private bool _nextState;
    public required Location Location { get; init; }
    private Square[] Neighbours { get; set; } = null!;

    public required bool IsAlive { get; set; }
    
    public void Reset()
    {
        IsAlive = false;
        _nextState = false;
    }

    public void MakeAware(Dictionary<int, Dictionary<int, Square>> squaresLookup)
    {
        var above = squaresLookup.GetValueOrDefault(Location.Y - 1);
        var equal = squaresLookup.GetValueOrDefault(Location.Y);
        var below = squaresLookup.GetValueOrDefault(Location.Y + 1);

        Neighbours = new[]
            {
                above?.GetValueOrDefault(Location.X - 1), above?.GetValueOrDefault(Location.X), above?.GetValueOrDefault(Location.X + 1),
                equal?.GetValueOrDefault(Location.X - 1), /*              self               */ equal?.GetValueOrDefault(Location.X + 1),
                below?.GetValueOrDefault(Location.X - 1), below?.GetValueOrDefault(Location.X), below?.GetValueOrDefault(Location.X + 1)
            }
            .Where(s => s is not null)
            .Select(s => s!)
            .ToArray();
    }

    public void Evaluate(bool fromNeighbour = false)
    {
        int aliveNeighbours = 0;
        // ReSharper disable once LoopCanBeConvertedToQuery : Performance
        foreach (var neighbour in Neighbours)
        {
            if (!fromNeighbour)
            {
                neighbour.Evaluate(true);
            }
            if (neighbour.IsAlive)
            {
                aliveNeighbours++;
            }
        }   

        _nextState =
            (!IsAlive && aliveNeighbours == 3)
            ||
            (IsAlive && aliveNeighbours is > 1 and < 4);
    }
    
    public void Evaluate2(bool fromNeighbour = false)
    {
        if (!IsAlive && !fromNeighbour)
        {
            return;
        }
        
        int aliveNeighbours = 0;
        // ReSharper disable once LoopCanBeConvertedToQuery : Performance
        foreach (var neighbour in Neighbours)
        {
            if (!fromNeighbour)
            {
                neighbour.Evaluate(true);
            }
            if (neighbour.IsAlive)
            {
                aliveNeighbours++;
            }
        }   

        _nextState =
            (!IsAlive && aliveNeighbours == 3)
            ||
            (IsAlive && aliveNeighbours is > 1 and < 4);
    }
    
    public void EvaluateOld()
    {
        int aliveNeighbours = 0;
        // ReSharper disable once LoopCanBeConvertedToQuery : Performance
        foreach (var neighbour in Neighbours)
        {
            if (neighbour.IsAlive)
            {
                aliveNeighbours++;
            }
        }   

        _nextState =
            (!IsAlive && aliveNeighbours == 3)
            ||
            (IsAlive && aliveNeighbours is > 1 and < 4);
    }

    public void Procede()
    {
        IsAlive = _nextState;
    }
}
using Godot;
using System.Collections.Generic;

public class CellGrid
{
    private int _max = 0;
    private bool _wrap = true;
    public bool Wrap => _wrap;
    public int GridWidth = 64;
    public int GridHeight = 64;

    public List<Cell> Cells { get; private set; }

    public CellGrid(int width, int height, bool wrap = true)
    {
        GridWidth = width;
        GridHeight = height;
        _wrap = wrap;
        Cells = new List<Cell>();
        _max = GridHeight > GridWidth ? GridHeight : GridWidth;
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Cells.Add(new Cell(x, y, _max));
            }
        }
        //  GD.Print("Added " + Cells.Count + " _m:" + _max);
        foreach (var c in Cells)
            c.RefreshNeighbors(this);
    }

    public Cell this[int x, int y]
    {
        get
        {
            if (IsInBounds(x, y))
                return Cells[x * _max + y];
            else return null;
        }
    }

    public Cell GetCellAt(Vector2 loc)
    {
        if (IsInBounds(loc))
            return this[(int)loc.x, (int)loc.y];
        else return null;
    }

    public bool IsInBounds(Vector2 location)
    {
        if ((int)location.x >= 0 && (int)location.x < GridWidth && (int)location.y >= 0 && (int)location.y < GridHeight)
            return true;
        else return false;
    }

    public bool IsInBounds(int x, int y)
    {
        if (x >= 0 && x < GridWidth && y >= 0 && y < GridHeight)
            return true;
        else return false;
    }
}
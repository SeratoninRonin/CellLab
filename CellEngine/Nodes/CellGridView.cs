using Godot;
using System;

public class CellGridView : TileMap
{

    CellGrid8 _grid;
    public CellGrid8 Grid => _grid;
    [Export]
    public int Width = 64;
    [Export]
    public int Height = 64;
    [Export]
    public bool Wrap = false;

    public Cell this[int x, int y]
    {
        get
        {
            return _grid[x, y];
        }
    }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        InitializeGrid();
    }

    public void InitializeGrid()
    {
        _grid = new CellGrid8(Width, Height, Wrap);
        GD.Print("Making grid: " + Width + "," + Height + "  wr:" + Wrap);
    }
}

using Godot;
using System.Collections.Generic;

public interface ICellGraph2D
{
    int Width { get; }
    int Height { get; }
    List<Cell> Cells { get; }

    void CreateGraph();

    bool IsInBounds(Vector2 location);

    Cell GetCellAt(int x, int y);
}
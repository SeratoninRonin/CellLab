using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Godot;

public interface ICellGraph2D
{
    int Width { get; }
    int Height { get; }
    List<Cell> Cells { get; }
    void CreateGraph();
    bool IsInBounds(Vector2 location);
    Cell GetCellAt(int x, int y);
    

}

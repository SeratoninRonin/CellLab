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
    //Dictionary<Vector2,Cell> Cells { get; }
    //List<Cell> Cells { get; }
    List<Cell> Cells { get; }
    void CreateGraph();
    bool IsInBounds(Vector2 location);
}

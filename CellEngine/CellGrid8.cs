using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Godot;


public class CellGrid8 : ICellGraph2D
{
    int _max = 0;
    public int Width { get; set; }
    public int Height { get; set; }

    bool _wrap = true;
    //public Dictionary<Vector2, Cell> Cells { get; private set; }
    //public Cell[] Cells { get; private set; }
    public List<Cell> Cells { get; private set; }

    public Cell this[int x, int y]
    {
        get
        {
            /*var loc = new Vector2(x, y);
            if (Cells.ContainsKey(loc))
                return Cells[loc];
            return null;*/
            //if (IsInBounds(x, y))
            //return Cells[x * Width + y];
            //else return null;
            // return Cells.ElementAt(x * Width + y);
            return Cells[x*_max + y];
        }
    }

    public Cell this[Vector2 vec]
    {
        get
        {
            //if (IsInBounds(vec))
            return Cells[(int)vec.x * _max + (int)vec.y];
            //else return null;
        }
    }
    public CellGrid8(int width, int height, bool wrap = false)
    {
        Width = width;
        Height = height;
        _wrap = wrap;
        _max = height < width ? height : width;
        Initialize();
        CreateGraph();
    }

    private void Initialize()
    {
        Cells = new List<Cell>();
        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                Cells.Add(new Cell(x, y, this));
                //Cells[x * Width + y] = new Cell(x, y, this);
            }
        }
    }

    public void CreateGraph()
    {
        for (int y = 0; y < Height; y++)
        {
            for (int x = 0; x < Width; x++)
            {
                var loc = new Vector2(x, y);
                var cell = this[loc];
                cell.Neighbors.Clear();
                if (IsInBounds(loc + Vector2.Up))
                    cell.Neighbors.Add(this[loc + Vector2.Up]);
                if (IsInBounds(loc + Vector2.Up + Vector2.Right))
                    cell.Neighbors.Add(this[loc + Vector2.Up + Vector2.Right]);
                if (IsInBounds((loc + Vector2.Right)))
                    cell.Neighbors.Add(this[loc + Vector2.Right]);
                if (IsInBounds(loc + Vector2.Right + Vector2.Down))
                    cell.Neighbors.Add(this[loc + Vector2.Right + Vector2.Down]);
                if (IsInBounds(loc + Vector2.Down))
                    cell.Neighbors.Add(this[loc + Vector2.Down]);
                if (IsInBounds(loc + Vector2.Down + Vector2.Left))
                    cell.Neighbors.Add(this[loc + Vector2.Down + Vector2.Left]);
                if (IsInBounds(loc + Vector2.Left))
                    cell.Neighbors.Add(this[loc + Vector2.Left]);
                if (IsInBounds(loc + Vector2.Left + Vector2.Up))
                    cell.Neighbors.Add(this[loc + Vector2.Left + Vector2.Up]);


            }
        }
        if (_wrap)
        {
            for (int w = 0; w < Width; w++)
            {
                var tp = new Vector2(w, 0);
                var bt = new Vector2(w, Height - 1);
                var cellTop = this[tp];
                var cellBottom = this[bt];

                var tl = tp + Vector2.Left;
                var tr = tp + Vector2.Right;
                var bl = bt + Vector2.Left;
                var br = bt + Vector2.Right;

                if (!cellTop.Neighbors.Contains(cellBottom))
                    cellTop.Neighbors.Add(cellBottom);
                if (!cellBottom.Neighbors.Contains(cellTop))
                    cellBottom.Neighbors.Add(cellTop);

                if (IsInBounds(tl))
                    if (!cellBottom.Neighbors.Contains(this[tl]))
                        cellBottom.Neighbors.Add(this[tl]);
                if (IsInBounds(tr))
                    if (!cellBottom.Neighbors.Contains(this[tr]))
                        cellBottom.Neighbors.Add(this[tr]);
                if (IsInBounds(bl))
                    if (!cellTop.Neighbors.Contains(this[bl]))
                        cellTop.Neighbors.Add(this[bl]);
                if (IsInBounds(br))
                    if (!cellTop.Neighbors.Contains(this[br]))
                        cellTop.Neighbors.Add(this[br]);

            }

            for (int h = 0; h < Height; h++)
            {
                var lf = new Vector2(0, h);
                var rt = new Vector2(Width - 1, h);
                var cellLeft = this[lf];
                var cellRight = this[rt];

                var tl = lf + Vector2.Up;
                var tr = rt + Vector2.Up;
                var bl = lf + Vector2.Down;
                var br = rt + Vector2.Down;

                if (!cellLeft.Neighbors.Contains(cellRight))
                    cellLeft.Neighbors.Add(cellRight);
                if (!cellRight.Neighbors.Contains(cellLeft))
                    cellRight.Neighbors.Add(cellLeft);

                if (IsInBounds(tl))
                    if (!cellRight.Neighbors.Contains(this[tl]))
                        cellRight.Neighbors.Add(this[tl]);
                if (IsInBounds(tr))
                    if (!cellLeft.Neighbors.Contains(this[tr]))
                        cellLeft.Neighbors.Add(this[tr]);
                if (IsInBounds(bl))
                    if (!cellRight.Neighbors.Contains(this[bl]))
                        cellRight.Neighbors.Add(this[bl]);
                if (IsInBounds(br))
                    if (!cellLeft.Neighbors.Contains(this[br]))
                        cellLeft.Neighbors.Add(this[br]);
            }
        }
    }
    public bool IsInBounds(Vector2 location)
    {
        if ((int)location.x >= 0 && (int)location.x < Width && (int)location.y >= 0 && (int)location.y < Height)
            return true;
        else return false;
    }

    public bool IsInBounds(int x, int y)
    {
        if (x >= 0 && x < Width && y >= 0 && y < Height)
            return true;
        else return false;
    }
}
using Godot;
using System;
using System.Collections.Generic;
using System.Linq;



public class WireworldScene : Node2D
{
    float _delay;
    TileMap _map;
    MoveableCamera _camera;
    Sprite _selector;
    CellGrid _cells;

    Dictionary<Vector2, float> changes;
    int _head, _tail, _wire;

    [Export]
    public float StepDelay = .1f;
    [Export]
    public int GridWidth = 64;
    [Export]
    public int GridHeight = 64;

    public override void _Ready()
    {
        _map = GetNode<TileMap>("Grid");
        _camera = GetNode<MoveableCamera>("MoveableCamera");
        _selector = GetNode<Sprite>("SelectorSprite");
        _cells = new CellGrid(GridWidth, GridHeight);
        changes = new Dictionary<Vector2, float>();
        _head = _map.TileSet.FindTileByName("Blue");
        _tail = _map.TileSet.FindTileByName("Red");
        _wire = _map.TileSet.FindTileByName("Yellow");
        _delay = StepDelay;
        _camera.GlobalPosition = (new Vector2(_cells.GridWidth, _cells.GridHeight) * _map.CellSize) / 2f;
        InitGrid();
        ApplyChanges();
    }

    public override void _Process(float delta)
    {
        _delay -= delta;
        if (_delay < 0)
        {
            var count = changes.Count;
            _delay = StepDelay;
            if (count > 0)
            {
                StepChanges();
                ApplyChanges();
            }
        }
    }

    public void InitGrid()
    {
        var active = _map.GetUsedCells();
        foreach (Vector2 a in active)
        {
            var c = _map.GetCellv(a);

            if (c == _head)
            {
                changes.Add(a, _head);
            }

            if (c == _tail)
            {
                changes.Add(a, _tail);
            }
            if (c == _wire)
            {
                changes.Add(a, _wire);
            }
        }
    }

    public void ApplyChanges()
    {

        foreach (var kvp in changes)
        {
            var cell = _cells.GetCellAt(kvp.Key);
            if (!cell.Traits.ContainsKey("WireState"))
                cell.Traits.Add("WireState", kvp.Value);
            else
                cell.Traits["WireState"] = kvp.Value;
            _map.SetCellv(kvp.Key, (int)kvp.Value);
        }
    }

    public void StepChanges()
    {
        var list = changes.Keys.ToList();
        changes.Clear();
        var closed = new List<Vector2>();

        foreach (Vector2 loc in list)
        {
            var cell = _cells.GetCellAt(loc);
            var idx = _map.GetCellv(loc);

            //if (closed.Contains(loc))
            //{
            //    continue;
            //}

            if (cell.Traits.ContainsKey("WireState"))
            {
                var state = (int)cell.Traits["WireState"];
                if (state == _head)
                {
                    if (!changes.ContainsKey(cell.Location))
                        changes.Add(cell.Location, _tail);
                    else changes[cell.Location] = _tail;
                }
                if (state == _tail)
                {
                    if (!changes.ContainsKey(cell.Location))
                        changes.Add(cell.Location, _wire);
                    else changes[cell.Location] = _wire;
                }
                if (state == _wire)
                {
                    var u = cell.GetNeighborsWithTrait("WireState", _head).Count;
                    if (u == 1 || u == 2)
                    {
                        if (!changes.ContainsKey(cell.Location))
                            changes.Add(cell.Location, _head);
                        else changes[cell.Location] = _head;
                    }
                }
                //closed.Add(loc);
                StepNeighbors(loc, closed);
            }
        }


    }
    private void StepNeighbors(Vector2 cellv, List<Vector2> closed)
    {
        var cell = _cells.GetCellAt(cellv);
        var neighbors = cell.GetNeighborsWithTrait("WireState");
        foreach (var n in neighbors)
        {
            //if (!closed.Contains(n.Location))
            //{
                closed.Add(n.Location);
                var state = (int)n.Traits["WireState"];
                if (state == _head)
                {
                    if (!changes.ContainsKey(n.Location))
                        changes.Add(n.Location, _tail);
                    else changes[n.Location] = _tail;
                }
                if (state == _tail)
                {
                    if (!changes.ContainsKey(n.Location))
                        changes.Add(n.Location, _wire);
                    else changes[n.Location] = _wire;
                }
                if (state == _wire)
                {
                    var u = n.GetNeighborsWithTrait("WireState", _head).Count;
                    if (u == 1 || u == 2)
                    {
                        if (!changes.ContainsKey(n.Location))
                            changes.Add(n.Location, _head);
                        else changes[n.Location] = _head;
                    }
                }
            //}
        }
    }
}

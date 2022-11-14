using Godot;
using System;
using System.Collections.Generic;
using System.Linq;



public class WireworldScene : Node2D
{
    bool _placing = false;
    int _selectedIndex = -1;
    bool _simPaused= false;
    float _delay;
    TileMap _map;
    MoveableCamera _camera;
    Sprite _selector;
    CellGrid _cells;
    ItemList _tileList;
    Button _playButton;

    Dictionary<Vector2, float> changes;
    int _head, _tail, _wire, _bdz;

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
        
        _head = _map.TileSet.FindTileByName("Blue");
        _tail = _map.TileSet.FindTileByName("Red");
        _wire = _map.TileSet.FindTileByName("Yellow");
        _tileList = GetNode<ItemList>("UI/Panel/VBox/TileList");
        _playButton = GetNode<Button>("UI/Panel/VBox/PlayButton");
        _delay = StepDelay;

        _tileList.AddIconItem(GD.Load<Texture>("res://Assets/square32Yellow.png"));
        _tileList.AddIconItem(GD.Load<Texture>("res://Assets/square32Red.png"));
        _tileList.AddIconItem(GD.Load<Texture>("res://Assets/square32Blue.png"));
        _tileList.AddIconItem(GD.Load<Texture>("res://Assets/square32b.png"));

        _cells = new CellGrid(GridWidth, GridHeight);
        changes = new Dictionary<Vector2, float>();
        _camera.GlobalPosition = (new Vector2(_cells.GridWidth, _cells.GridHeight) * _map.CellSize) / 2f;
        OnPlayButtonPressed();
        InitGrid();
        //StepChanges();
        ApplyChanges();
    }

    public override void _Process(float delta)
    {
        if (Input.IsActionJustReleased("Space"))
            OnPlayButtonPressed();

        var mpos = GetGlobalMousePosition();
        _selector.GlobalPosition = _map.MapToWorld(_map.WorldToMap(mpos)) + (_map.CellSize / 2);
        if (!_simPaused)
        {
            _delay -= delta;
            if (_delay < 0)
            {
                var count = changes.Count;
                _delay = StepDelay;
                if (count > 0)
                {
                    GD.Print("Stepping " + count + " changes");
                    StepChanges();
                    ApplyChanges();
                }
            }
        }
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        if(@event is InputEventMouseButton mb)
        {
            if(mb.ButtonIndex == (int)ButtonList.Left && mb.Pressed)  //left was just pressed
            {
                if(!(_selectedIndex<0) && _simPaused) //can only build when paused
                {
                    GD.Print("Gonna place!");
                    var loc = _map.WorldToMap(GetGlobalMousePosition());
                    if (_selectedIndex == 0) //yellow
                    {
                        if (!changes.ContainsKey(loc))
                            changes.Add(loc, _wire);
                        else
                            changes[loc] = _wire;
                        _placing = true;
                    }
                    if(_selectedIndex == 1)
                    {
                        if (!changes.ContainsKey(loc))
                            changes.Add(loc, _tail);
                        else
                            changes[loc] = _tail;
                        _placing = true;
                    }
                    if(_selectedIndex==2)
                    {
                        if (!changes.ContainsKey(loc))
                            changes.Add(loc, _head);
                        else
                            changes[loc] = _head;
                        _placing = true;
                    }
                    if(_selectedIndex==3)
                    {
                        if (!changes.ContainsKey(loc))
                            changes.Add(loc, _bdz);
                        else
                            changes[loc] = _bdz;
                        _placing = true;
                    }
                    ApplyChanges();
                }
            }
            else if(mb.ButtonIndex == (int)ButtonList.Left && !mb.Pressed)
            {
                _placing = false;
                ApplyChanges();
            }
        }
        if(@event is InputEventMouseMotion mm)
        {
            if (_placing)// && !_simPaused)
            {
                GD.Print("dragging");
                var loc = _map.WorldToMap(GetGlobalMousePosition());
                if (_selectedIndex == 0) //yellow
                {
                    if (!changes.ContainsKey(loc))
                        changes.Add(loc, _wire);
                    else
                        changes[loc] = _wire;
                }
                if (_selectedIndex == 1)
                {
                    if (!changes.ContainsKey(loc))
                        changes.Add(loc, _tail);
                    else
                        changes[loc] = _tail;
                }
                if (_selectedIndex == 2)
                {
                    if (!changes.ContainsKey(loc))
                        changes.Add(loc, _head);
                    else
                        changes[loc] = _head;
                }
                if (_selectedIndex == 3)
                {
                    if (!changes.ContainsKey(loc))
                        changes.Add(loc, _bdz);
                    else
                        changes[loc] = _bdz;
                }

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
            if (kvp.Value != _bdz)
            {
                if (!cell.Traits.ContainsKey("WireState"))
                    cell.Traits.Add("WireState", kvp.Value);
                else
                    cell.Traits["WireState"] = kvp.Value;
                _map.SetCellv(kvp.Key, (int)kvp.Value);
            }
            else
            {
                if (cell.Traits.ContainsKey("WireState"))
                    cell.Traits.Remove("WireState");
                _map.SetCellv(kvp.Key, -1);
            }
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
    
    public void OnStepSliderChanged(float value)
    {
        StepDelay = value;
        GD.Print("slider change to " + value);
    }

    public void OnClearButtonPressed()
    {
        _cells = new CellGrid(GridWidth, GridHeight);
        changes = new Dictionary<Vector2, float>();
        _map.Clear();
        // _camera.GlobalPosition = (new Vector2(_cells.GridWidth, _cells.GridHeight) * _map.CellSize) / 2f;
        _placing = false;
        InitGrid();
        ApplyChanges();
    }

    public void OnPlayButtonPressed()
    {
        _simPaused = !_simPaused;
        _placing = false;
        if (_simPaused)
            _playButton.Text = "Play";
        else
            _playButton.Text = "Pause";
    }

    public void OnQuitButtonPressed()
    {
        GetTree().ChangeScene("res://MainMenuScene.tscn");
    }

    public void OnStepButtonPressed()
    {
        _placing = false;
        if (!_simPaused)
            OnPlayButtonPressed();
        StepChanges();
        ApplyChanges();
    }

    public void OnTileSelected(int index)
    {
        _placing = false;
        GD.Print("Tile " + index + " Selected!");
        _selector.Texture=_tileList.GetItemIcon(index);
        _selectedIndex = index;
    }
}

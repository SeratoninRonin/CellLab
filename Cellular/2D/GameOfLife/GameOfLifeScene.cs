using Godot;
using System.Collections.Generic;

public class GameOfLifeScene : Node2D
{
    private int robidx;
    private GameOfLifeUI _ui;
    private TileMap _grid;

    //TileMap _overlay;
    private MoveableCamera _camera;

    private Sprite _selector;
    private bool _simPause = false;
    private int grididx;
    private float _delay;
    private bool[,] board;
    private List<Vector2> changes = new List<Vector2>();
    private bool _placing = false;
    private bool _destroying = false;

    [Export]
    public float StepDelay = .25f;

    [Export]
    public bool Wrap = true;

    [Export]
    public int GridWidth = 256;

    [Export]
    public int GridHeight = 256;

    private bool _gridVisible = true;

    [Export]
    public bool GridVisible
    {
        get { return _gridVisible; }
        set { _gridVisible = value; Update(); }
    }

    public bool IsPaused => _simPause;

    public override void _Ready()
    {
        _delay = StepDelay;
        _grid = GetNode<TileMap>("Grid");
        _camera = GetNode<MoveableCamera>("MoveableCamera");
        //_overlay = GetNode<TileMap>("Overlay");
        _selector = GetNode<Sprite>("SelectorSprite");
        _selector.Visible = GridVisible;
        _ui = GetNode<GameOfLifeUI>("UI/Menu");
        robidx = _grid.TileSet.FindTileByName("Rob");
        grididx = _grid.TileSet.FindTileByName("GridWhite");
        board = new bool[GridWidth, GridHeight];
        AddNoise();
        InitGrid();
        var pos = new Vector2();
        pos.x = GridWidth * _grid.CellSize.x;
        pos.y = GridHeight * _grid.CellSize.y;
        pos *= .5f;
        _camera.GlobalPosition = pos;
        TickAll();
        var t = new Tween();
        t.InterpolateProperty(_camera, "zoom", Vector2.One * 16f, Vector2.One, 7f);
        AddChild(t);
        t.Start();
    }

    public override void _Process(float delta)
    {
        var mpos = GetGlobalMousePosition();
        _selector.GlobalPosition = MapToWorld(WorldToMap(mpos)) + (_grid.CellSize / 2);

        if (!_simPause)
        {
            _delay -= delta;
            if (_delay <= 0)
            {
                _delay = StepDelay;
                TickChanges();
            }
        }
        Update();
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        //check start/end clicks
        if (@event is InputEventMouseButton mb)
        {
            //left release
            if (mb.ButtonIndex == (int)ButtonList.Left && !mb.Pressed)
            {
                var pos = GetGlobalMousePosition();
                var loc = _grid.WorldToMap(pos);
                //GD.Print("Click!  p:" + pos + "  l:" + loc);
                _placing = false;
                if (IsInBounds(loc))
                {
                    var x = (int)loc.x;
                    var y = (int)loc.y;
                    var alive = board[x, y];
                    if (!alive)
                    {
                        board[x, y] = true;
                        changes.Add(loc);
                        _grid.SetCell(x, y, robidx);
                    }
                }
            }
            //right release
            else if (mb.ButtonIndex == (int)ButtonList.Right && !mb.Pressed)
            {
                var loc = _grid.WorldToMap(GetGlobalMousePosition());
                _destroying = false;
                if (IsInBounds(loc))
                {
                    var x = (int)loc.x;
                    var y = (int)loc.y;
                    var alive = board[x, y];
                    if (alive)
                    {
                        board[x, y] = false;
                        changes.Add(loc);
                        _grid.SetCell(x, y, -1);
                    }
                }
            }
            //left press
            else if (mb.ButtonIndex == (int)ButtonList.Left && mb.Pressed)
            {
                if (!_simPause)
                    _ui.OnPlayButtonPressed();
                var loc = _grid.WorldToMap(GetGlobalMousePosition());
                _placing = true;
                if (IsInBounds(loc))
                {
                    var x = (int)loc.x;
                    var y = (int)loc.y;
                    var alive = board[x, y];
                    if (!alive)
                    {
                        board[x, y] = true;
                        changes.Add(loc);
                        _grid.SetCell(x, y, robidx);
                    }
                }
            }
            //right press
            else if (mb.ButtonIndex == (int)ButtonList.Right && mb.Pressed)
            {
                if (!_simPause)
                    _ui.OnPlayButtonPressed();
                var loc = _grid.WorldToMap(GetGlobalMousePosition());
                _destroying = true;
                if (IsInBounds(loc))
                {
                    var x = (int)loc.x;
                    var y = (int)loc.y;
                    var alive = board[x, y];
                    if (alive)
                    {
                        board[x, y] = false;
                        changes.Add(loc);
                        _grid.SetCell(x, y, -1);
                    }
                }
            }
        }
        //check dragging
        if (@event is InputEventMouseMotion mm)
        {
            if (!_placing && !_destroying)
                return;
            else
            {
                var loc = _grid.WorldToMap(GetGlobalMousePosition());
                var x = (int)loc.x;
                var y = (int)loc.y;
                bool alive = board[x, y];

                if (_placing)
                {
                    if (!alive)
                    {
                        board[x, y] = true;
                        changes.Add(loc);
                        _grid.SetCell(x, y, robidx);
                    }
                }
                else if (_destroying)
                {
                    if (alive)
                    {
                        board[x, y] = false;
                        changes.Add(loc);
                        _grid.SetCell(x, y, -1);
                    }
                }
            }
        }
    }

    public override void _Draw()
    {
        DrawRect(new Rect2(-15, -15, new Vector2(GridWidth, GridHeight) * _grid.CellSize + new Vector2(15, 15)), Colors.Black, false, 15);
        //draw the grid if needed
        if (GridVisible)
        {
            var factor = (int)_camera.Zoom.x;
            factor = (int)Mathf.Clamp(factor, 1f, 8f);
            if (factor == 0)
                factor = 1;
            for (int x = 0; x < GridWidth; x += factor)
            {
                var col = Colors.Black;
                DrawLine(MapToWorld(x, 0), MapToWorld(x, GridHeight - 1), col);
            }
            for (int y = 0; y < GridHeight; y += factor)
            {
                var col = Colors.Black;
                DrawLine(MapToWorld(0, y), MapToWorld(GridWidth - 1, y), col);
            }
        }
    }

    public void TickChanges()
    {
        //var list = changes.ToList();
        //changes.Clear();
        bool[,] closed = new bool[GridWidth, GridHeight];
        bool[,] newBoard = new bool[GridWidth, GridHeight];
        var list = new List<Vector2>();

        foreach (var loc in changes)
        {
            var x = (int)loc.x;
            var y = (int)loc.y;

            if (!closed[x, y])
            {
                bool state = board[x, y];
                bool alive = board[x, y];
                //check cell and neighbors, add to closed
                var count = CountNeighborAlives(x, y);

                if (alive)
                {
                    if (count < 2)
                        alive = false;
                    if (count == 2 || count == 3)
                        alive = true;
                    if (count > 3)
                        alive = false;
                }
                else
                {
                    if (count == 3)
                        alive = true;
                }

                newBoard[x, y] = alive;
                closed[x, y] = true;

                if (alive)
                {
                    _grid.SetCell(x, y, robidx);
                }
                else
                {
                    _grid.SetCell(x, y, -1);
                }

                if (alive || (!alive && state != alive))
                {
                    list.Add(new Vector2(x, y));
                }
            }
            TickNeighbors(x, y, newBoard, closed, list);
        }
        board = newBoard;
        changes = list;
    }

    private void TickNeighbors(int cellX, int cellY, bool[,] newBoard, bool[,] closed, List<Vector2> changeList)
    {
        bool edge = false;
        var end = GridWidth - 1;
        var bot = GridHeight - 1;
        if (cellX == 0 || cellY == 0 || cellX == end || cellY == bot)
            edge = true;

        for (int x = -1; x < 2; x++)
        {
            for (int y = -1; y < 2; y++)
            {
                if (x == 0 && y == 0)
                    continue;

                var w = cellX + x;
                var z = cellY + y;
                if (edge && Wrap)
                {
                    if (cellX == 0 && x == -1)
                    {
                        w = end;
                    }
                    else if (cellX == end && x == 1)
                    {
                        w = 0;
                    }

                    if (cellY == 0 && y == -1)
                    {
                        z = bot;
                    }
                    else if (cellY == bot && y == 1)
                    {
                        z = 0;
                    }
                }

                if (IsInBounds(w, z))
                {
                    if (!closed[w, z])
                    {
                        var alive = board[w, z];
                        var state = board[w, z];
                        var count = CountNeighborAlives(w, z);

                        if (alive)
                        {
                            if (count < 2)
                                alive = false;
                            if (count == 2 || count == 3)
                                alive = true;
                            if (count > 3)
                                alive = false;
                        }
                        else
                        {
                            if (count == 3)
                                alive = true;
                        }
                        newBoard[w, z] = alive;
                        closed[w, z] = true;
                        if (alive)
                        {
                            _grid.SetCell(w, z, robidx);
                        }
                        else
                        {
                            _grid.SetCell(w, z, -1);
                        }
                        if (alive || (!alive && state != alive))
                        {
                            changeList.Add(new Vector2(w, z));
                        }
                    }
                }
            }
        }
    }

    public void TickAll()
    {
        changes.Clear();
        bool[,] newBoard = new bool[GridWidth, GridHeight];

        for (int x = 0; x < GridWidth; x++)
        {
            for (int y = 0; y < GridHeight; y++)
            {
                newBoard[x, y] = false;
                bool state = board[x, y];
                bool alive = board[x, y];
                var count = CountNeighborAlives(x, y);

                if (alive)
                {
                    if (count < 2)
                        alive = false;
                    if (count == 2 || count == 3)
                        alive = true;
                    if (count > 3)
                        alive = false;
                }
                else
                {
                    if (count == 3)
                        alive = true;
                }
                newBoard[x, y] = alive;
                if (alive)
                {
                    _grid.SetCell(x, y, robidx);
                }
                else
                {
                    _grid.SetCell(x, y, -1);
                }
                if (alive || (!alive && state != alive))
                {
                    changes.Add(new Vector2(x, y));
                }
            }
        }
        board = newBoard;
    }

    private void InitGrid()
    {
        _grid.Clear();
        for (int x = 0; x < GridWidth; x++)
        {
            for (int y = 0; y < GridHeight; y++)
            {
                if (board[x, y])
                {
                    _grid.SetCell(x, y, robidx);
                }
                else
                {
                    _grid.SetCell(x, y, -1);
                }
            }
        }
    }

    internal int CountNeighborAlives(int cellX, int cellY)
    {
        var count = 0;
        bool edge = false;
        var end = GridWidth - 1;
        var bot = GridHeight - 1;
        if (cellX == 0 || cellY == 0 || cellX == end || cellY == bot)
            edge = true;

        for (int x = -1; x < 2; x++)
        {
            for (int y = -1; y < 2; y++)
            {
                if (x == 0 && y == 0)
                    continue;

                var w = cellX + x;
                var z = cellY + y;
                if (edge && Wrap)
                {
                    if (cellX == 0 && x == -1)
                    {
                        w = end;
                    }
                    else if (cellX == end && x == 1)
                    {
                        w = 0;
                    }

                    if (cellY == 0 && y == -1)
                    {
                        z = bot;
                    }
                    else if (cellY == bot && y == 1)
                    {
                        z = 0;
                    }
                }
                if (IsInBounds(w, z) && board[w, z])
                    count++;
            }
        }
        return count;
    }

    public void TogglePause()
    {
        _simPause = !_simPause;
    }

    public void ClearBoard()
    {
        board = new bool[GridWidth, GridHeight];
        changes.Clear();
        _grid.Clear();
    }

    public void AddNoise()
    {
        for (int x = 0; x < GridWidth; x++)
        {
            for (int y = 0; y < GridHeight; y++)
            {
                var alive = board[x, y];
                if (!alive)
                {
                    if (Randomizer.RollDie(30) >= 29)
                    {
                        board[x, y] = true;
                        changes.Add(new Vector2(x, y));
                        _grid.SetCell(x, y, robidx);
                    }
                }
            }
        }
    }

    public Vector2 WorldToMap(Vector2 coord)
    {
        if (coord < Vector2.Zero)
            return -Vector2.One;
        else
            return _grid.WorldToMap(coord);
    }

    public Vector2 MapToWorld(Vector2 coord)
    {
        if (coord < Vector2.Zero)
            return -Vector2.One;
        else
            return _grid.MapToWorld(coord);
    }

    public Vector2 MapToWorld(int x, int y)
    {
        return MapToWorld(new Vector2(x, y));
    }

    public Vector2 WorldToMap(int x, int y)
    {
        return MapToWorld(new Vector2(x, y));
    }

    public bool IsInBounds(int x, int y)
    {
        if (x >= 0 && x < GridWidth && y >= 0 && y < GridHeight)
            return true;
        else return false;
    }

    public bool IsInBounds(Vector2 loc)
    {
        return IsInBounds((int)loc.x, (int)loc.y);
    }
}
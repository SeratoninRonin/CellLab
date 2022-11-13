using Godot;
using System.Collections.Generic;

public class Terrain2D : Node2D
{
    [Export]
    public bool LineOn = true;
    [Export]
    public Color LineColor = Colors.White;
    [Export]
    public int LineWidth = 8;
    [Export]
    public int LineStep = 3;
    [Export]
    public bool GenerateColliders = true;
    [Export]
    public int NumHills = 2;
    [Export]
    public int Slice = 10;
    [Export]
    public int MaxHillHeight = 100;
    [Export]
    public int MinHillHeight = 10;
    [Export]
    public int StartX = -50;
    [Export(PropertyHint.Range,"0.0,1.0")]
    public float StartY = .96f;
    [Export]
    public Texture Tex;
    private Line2D line;
    private List<Vector2> terrain = new List<Vector2>();
    private int stepCounter = 0;
    public override void _Ready()
    {
        line = GetNode("Line2D") as Line2D;
        if (line == null)
        {
            LineOn = false;
        }
        else
        {
            line.DefaultColor = LineColor;
            line.Width = LineWidth;
        }
    }
    public void AddHills()
    {
        var screensize = GetViewport().GetVisibleRect().Size;
        var hillWidth = screensize.x / NumHills;
        var hillSlices = hillWidth / Slice;
        var start = terrain[terrain.Count - 1];
        var poly = new List<Vector2>();

        for (int i = 0; i < NumHills; i++)
        {
            var height = Randomizer.Between(MinHillHeight, MaxHillHeight);
            start.y -= height;
            for (int j = 0; j < hillSlices; j++)
            {
                var hillPoint = Vector2.Zero;
                hillPoint.x = start.x + j * Slice + hillWidth * i;
                hillPoint.y = start.y + height * Mathf.Cos(2 * Mathf.Pi / hillSlices * j);
                terrain.Add(hillPoint);
                poly.Add(hillPoint);
                if (LineOn)
                {
                    stepCounter++;
                    if (stepCounter >= LineStep || j == 0)
                    {
                        stepCounter = 0;
                        line.AddPoint(hillPoint);
                    }
                }
            }
            start.y += height;
        }
        var lastPoint = terrain[terrain.Count - 1];
        lastPoint.y = screensize.y * StartY;
        lastPoint.x += 32;
        terrain.RemoveAt(terrain.Count - 1);
        terrain.Add(lastPoint);

        poly.RemoveAt(poly.Count - 1);
        poly.Add(lastPoint);

        if (LineOn)
        {
            line.AddPoint(lastPoint);
        }

        poly.Add(new Vector2(terrain[terrain.Count - 1].x, screensize.y));
        poly.Add(new Vector2(start.x, screensize.y));

        if (GenerateColliders)
        {
            var body = new StaticBody2D();
            var shape = new CollisionPolygon2D();
            AddChild(body);
            body.AddChild(shape);
            shape.Polygon = poly.ToArray();
        }

        var texPolygon = new Polygon2D();
        texPolygon.Polygon = poly.ToArray();
        texPolygon.Texture = Tex;
        AddChild(texPolygon);
    }

    public void Clear()
    {
        var children = GetChildren();
        foreach (var c in children)
        {
            var n = (Node)c;
            if (n is Line2D)
            {
                continue;
            }
            else
            {
                n.QueueFree();
            }
        }
        terrain.Clear();
        if (LineOn)
        {
            line.ClearPoints();
        }
    }

    public void Generate(int screensWide)
    {
        Clear();
        terrain = new List<Vector2>();
        var screensize = GetViewport().GetVisibleRect().Size;

        /*if (GenerateForts)
        {
            var baseStart = ( Fort)GD.Load<PackedScene>("res://Source/Gameplay/Level/Fort.tscn").Instance();
            var baseSize = baseStart.GetNode<Sprite>("Body").Texture.GetSize();
            baseSize.y += baseStart.GetNode<Sprite>("Body/FortFront").Texture.GetSize().y;
            baseStart.GlobalPosition = new Vector2(0, screensize.y - baseSize.y / 2);
            //AddChild(baseStart);
            CallDeferred("add_child", baseStart);
            terrain.Add(new Vector2(baseSize.x - 1, StartY * screensize.y));
        }
        else
        {*/
            terrain.Add(new Vector2(StartX, StartY * screensize.y));
        //}

        for (int i = 0; i < screensWide; i++)
        {
            AddHills();
        }

        /*if (GenerateForts)
        {
            var baseEnd = (Fort)GD.Load<PackedScene>("res://Source/Gameplay/Level/Fort.tscn").Instance();
            var baseSize = baseEnd.GetNode<Sprite>("Body").Texture.GetSize();
            baseSize.y += baseEnd.GetNode<Sprite>("Body/FortFront").Texture.GetSize().y;
            var lastPoint = terrain[terrain.Count - 1];
            baseEnd.GlobalPosition = new Vector2(lastPoint.x + 1 + baseSize.x, screensize.y - baseSize.y / 2);
            baseEnd.IsEnd = true;
            baseEnd.Scale = new Vector2(-1, 1);
            //AddChild(baseEnd);
            CallDeferred("add_child", baseEnd);
        }*/

    }

}

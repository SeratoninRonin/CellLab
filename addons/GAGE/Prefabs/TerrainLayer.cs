using Godot;
using System.Collections.Generic;

/// <summary>
/// TerrainLayer adds a rolling terrain parallax background layer
/// </summary>
public class TerrainLayer : ParallaxLayer
{
    /// <summary>
    /// Draws a line on the top of the layer
    /// </summary>
    [Export]
    public bool LineOn = true;

    /// <summary>
    /// The color of the line
    /// </summary>
    [Export]
    public Color LineColor = Colors.White;

    /// <summary>
    /// Line width
    /// </summary>
    [Export]
    public int LineWidth = 8;

    /// <summary>
    /// How many screens wide to make the layer.
    /// </summary>
    [Export]
    public int ScreensWide = 1;

    /// <summary>
    /// The number of hills generated per layer
    /// </summary>
    [Export]
    public int NumHills = 2;

    /// <summary>
    /// How much to slice up the terrain.  Lower numbers are smoother.
    /// </summary>
    [Export]
    public int Slice = 10;

    /// <summary>
    /// The maximum hill height
    /// </summary>
    [Export]
    public int MaxHillHeight = 100;

    /// <summary>
    /// The minimum hill height
    /// </summary>
    [Export]
    public int MinHillHeight = 10;

    /// <summary>
    /// Where to start the hills as a % of the screen height
    /// </summary>
    [Export]
    public float StartY = .96f;

    /// <summary>
    /// The texture to use to fill the layer
    /// </summary>
    [Export]
    public Texture Tex;

    private List<Vector2> terrain = new List<Vector2>();
    private Vector2 screensize;
    private Line2D line;

    public override void _Ready()
    {
        line = GetNode("Line2D") as Line2D;
        if (line == null)
            LineOn = false;
        else
        {
            line.DefaultColor = LineColor;
            line.Width = LineWidth;
        }
    }

    public void AddHills()
    {
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
            }
            start.y += height;
        }
        var lastPoint = terrain[terrain.Count - 1];
        lastPoint.y = screensize.y * StartY;
        lastPoint.x += 64;
        terrain.Add(lastPoint);
        poly.Add(lastPoint);

        if (LineOn)
        {
            foreach (Vector2 p in poly)
                line.AddPoint(p);
        }

        poly.Add(new Vector2(terrain[terrain.Count - 1].x, screensize.y));
        poly.Add(new Vector2(start.x, screensize.y));

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
                continue;
            else
                n.QueueFree();
        }
        terrain.Clear();
        if (LineOn)
            line.ClearPoints();
    }

    public void Generate()
    {
        Clear();
        screensize = GetViewport().GetVisibleRect().Size;
        terrain = new List<Vector2>();

        terrain.Add(new Vector2(-50, StartY * screensize.y));
        for (int i = 0; i < ScreensWide; i++)
        {
            AddHills();
        }
    }
}
using Godot;
using System.Collections.Generic;
#if DEBUG
public enum DebugDrawItemType
{
    None = 0,
    String = 1,
    Vector = 2,
    Lines = 3,
    Box = 4,
    Polygon = 5,
    Circle = 6,
    Convex = 7
}

public class DebugDrawItem : Node
{
    [Export]
    public bool Enabled = true;
    [Export]
    public string Property = string.Empty;
    [Export]
    public DebugDrawItemType DrawType = DebugDrawItemType.String;
    [Export]
    public Vector2 Scale = Vector2.One;
    [Export]
    public Color Color = Colors.White;
    [Export]
    public Vector2 Size = Vector2.One;
    [Export]
    public Vector2 Offset = Vector2.Zero;
    [Export(PropertyHint.Range,"0,10,.1")]
    public float LineWidth = 1.0f;
    [Export]
    public int Sides = 1;
    [Export]
    public List<Vector2> Points = new List<Vector2>();


    public override void _Process(float delta)
    {
        if (Enabled)
        {
            Core.Immediate.Draw.DrawDebug(this);
        }
    }
}
#endif
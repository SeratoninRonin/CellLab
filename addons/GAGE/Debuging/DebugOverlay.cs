using Godot;
using System.Collections.Generic;
#if DEBUG
public class DebugOverlay : CanvasLayer
{
    public DebugDrawControl Draw;

    public List<DebugDrawItem> Vectors = new List<DebugDrawItem>();
    public override void _Ready()
    {

        Draw = GetNode<Control>("Draw") as DebugDrawControl;
    }


}
#endif
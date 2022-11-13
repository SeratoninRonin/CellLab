using Godot;
using System.Collections.Generic;
#if DEBUG
public class DebugDrawControl : Control
{
    private Font font;
    private List<DebugDrawItem> _toDraw = new List<DebugDrawItem>();
    
    public override void _Ready()
    {
        font = this.GetFont("font");
    }

    public override void _Draw()
    {
        if (!Visible || !Core.DebugRenderEnabled)
        {
            return;
        }

        foreach (var debugItem in _toDraw)
        {
            var parent = debugItem.GetParent();
            var target = debugItem.GetParent().Get(debugItem.Property);
            switch (debugItem.DrawType)
            {
                case DebugDrawItemType.String:
                    
                    Vector2 pos = Vector2.Zero;
                    if(parent is Node2D n2)
                    {
                        pos = GetViewportTransform() * n2.GlobalPosition;
                    }
                    else if (parent is Spatial sp)
                    {
                        var cam = GetViewport().GetCamera();
                        pos = cam.UnprojectPosition(sp.GlobalTransform.origin);
                    }
                    else
                    {
                        pos = Vector2.One;
                    }
                    var size = font.GetStringSize(target.ToString());
                    pos += debugItem.Offset;
                    pos -= size / 2;
                    DrawString(font, pos, target.ToString(), debugItem.Color);
                    break;
                case DebugDrawItemType.Vector:
                    
                    if (target is Vector2 v2)
                    {
                        if (debugItem.GetParent() is Node2D n)
                        {
                            var start = n.GetViewportTransform() * n.Position;
                            var end = (start + v2 * debugItem.Scale);
                            start += debugItem.Offset;
                            end += debugItem.Offset;
                            DrawLine(start, end, debugItem.Color, debugItem.LineWidth);
                            this.DrawTriangle(end, start.DirectionTo(end), debugItem.LineWidth * 2, debugItem.Color);
                        }
                    }
                    else if(target is Vector3 v3)
                    {
                        if(debugItem.GetParent() is Spatial s)
                        {
                            var cam =GetViewport().GetCamera();

                            var start = cam.UnprojectPosition(s.GlobalTransform.origin);
                            var end = cam.UnprojectPosition(s.GlobalTransform.origin + (Vector3)target);
                            start += debugItem.Offset;
                            end += debugItem.Offset;
                            end *= debugItem.Scale;
                            DrawLine(start, end, debugItem.Color, debugItem.LineWidth);
                            this.DrawTriangle(end, start.DirectionTo(end), debugItem.LineWidth * 2, debugItem.Color);
                        }
                    }
                    else //there is no property to track, just draw a vector for some reason
                    {
                        if (parent is Node2D node2d)
                        {
                            var start = node2d.GlobalPosition;
                            var end = (start + debugItem.Size * debugItem.Scale);
                            start += debugItem.Offset;
                            end += debugItem.Offset;
                            DrawLine(start, end, debugItem.Color, debugItem.LineWidth);
                            this.DrawTriangle(end, start.DirectionTo(end), debugItem.LineWidth * 2, debugItem.Color);
                        }
                        else if (parent is Spatial spat)
                        {
                            var cam = GetViewport().GetCamera();

                            var start = cam.UnprojectPosition(spat.GlobalTransform.origin);
                            var end = cam.UnprojectPosition(spat.GlobalTransform.origin + (Vector3)target);
                            start += debugItem.Offset;
                            end += debugItem.Offset;
                            end *= debugItem.Scale;
                            DrawLine(start, end, debugItem.Color, debugItem.LineWidth);
                            this.DrawTriangle(end, start.DirectionTo(end), debugItem.LineWidth * 2, debugItem.Color);
                        }
                    }
                    break;
                case DebugDrawItemType.Box:
                    var position = Vector2.Zero;
                    if(parent is Node2D n2d)
                    {

                    }
                    else if(parent is Spatial sp)
                    {

                    }
                    break;
                case DebugDrawItemType.Circle:
                    break;
                case DebugDrawItemType.Polygon:
                    break;
                case DebugDrawItemType.Lines:
                    break;
                case DebugDrawItemType.Convex:
                    break;
                default:
                    break;
            }
        }
        _toDraw.Clear();
        base._Draw();
    }

    internal void DrawDebug(DebugDrawItem debugDrawItem)
    {
        _toDraw.Add(debugDrawItem);
    }

    public override void _Process(float delta)
    {
        Update();
        base._Process(delta);
    }
}
#endif
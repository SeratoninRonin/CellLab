using System;
using Godot;

public static class Vector2Ext
{
    public static float ToroidalDistance(this Vector2 from, Vector2 to)
    {
        float dx = Mathf.Abs(to.x - from.x);
        float dy = Mathf.Abs(to.y - from.y);

        if (dx > .5f)
            dx = 1f - dx;

        if (dy > .5f)
            dy = 1f - dy;

        return Mathf.Sqrt(dx * dx + dy * dy);
    }
}
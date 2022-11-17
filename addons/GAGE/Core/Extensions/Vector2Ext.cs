using Godot;

public static class Vector2Ext
{
    public static float ToroidalDistance(this Vector2 from, Vector2 to, Vector2 size)
    {
        float dx = Mathf.Abs(to.x - from.x);
        float dy = Mathf.Abs(to.y - from.y);

        if (dx > size.x * .5f)
            dx = size.x - dx;

        if (dy > size.y * .5f)
            dy = size.y - dy;

        return Mathf.Sqrt(dx * dx + dy * dy);
    }

    public static float ToroidalDistanceSquared(this Vector2 from, Vector2 to, Vector2 size)
    {
        float dx = Mathf.Abs(to.x - from.x);
        float dy = Mathf.Abs(to.y - from.y);

        if (dx > size.x * .5f)
            dx = size.x - dx;

        if (dy > size.y * .5f)
            dy = size.y - dy;

        return dx * dx + dy * dy;
    }
}
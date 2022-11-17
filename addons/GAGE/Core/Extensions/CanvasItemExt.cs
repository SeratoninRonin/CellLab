using Godot;

public static class CanvasItemExt
{
    public static void DrawTriangle(this CanvasItem canvas, Vector2 pos, Vector2 dir, float size, Color color)
    {
        var a = pos + dir * size;
        var b = pos + dir.Rotated(MathHelper.TwoPi / 3) * size;
        var c = pos + dir.Rotated(2 * MathHelper.TwoPi / 3) * size;
        Vector2[] arr = new Vector2[3];
        arr[0] = a;
        arr[1] = b;
        arr[2] = c;
        Color[] colAr = new Color[3];
        colAr[0] = color;
        colAr[1] = color;
        colAr[2] = color;

        if (!(a == b && b == c))
        {
            canvas.DrawPolygon(arr, colAr);
        }
    }
}
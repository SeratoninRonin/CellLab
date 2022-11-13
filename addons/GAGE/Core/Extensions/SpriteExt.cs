using System;
using System.Collections.Generic;
using Godot;

public static class SpriteExt
{
    public static Vector2[] CreatePolygon2D(this Sprite sprite)
    {
        var bm = new Godot.BitMap();
        bm.CreateFromImageAlpha(sprite.Texture.GetData());
        var w = sprite.Texture.GetWidth();
        var h = sprite.Texture.GetHeight();
        //var rect = new Rect2(0,0,sprite.Texture.GetWidth(), sprite.Texture.GetHeight());
        var rect = new Rect2(0,0, w, h);
        var my_array = bm.OpaqueToPolygons(rect);
        if (my_array != null && my_array.Count > 0)
            return (Vector2[])my_array[0];
        else return null;
    }

    public static Godot.Collections.Array CreatePolygons(this Sprite sprite)
    {
        var array = new Godot.Collections.Array();
        var bm = new Godot.BitMap();
        bm.CreateFromImageAlpha(sprite.Texture.GetData());
        var x = sprite.GlobalPosition.x;
        var y = sprite.GlobalPosition.y;
        var rect = new Rect2(0, 0, sprite.Texture.GetWidth(), sprite.Texture.GetHeight());
        var my_array = bm.OpaqueToPolygons(rect);
        if (my_array != null && my_array.Count > 0)
        {
            for (int i = 0; i < my_array.Count; i++)
            {
                array.Add((Vector2[])my_array[0]);
            }
        }
        return array;
    }
}
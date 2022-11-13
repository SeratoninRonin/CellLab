using Godot;
using System;

public static class Camera2DExt
{
    public static Vector2 ScreenToWorld(this Camera2D cam, Vector2 screenCoordinates)
    {
        return screenCoordinates + cam.GetCameraScreenCenter() - cam.GetViewport().GetVisibleRect().Size / 2;
    }

    public static Vector2 WorldToScreen(this Camera2D cam, Vector2 worldCoordinates)
    {
        var result = -Vector2.One;
        result = worldCoordinates + cam.GetViewport().GetVisibleRect().Size / 2 - cam.GetCameraScreenCenter();
        return result;
    }
}

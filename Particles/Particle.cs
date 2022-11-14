using Godot;
using System;

public class Particle : Sprite, IScenePool
{
    int width, height;
    Vector2 texSize;
    public ForceColors ForceColor = ForceColors.White;
    public Vector2 Velocity = Vector2.Zero;

    public override void _Ready()
    {
        width = (int)GetViewport().Size.x;
        height = (int)GetViewport().Size.y;
        texSize = Texture.GetSize();
        GD.Print(width + "," + height);
    }

    public override void _Process(float delta)
    {
        if (Visible)
        {
            GlobalPosition += Velocity * delta;
            var pos = GlobalPosition;

            if (pos.x < 0-texSize.x)
                pos.x = width + pos.x + texSize.x;
            if (pos.x > width+texSize.x)
                pos.x = pos.x - width - texSize.x;
            if(pos.y< 0-texSize.y)    
                pos.y = height + pos.y + texSize.y;
            if (pos.y > height+texSize.y)
                pos.y = pos.y - height - texSize.y;

            GlobalPosition = pos;
        }
    }

    public void Reset()
    {
        Visible = false;
        Velocity= Vector2.Zero;
        Modulate = Colors.White;
        GlobalPosition = new Vector2(-float.MaxValue,-float.MaxValue);
    }

    public void ApplyForce(Vector2 dir, float force)
    {
        Velocity += dir * force;
    }
}

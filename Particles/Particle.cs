using Godot;
using System;

public class Particle : IQuadTreeStorable
{
    int width, height;
    public Vector2 TexSize;
    public Color ForceColor = Colors.White;
    public Vector2 Velocity = Vector2.Zero;
    public Vector2 Position = Vector2.Zero;
    private Rect2 _bounds;
    public Rect2 Bounds { get { return _bounds; } set { _bounds = value; } }

    public Particle(Vector2 position, Color forceColor, Vector2 velocity, Vector2 viewportSize, Vector2 spriteSize)
    {
        Position = position;
        ForceColor = forceColor;
        Velocity = velocity;
        width = (int)viewportSize.x;
        height = (int)viewportSize.y;
        TexSize = spriteSize;
    }

    public void Update(float delta)
    {
        Position.x = (width + Position.x + Velocity.x * delta) % width;
        Position.y = (height + Position.y + Velocity.y * delta) % height;
        _bounds.Position = Position;
        //    var pos = Position;

        //if (pos.x < 0 - TexSize.x)
        //    pos.x = width + pos.x;// + TexSize.x;
        //if (pos.x > width + TexSize.x)
        //    pos.x = pos.x - width;// - TexSize.x;
        //if (pos.y < 0 - TexSize.y)
        //    pos.y = height + pos.y;// + TexSize.y;
        //if (pos.y > height + TexSize.y)
        //    pos.y = pos.y - height;// - TexSize.y;

        //    Position = pos;
    }
        
    public void ApplyForce(Vector2 dir, float force)
    {
        Velocity += dir * force;
    }
}

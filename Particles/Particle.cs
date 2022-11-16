using Godot;
using System;

public class Particle : IQuadTreeStorable
{
    bool _wrap = false;
    int width, height;
    public Vector2 TexSize;
    public Color ForceColor = Colors.White;
    public Vector2 Velocity = Vector2.Zero;
    public Vector2 Position = Vector2.Zero;
    private Rect2 _bounds;
    public Rect2 Bounds { get { return _bounds; } set { _bounds = value; } }

    public Particle( Vector2 position, Color forceColor, Vector2 velocity, Vector2 worldSize, Vector2 spriteSize, bool toroidalSpace=false)
    {
        Position = position;
        ForceColor = forceColor;
        Velocity = velocity;
        width = (int)worldSize.x;
        height = (int)worldSize.y;
        TexSize = spriteSize;
        _wrap = toroidalSpace;
    }

    public void Update(float delta)
    {
        if (_wrap)
        {
            Position.x = (width + Position.x + Velocity.x * delta) % width;
            Position.y = (height + Position.y + Velocity.y * delta) % height;
            _bounds.Position = Position;
        }
        else
        {
            var newPos = Position + (Velocity * delta);
            if(newPos.x<0 || newPos.x>width || newPos.y<0 || newPos.y>height)
            {
                var norm = Vector2.Up;
                if (newPos.x < 0)
                    norm = Vector2.Right;
                else if (newPos.x > width)
                    norm = Vector2.Left;
                else if (newPos.y < 0)
                    norm = Vector2.Down;
                else if (newPos.y > height)
                    norm = Vector2.Up;
                Velocity = -Velocity.Reflect(norm);
                if (newPos.y < 0)
                    Position.y = 1;
                if (newPos.y > height)
                    Position.y = height - 2;
                if (newPos.x < 0)
                    Position.x = 1;
                if (newPos.x > width)
                    Position.x = width - 2;
            }
            Position += Velocity * delta;
            _bounds.Position = Position;

        }

    }
        
    public void ApplyForce(Vector2 dir, float force)
    {
        Velocity += dir * force;
    }
}

using Godot;

public class TouchJoystickButton : Node2D
{
    public Vector2 ButtonRadius = new Vector2(32, 32);
    public float JoystickRadius = 64;
    public float Acceleration = 20;
    public float Threshold = 8;

    public int OngoingDrag { get { return _ongoingDrag; } }
    protected int _ongoingDrag = -1;

    public override void _Ready()
    {
        Position = Vector2.Zero - ButtonRadius;
    }

    public override void _Process(float delta)
    {
        if (_ongoingDrag == -1)
        {
            var pos_diff = (Vector2.Zero - ButtonRadius) - Position;
            Position += pos_diff * Acceleration * delta;
        }
    }

    public override void _Input(InputEvent @event)
    {
        if (IsVisibleInTree())
        {
            Vector2 position = Vector2.Zero;

            int index = -2;
            if (@event is InputEventScreenDrag || @event is InputEventScreenTouch)
            {
                if (@event is InputEventScreenDrag sd)
                {
                    position = sd.Position;
                    index = sd.Index;
                }
                if (@event is InputEventScreenTouch stc)
                {
                    position = stc.Position;
                    index = stc.Index;
                }
            }
            else
            {
                return;
            }

            if (@event is InputEventScreenDrag || (@event is InputEventScreenTouch && @event.IsPressed()))
            {
                var eventDist = (position - GetParent<Node2D>().GlobalPosition).Length();
                if ((eventDist <= JoystickRadius * GlobalScale.x) || index == _ongoingDrag)
                {
                    GlobalPosition = position - ButtonRadius * GlobalScale;

                    if (GetButtonPosition().Length() > JoystickRadius)
                    {
                        Position = GetButtonPosition().Normalized() * JoystickRadius - ButtonRadius;
                    }

                    _ongoingDrag = index;
                }
            }
            if (@event is InputEventScreenTouch st && !@event.IsPressed() && _ongoingDrag == index)
            {
                _ongoingDrag = -1;
            }
        }
    }

    public Vector2 GetButtonPosition()
    {
        return Position + ButtonRadius;
    }

    public Vector2 GetDirection()
    {
        if (GetButtonPosition().Length() > Threshold)
        {
            return GetButtonPosition().Normalized();
        }
        else
        {
            return Vector2.Zero;
        }
    }

    public Vector2 GetValue()
    {
        if (GetButtonPosition().Length() > Threshold)
        {
            return GetButtonPosition() / JoystickRadius;
        }
        else
        {
            return Vector2.Zero;
        }
    }
}

using Godot;

public class TouchJoystick : Node2D
{
    private TouchJoystickButton button;

    public Vector2 ButtonDirection => button.GetDirection();
    public Vector2 ButtonValue => button.GetValue();
    public int OngoingGestureIndex => button.OngoingDrag;

    [Export]
    public Vector2 ButtonRadius = new Vector2(32, 32);
    [Export]
    public float JoystickRadius = 64;
    [Export]
    public float Acceleration = 20;
    [Export]
    public float Threshold = 8;


    public override void _Ready()
    {
        button = GetNode<TouchJoystickButton>("TouchJoystickButton");
        button.ButtonRadius = ButtonRadius;
        button.JoystickRadius = JoystickRadius;
        button.Acceleration = Acceleration;
        button.Threshold = Threshold;
    }
}

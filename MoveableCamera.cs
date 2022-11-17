using Godot;

public class MoveableCamera : Camera2D
{
    public Vector2 DesiredZoom = Vector2.One;

    [Export]
    public float CameraSpeed = 100f;

    [Export]
    public Vector2 ZoomMin = new Vector2(.5f, .5f);

    [Export]
    public Vector2 ZoomMax = new Vector2(3.5f, 3.5f);

    [Export]
    public Vector2 ZoomAmount = new Vector2(.1f, .1f);

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(float delta)
    {
        var lr = Input.GetAxis("CameraLeft", "CameraRight");
        var ud = Input.GetAxis("CameraUp", "CameraDown");
        var zm = Input.GetAxis("CameraZoomIn", "CameraZoomOut");
        if (zm != 0)
        {
            //GD.Print("Zoom");
            var zim = Zoom;
            zim += Vector2.One * zm * delta;
            if (zim.IsEqualApprox(ZoomMin) || zim < ZoomMin)
                zim = ZoomMin;
            if (zim.IsEqualApprox(ZoomMax) || zim > ZoomMax)
                zim = ZoomMax;
            Zoom = zim;
            DesiredZoom = zim;
        }

        if (Zoom != DesiredZoom)
        {
            Zoom = Zoom.LinearInterpolate(DesiredZoom, .1f);
        }

        var mov = new Vector2(lr, ud) * CameraSpeed;
        if (mov != Vector2.Zero)
        {
            GlobalPosition += mov * delta;
        }
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event is InputEventMouseButton)
        {
            Vector2 zm = Zoom;
            if (@event.IsActionPressed("CameraZoomIn"))
            {
                zm -= ZoomAmount;
                GD.Print(Zoom);
            }
            else if (@event.IsActionPressed("CameraZoomOut"))
            {
                zm += ZoomAmount;
                GD.Print(Zoom);
            }
            if (zm.IsEqualApprox(ZoomMin) || zm < ZoomMin)
            {
                zm = ZoomMin;
            }
            if (zm.IsEqualApprox(ZoomMax) || zm > ZoomMax)
            {
                zm = ZoomMax;
            }
            Zoom = zm;
            DesiredZoom = zm;
        }
        base._UnhandledInput(@event);
    }
}
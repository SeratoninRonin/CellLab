using Godot;

public class InputEventSwipe : InputEventScreenTouch
{
    public Vector2 Direction = Vector2.Zero;
}

public class SwipeDetector : Node
{
    [Signal]
    public delegate void Swiped(Vector2 direction);

    [Signal]
    public delegate void SwipeCanceled(Vector2 startPosition);

    [Export]
    public float MaxDiagonalSlope = 1.3f;

    private Timer timer;
    private Vector2 swipeStartPosition = new Vector2();

    public override void _Ready()
    {
        timer = GetNode<Timer>("SwipeTimeout");
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event is InputEventScreenTouch st)
        {
            if (st.IsPressed())
                StartDetection(st.Position);
            else if (!timer.IsStopped())
                EndDetection(st.Position);
        }
    }

    public void StartDetection(Vector2 position)
    {
        swipeStartPosition = position;
        timer.Start();
    }

    public void EndDetection(Vector2 position)
    {
        timer.Stop();
        Vector2 dir = (position - swipeStartPosition).Normalized();
        //check diagonal
        if (Mathf.Abs(dir.x) + Mathf.Abs(dir.y) >= MaxDiagonalSlope)
        {
            return;
        }

        //var swipe = new InputEventSwipe();

        if (Mathf.Abs(dir.x) > Mathf.Abs(dir.y))
        {
            //swipe.Direction = new Vector2(-Mathf.Sign(dir.x), 0f);
            EmitSignal(nameof(Swiped), new Vector2(Mathf.Sign(dir.x), 0f));
        }
        else
        {
            //swipe.Direction = new Vector2(0f, -Mathf.Sign(dir.y));
            EmitSignal(nameof(Swiped), new Vector2(0f, Mathf.Sign(dir.y)));
        }

        //Input.ParseInputEvent(swipe);
    }

    public void OnTimerTimeout()
    {
        EmitSignal(nameof(SwipeCanceled), swipeStartPosition);
    }
}
using Godot;

/// <summary>
/// CameraShaker adds shake to a Camera2D
/// Add as a child of the active camera
/// </summary>
public class CameraShaker : Node
{
    private const Tween.TransitionType TRANS = Tween.TransitionType.Sine;
    private const Tween.EaseType EASE = Tween.EaseType.InOut;

    private float amplitude = 0;
    private int priority = 0;

    private Tween shakeTween;
    private Timer frequencyTimer;
    private Timer durationTimer;
    private Camera2D camera;

    public override void _Ready()
    {
        shakeTween = GetNode<Tween>("ShakeTween");
        frequencyTimer = GetNode<Timer>("Frequency");
        durationTimer = GetNode<Timer>("Duration");
        camera = GetParent<Camera2D>();
        base._Ready();
    }

    public void Start(float duration = .2f, float frequency = .1f, float amplitude = 12, int priority = 0)
    {
        if (priority >= this.priority)
        {
            this.priority = priority;
            this.amplitude = amplitude;

            durationTimer.WaitTime = duration;
            frequencyTimer.WaitTime = frequency;
            durationTimer.Start();
            frequencyTimer.Start();

            NewShake();
        }
    }

    public void NewShake()
    {
        var rand = new Vector2();
        rand.x = Randomizer.Between(-amplitude, amplitude);
        rand.y = Randomizer.Between(-amplitude, amplitude);

        shakeTween.InterpolateProperty(camera, "offset", camera.Offset, rand, frequencyTimer.WaitTime, TRANS, EASE);
        shakeTween.Start();
    }

    public void Reset()
    {
        shakeTween.InterpolateProperty(camera, "offset", camera.Offset, Vector2.Zero, frequencyTimer.WaitTime, TRANS, EASE);
        shakeTween.Start();
        priority = 0;
    }

    public void OnFrequencyTimeout()
    {
        NewShake();
    }

    public void OnDurationTimeout()
    {
        Reset();
        frequencyTimer.Stop();
        durationTimer.Stop();
    }
}
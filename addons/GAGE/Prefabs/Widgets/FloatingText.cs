using Godot;
using System;

public class FloatingText : Position2D, IScenePool
{
    public Color Color = Colors.White;
    public Vector2 Velocity = Vector2.Zero;
    public Vector2 Gravity = Vector2.Down;
    public float Mass = 1;
    public Label Label;
    public Tween Tween;
    [Export]
    public Font Font;

    public override void _Ready()
    {
        this.Label = GetNode<Label>("Label");
        this.Tween = GetNode<Tween>("Tween");
        //TODO: ugh
        GlobalPosition = -Vector2.One;
        Visible = false;
    }

    public override void _Process(float delta)
    {
        if(Visible)
        {
            Velocity += Gravity * Mass * delta;
            Position += Velocity * delta;
        }
    }

    public void Start(Color color, Vector2 velocity, Vector2 gravity, float mass=1,float fadeoutTime = .3f,float fadeoutDelay=.7f,
        float inflateTime=.3f, float deflateTime = 1.0f, float deflateDelay=.6f)
    {
        Color = color;
        Velocity = velocity;
        Mass = mass;
        Gravity = gravity;
        Modulate = color;
        Scale = Vector2.Zero;
        
        //after .7 seconds, have the text fade out quickly
        Tween.InterpolateProperty(this, "modulate", color,
            new Color(color.r, color.g, color.b, 0f),
            fadeoutTime, Tween.TransitionType.Linear, Tween.EaseType.Out, fadeoutDelay);
        //Inflate quickly
        Tween.InterpolateProperty(this, "scale", Vector2.Zero, Vector2.One, inflateTime, 
            Tween.TransitionType.Quart, Tween.EaseType.Out);
        //after .6 seconds, shrink
        Tween.InterpolateProperty(this, "scale", Vector2.One, Vector2.Zero, deflateTime,
            Tween.TransitionType.Linear, Tween.EaseType.Out, deflateDelay);

        Tween.Start();
        Visible = true;
        
    }

    public void Start(Color color, Vector2 velocity, Vector2 gravityDir, float duration, float mass = 1)
    {

        var fadeoutTime = duration * .7f;
        var fadeoutDelay = duration * .3f;
        var inflateTime = duration * .3f;
        var deflateTime = duration * .7f;
        var deflateDelay = duration * .6f;

        Start(color, velocity, gravityDir, mass, fadeoutTime, fadeoutDelay, inflateTime, deflateTime, deflateDelay);
    }

    public void OnAllTweensCompleted()
    {
        QueueFree();
    }

    public void Reset()
    {
        Label.Text = string.Empty;
        GlobalPosition = -Vector2.One;
        Visible = false;
        Tween.StopAll();
    }
}

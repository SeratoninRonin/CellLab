using Godot;

public class TypewriterLabel : Label
{
    [Export]
    public float TotalSeconds = 1;

    private AnimationPlayer anim;
    public override void _Ready()
    {
        anim = GetNode<AnimationPlayer>("Anim");

        if (TotalSeconds > 0)
        {
            anim.PlaybackSpeed = 1f / TotalSeconds;
        }
        else
        {
            anim.PlaybackSpeed = 0;
        }
        Play();
    }

    public void ShowNow()
    {
        if (anim != null)
        {
            if(anim.HasAnimation("ShowNow"))
                anim.Play("ShowNow");
         
        }
        PercentVisible = 1f;
    }

    public void SetSpeed(float speed)
    {
        anim.PlaybackSpeed = speed;
    }

    public void Play()
    {
        anim.Play("Play");
    }

    public void Clear()
    {
        anim.Play("Clear");
    }
}

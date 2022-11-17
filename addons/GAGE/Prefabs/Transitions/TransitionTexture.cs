using Godot;

public class TransitionTexture : TextureRect
{
    [Signal]
    private delegate void Finished();

    [Export]
    public Texture Mask;

    [Export]
    public Texture Old;

    [Export]
    public float TransitionTime = 1.0f;

    [Export]
    public bool Invert = false;

    [Export]
    public bool Distort = false;

    [Export]
    public float Smoothing = 0f;

    private Tween tween;

    public override void _Ready()
    {
        tween = GetNode<Tween>("Tween");
        tween.Connect("tween_all_completed", this, "OnComplete");
        Reset();
        base._Ready();
    }

    public void Reset()
    {
        var mat = (ShaderMaterial)this.Material;
        mat.SetShaderParam("old_screen", Old);
        mat.SetShaderParam("mask", Mask);
        mat.SetShaderParam("invert", Invert);
        mat.SetShaderParam("distort", Distort);
        mat.SetShaderParam("smoothing", Smoothing);
        tween.InterpolateProperty(mat, "shader_param/delta", 0f, 1f, TransitionTime);
        tween.Start();
    }

    public override void _Process(float delta)
    {
        base._Process(delta);
    }

    public void OnComplete()
    {
        EmitSignal(nameof(Finished));
        QueueFree();
    }
}
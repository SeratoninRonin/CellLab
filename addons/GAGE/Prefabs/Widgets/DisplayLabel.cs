using Godot;
[Tool]
public class DisplayLabel : HBoxContainer
{
    [Export]
    public Texture LegendTexture;
    [Export]
    public string InitialValue;

    public TextureRect Legend;
    public Label Value;

    public override void _Ready()
    {
        Legend = GetNode<TextureRect>("Legend");
        Value = GetNode<Label>("Value");
        Legend.Texture = LegendTexture;
        Value.Text = InitialValue;
    }
}

using Godot;

public class MainMenuScene : Node2D
{
    public override void _Ready()
    {
    }

    public void OnWolfButtonPressed()
    {
        GetTree().ChangeScene("res://Cellular/1D/WolframScene.tscn");
    }

    public void OnLifeButtonPressed()
    {
        GetTree().ChangeScene("res://Cellular/2D/GameOfLife/GameOfLifeScene.tscn");
    }

    public void OnWireButtonPressed()
    {
        GetTree().ChangeScene("res://Cellular/2D/Wireworld/WireworldScene.tscn");
    }

    public void OnQuitButtonPressed()
    {
        GetTree().Quit();
    }
}
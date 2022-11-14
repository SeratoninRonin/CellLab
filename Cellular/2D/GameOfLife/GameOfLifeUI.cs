using Godot;
using System;

public class GameOfLifeUI : PanelContainer
{

    GameOfLifeScene _parent;
    Button _playButton;
    public override void _Ready()
    {
        _parent = GetParent().GetParent<GameOfLifeScene>();
        _playButton = GetNode<Button>("VBox/PlayButton");
        if (_parent.IsPaused)
            _playButton.Text = "Play!";
        else
            _playButton.Text = "Pause";
    }

    public override void _Process(float delta)
    {
        if (Input.IsActionJustReleased("Space"))
            OnPlayButtonPressed();
        if (Input.IsActionJustReleased("ToggleGrid"))
            _parent.GridVisible = !_parent.GridVisible;
    }

    public void OnStepButtonPressed()
    {
        _parent.TickAll();
    }

    public void OnQuitButtonPressed()
    {
        GetTree().ChangeScene("res://MainMenuScene.tscn");
    }

    public void OnResetButtonPressed()
    {
        GetTree().ReloadCurrentScene();
    }

    

    public void OnWrapPressed()
    {
        _parent.Wrap = !_parent.Wrap;
    }
    public void OnDelayValueChanged(float value)
    {
        _parent.StepDelay = value;
        GD.Print("step " + value);
    }
    public void OnPlayButtonPressed()
    {
        _parent.TogglePause();

        if (_parent.IsPaused)
            _playButton.Text = "Play!";
        else
            _playButton.Text = "Pause";

    }

    public void OnClearButtonPressed()
    {
        if (!_parent.IsPaused)
            OnPlayButtonPressed();
        _parent.ClearBoard();
    }

    public void OnNoiseButtonPressed()
    {
       // if (!_parent.IsPaused)
         //   OnPlayButtonPressed();
        _parent.AddNoise(); 
    }
}


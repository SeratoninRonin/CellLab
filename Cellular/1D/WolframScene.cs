using Godot;
using System.Collections;

public enum WolfInputMode
{
    Empty,
    Full,
    Center,
    Random
}

public class WolframScene : Node2D
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";

    // Called when the node enters the scene tree for the first time.

    private byte _rule;

    private BitArray _ruleset;
    private TileMap _grid;
    private Button _inputButton;
    private Button _wrapButton;
    private Label _genLabel;
    //Label _ruleLabel;

    private int width;
    private int height;
    private int center;
    private bool wrap = false;

    private int robidx = -1;
    private int gen = 1;

    private WolfInputMode mode = WolfInputMode.Center;

    public override void _Ready()
    {
        _rule = 0;
        _ruleset = new BitArray(new byte[] { _rule });
        _grid = GetNode<TileMap>("Grid");
        _inputButton = GetNode<Button>("Box/InputButton");
        _wrapButton = GetNode<Button>("Box/WrapButton");
        _genLabel = GetNode<Label>("Box/GenLabel");
        //_ruleLabel = GetNode<Label>("Split/RightBox/VBoxContainer/RuleLabel");
        width = (int)(GetViewport().Size.x / _grid.CellSize.x);
        height = (int)(GetViewport().Size.y / _grid.CellSize.y);
        center = width / 2;
        //GD.Print("grid size: " + width + "," + height + "  center: " + center);
        robidx = _grid.TileSet.FindTileByName("Rob");
        InitializeGrid();
    }

    public override void _Process(float delta)
    {
        _genLabel.Text = "Generation " + gen;
        //_ruleLabel.Text = "Rule " + Ruleset;
    }

    private void InitializeGrid()
    {
        _grid.Clear();
        gen = 1;
        switch (mode)
        {
            case WolfInputMode.Empty:
                break;

            case WolfInputMode.Full:
                for (int i = 0; i < width; i++)
                {
                    _grid.SetCell(i, 0, robidx);
                }
                break;

            case WolfInputMode.Center:
                _grid.SetCellv(new Vector2(center, 0), robidx);
                break;

            case WolfInputMode.Random:
                for (int i = 0; i < width; i++)
                {
                    if (Randomizer.BooleanCoinFlip())
                        _grid.SetCellv(new Vector2(i, 0), robidx);
                }
                break;
        }
    }

    public bool MapRule(bool left, bool cell, bool right)
    {
        if (!left && !cell && !right)
            return _ruleset[0];
        if (!left && !cell && right)
            return _ruleset[1];
        if (!left && cell && !right)
            return _ruleset[2];
        if (!left && cell && right)
            return _ruleset[3];
        if (left && !cell && !right)
            return _ruleset[4];
        if (left && !cell && right)
            return _ruleset[5];
        if (left && cell && !right)
            return _ruleset[6];
        if (left && cell && right)
            return _ruleset[7];
        else return false;
    }

    public bool[] PreviousLine()
    {
        bool[] prev = new bool[width];
        //get the previous line
        for (int w = 0; w < width; w++)
        {
            if (_grid.GetCell(w, gen - 1) < 0)
                prev[w] = false;
            else
                prev[w] = true;
        }
        return prev;
    }

    public bool[] NextLine(bool[] previous)
    {
        var result = new bool[width];
        for (int q = 1; q < width - 1; q++)
        {
            result[q] = MapRule(previous[q - 1], previous[q], previous[q + 1]);
        }
        if (wrap)
        {
            result[0] = MapRule(previous[width - 1], previous[0], previous[1]);
            result[width - 1] = MapRule(previous[width - 2], previous[width - 1], previous[0]);
        }
        else
        {
            result[0] = MapRule(false, previous[0], previous[1]);
            result[width - 1] = MapRule(previous[width - 2], previous[width - 1], false);
        }
        return result;
    }

    public void OnGenerateButtonPressed()
    {
        InitializeGrid();
        while (gen < height)
            OnStepButtonPressed();
    }

    public void OnInputButtonPressed()
    {
        if (mode == WolfInputMode.Random)
            mode = WolfInputMode.Empty;
        else
            mode++;

        _inputButton.Text = mode.ToString();

        InitializeGrid();
    }

    public void OnStepButtonPressed()
    {
        var line = NextLine(PreviousLine());
        for (int w = 0; w < width; w++)
        {
            if (line[w])
            {
                _grid.SetCell(w, (int)gen, robidx);
            }
            else
            {
                _grid.SetCell(w, (int)gen, -1);
            }
        }
        gen++;
    }

    public void OnRuleBoxChanged(float value)
    {
        _rule = (byte)value;
        _ruleset = new BitArray(new byte[] { _rule });
       // GD.Print("rule to " + value + " b:" + (byte)value + " i:" + (int)value);
       // GD.Print("ruleset: " + _rule);
        InitializeGrid();
    }

    public void OnWrapButtonPressed()
    {
        wrap = !wrap;
        _wrapButton.Text = "Wrap: " + wrap;
    }

    public void OnMainMenuButtonPressed()
    {
        GetTree().ChangeScene("res://MainMenuScene.tscn");
    }
}
using Godot;
using System.Collections.Generic;

public class TextScrollPopup : CenterContainer
{
    private bool finished = false;
    private bool skip = false;
    private TypewriterLabel current;
    protected VBoxContainer _vbox;
    protected Timer _scrollTimer;
    protected Timer _finishedTimer;
    protected string _headingText = string.Empty;
    protected List<string> _lines = new List<string>();

    [Export]
    public string ActionNext = "ui_accept";

    [Export]
    public DynamicFont HeadingFont = null;

    [Export]
    public DynamicFont ItemFont = null;

    [Export]
    public Color HeadingColor = Colors.White;

    [Export]
    public Color ItemColor = Colors.White;

    [Export]
    public PackedScene TypewriterEffect;

    [Export]
    public float LineTypeSpeed = 1f;

    [Export]
    public float LineScrollDelay = .7f;

    [Export]
    public float FinishDelay = 3f;

    [Signal]
    public delegate void Finished();

    public override void _Ready()
    {
        _vbox = GetNode<VBoxContainer>("VBox");
        _scrollTimer = GetNode<Timer>("ScrollTimer");
        _finishedTimer = GetNode<Timer>("FinishedTimer");
        current = null;
    }

    public override void _Process(float delta)
    {
        if (this.Visible)
        {
            if (Input.IsActionJustPressed(ActionNext))
            {
                skip = true;
                if (!finished)
                {
                    if (current != null)
                    {
                        if (!current.IsQueuedForDeletion())
                            current.ShowNow();
                    }

                    if (_scrollTimer.TimeLeft > 0)
                    {
                        _scrollTimer.WaitTime = LineScrollDelay / 10;
                        _scrollTimer.Start();
                    }
                }
                else
                {
                    if (_finishedTimer.WaitTime > FinishDelay / 2)
                    {
                        _finishedTimer.WaitTime = FinishDelay / 2;
                        _finishedTimer.Start();
                    }
                }
            }
        }
    }

    public void Clear()
    {
        foreach (var i in _vbox.GetChildren())
        {
            if (i is Node n)
            {
                n.QueueFree();
            }
        }
    }

    public void ResetTimers()
    {
        StopScrollTimer();
        StopFinishTimer();
    }

    public void ShowScroll(string heading, List<string> items, float lineTypeSpeed = 1f, float lineScrollDelay = .7f, float finishDelay = 2f)
    {
        finished = false;
        skip = false;
        LineTypeSpeed = lineTypeSpeed;
        LineScrollDelay = lineScrollDelay;
        FinishDelay = finishDelay;
        //set vars
        _lines = items;
        current = null;
        //clear any children
        Clear();
        //set the heading
        var label = new Label();
        label.Text = heading;
        label.Set("custom_fonts/font", HeadingFont);
        label.Set("custom_colors/font_color", HeadingColor);
        label.Align = Label.AlignEnum.Center;
        _vbox.AddChild(label);
        ResetTimers();
        //connect the timer
        if (!_scrollTimer.IsConnected("timeout", this, "ShowNextLine"))
        {
            _scrollTimer.Connect("timeout", this, "ShowNextLine");
        }
        //start
        _scrollTimer.Start();
        //go visible
        this.Visible = true;
    }

    public void ShowNextLine()
    {
        if (_lines == null || _lines.Count == 0)
        {
            Done();
        }
        else
        {
            var label = (TypewriterLabel)TypewriterEffect.Instance();
            label.Text = _lines[0];
            label.Set("custom_fonts/font", ItemFont);
            label.Set("custom_colors/font_color", ItemColor);
            label.Align = Label.AlignEnum.Center;

            if (!skip)
                label.TotalSeconds = LineTypeSpeed;
            else
            {
                label.TotalSeconds = LineTypeSpeed / 10;
                label.ShowNow();
            }
            _vbox.AddChild(label);
            current = label;
            _lines.RemoveAt(0);
        }
    }

    private void Done()
    {
        finished = true;
        StopScrollTimer();
        //Starts a pause
        _finishedTimer.Connect("timeout", this, "NextScreen");
        _finishedTimer.Start();
    }

    private void NextScreen()
    {
        if (_finishedTimer.IsConnected("timeout", this, "NextScreen"))
        {
            _finishedTimer.Disconnect("timeout", this, "NextScreen");
        }

        _finishedTimer.Stop();
        _finishedTimer.WaitTime = FinishDelay;
        EmitSignal(nameof(Finished));
    }

    private void StopScrollTimer()
    {
        _scrollTimer.Stop();
        _scrollTimer.WaitTime = LineScrollDelay;
        if (_scrollTimer.IsConnected("timeout", this, "ShowNextLine"))
        {
            _scrollTimer.Disconnect("timeout", this, "ShowNextLine");
        }
    }

    private void StopFinishTimer()
    {
        _finishedTimer.Stop();
        _finishedTimer.WaitTime = FinishDelay;
    }

    public void Hide(bool clear = true)
    {
        if (clear)
        {
            Clear();
        }

        //StopScrollTimer();
        ResetTimers();
        this.Visible = false;
    }
}
using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public class ParticleScene : Node2D
{
    Label _dragLabel;
    private static Particle _bob;
    bool _gridOn = true;
    Label fpsLabel;
    Font font;
    Vector2 _size;
    bool _wrap = false;
    private Vector2 _maxRangeSize;
    //public List<Force> Forces = new List<Force>();
    //public List<Particle> Particles = new List<Particle>();
    //public ChargedForce Force = new ChargedForce();
    public QuadTree<Particle> QuadTree;

    [Export]
    public float Drag = 0f;
    [Export]
    public Texture ParticleTexture;
    [Export]
    public Vector2 SimSize = new Vector2(4096, 4096);
    [Export]
    public int MaxForceRange = 128;
    [Export]
    public float Sigma = .00001f;

    public override void _Ready()
    {
        _maxRangeSize = new Vector2(MaxForceRange * 2, MaxForceRange * 2);
        fpsLabel = GetNode<Label>("UI/FPSLabel");
        _size = SimSize;
        QuadTree = new QuadTree<Particle>(new Rect2(0, 0, _size));
        font = GD.Load<Font>("res://Assets/Spaceport28.tres");
        _dragLabel = GetNode<Label>("%DragLabel");
        _dragLabel.Text = "Drag: " + Drag;
        var sld = GetNode<HSlider>("%DragSlider");
        sld.Value = Drag;
        //AddParticles(Randomizer.Between(16, 32));
        AddParticles(Randomizer.Between(64, 128), Colors.Yellow, 20f, 40f);
        AddParticles(Randomizer.Between(64, 128), Colors.Red, 10f, 30f);
        AddParticles(Randomizer.Between(64, 128), Colors.Purple, 30f, 50f);

        var pos = new Vector2(Randomizer.Between(0, _size.x), Randomizer.Between(0, _size.y));
        _bob = new Particle(pos, Colors.Yellow, Randomizer.RandomVec2() * Randomizer.Between(100, 300), _size, ParticleTexture.GetSize(), _wrap);
        QuadTree.Add(_bob);

        ForceCalculator.WorldSpace = SimSize;
        ForceCalculator.Rule(Colors.Yellow, Colors.Yellow, -150.50f, 64f);
        ForceCalculator.Rule(Colors.Yellow, Colors.Purple, 90f, 64f);
        ForceCalculator.Rule(Colors.Yellow, Colors.Red, -10f, 32f);
        ForceCalculator.Rule(Colors.Red, Colors.Red, 50f, 96f);
        ForceCalculator.Rule(Colors.Red, Colors.Yellow, 91.78f, 64f);
        ForceCalculator.Rule(Colors.Red, Colors.Purple, -1f, 16f);
        ForceCalculator.Rule(Colors.Purple, Colors.Red, -71.9f, 64f);
        ForceCalculator.Rule(Colors.Purple, Colors.Yellow, 69f, 64f);
        ForceCalculator.Rule(Colors.Purple, Colors.Purple, 120f, 64f);

    }

    private void AddParticles(int count, Color forceColor, float velMin, float velMax)
    {

        for (int i = 0; i < count; i++)
        {
            //var color = Randomizer.RandomColor(false);
            var pos = new Vector2(Randomizer.Between(0, _size.x), Randomizer.Between(0, _size.y));
            var p = new Particle(pos, forceColor, Randomizer.RandomVec2() * Randomizer.Between(velMin, velMax), _size, new Vector2(1,1), _wrap);
            QuadTree.Add(p);

        }
    }

    public override void _Process(float delta)
    {
        if (Input.IsActionJustReleased("ToggleGrid"))
            _gridOn = !_gridOn;
        //if (Core.DebugRenderEnabled)
        fpsLabel.Text = "FPS: " + Engine.GetFramesPerSecond() + "\nParticle Count:" + QuadTree.Count;

        ApplyForces();
        foreach (var p in QuadTree.ToList())
        {
            p.Velocity = p.Velocity * (1 - Drag);
            p.Update(delta);
            QuadTree.Move(p);
        }



    }

    public override void _PhysicsProcess(float delta)
    {
        Update();

    }

    public override void _Draw()
    {
        foreach (var p in QuadTree)
        {
            //DrawTexture(ParticleTexture, p.Position, p.ForceColor);
            //DrawLine(p.Position, p.Position + Vector2.One, p.ForceColor);
            DrawCircle(p.Position, 2, p.ForceColor);
        }


        if (_gridOn)
            DrawQuadHash();

    }

    internal void ApplyForces()
    {
        var list = QuadTree.GetAllObjects();
        foreach (var home in list)
        {
            //var matchrules = ForceCalculator.Matches[home.ForceColor];
            //foreach(var rule in matchrules)
            //{
            //    var dist = rule.ForceDetails.Distance;
            //    var center = home.Position;
            //    var uLeft = new Vector2(center.x - dist, center.y - dist);
            //    var neighbors = QuadTree.GetObjects(new Rect2(uLeft, new Vector2(dist * 2, dist * 2)));
            //    foreach(var n in neighbors)
            //    {
            //        if (home == n)
            //            continue;
            //        var match = new ColorMatch(home.ForceColor, n.ForceColor);
            //        var f = ForceCalculator.CalculateForce(home.ForceColor, n.ForceColor, n.Position.DistanceTo(home.Position));
            //        n.ApplyForce(n.Position.DirectionTo(home.Position), f);
            //    }
            //}


            var center = home.Position;
            if (ForceCalculator.Ranges.ContainsKey(home.ForceColor))
            {
                var maxr = ForceCalculator.Ranges[home.ForceColor];
                var uLeft = new Vector2(center.x - maxr, center.y - maxr);
                var neighbors = QuadTree.GetObjects(new Rect2(uLeft, new Vector2(maxr * 2, maxr * 2)));
                foreach (var n in neighbors)
                {

                    if (n == home)
                        continue;
                    var dis = n.Position.DistanceTo(home.Position);
                    var f = ForceCalculator.CalculateForce(home.ForceColor, n.ForceColor, dis);

                    if (dis<ForceCalculator.SafeDistance)
                    {
                        //n.Velocity *= .5f;
                        //                    var b = Randomizer.BooleanCoinFlip() ? 1 : -1;
                        //n.Velocity = n.Velocity.Reflect(n.Velocity.Rotated(Mathf.Deg2Rad(90*b)).Normalized());
                        //n.Velocity = Vector2.Zero;
                        //n.Velocity = -n.Velocity.Normalized() * ForceCalculator.SafeDistance;
                        //n.Velocity = Vector2.Zero;
                        // n.Velocity = -n.Velocity;
                       // n.Velocity = Vector2.Zero;
                       n.Velocity=Randomizer.RandomVec2()*ForceCalculator.SafeDistance;
                        
                    }
                    if (Mathf.Abs(f) >= Sigma)
                        n.ApplyForce(n.Position.DirectionTo(home.Position), f);
                }
            }
        }
    }

    private void DrawQuadHash()
    {
        Vector2 cellsize = new Vector2(128, 128);
        for (int x = 0; x < _size.x; x += (int)cellsize.x)
        {
            for (int y = 0; y < _size.y; y += (int)cellsize.y)
            {
                var rect = new Rect2(x, y, cellsize);
                var count = QuadTree.GetObjects(rect).Count;
                DrawRect(rect, Colors.Purple, false);
                DrawString(font, rect.GetCenter(), count.ToString(), Colors.Purple);
            }
        }
    }

    public void OnAddParticlesButtonPressed()
    {
        for (int i = 0; i < Randomizer.Between(16, 32); i++)
        {
            var r = Randomizer.RollDie(3);
            if (r == 1)
                AddParticles(1, Colors.Yellow, 100f, 300f);
            if (r == 2)
                AddParticles(1, Colors.Red, 100f, 300f);
            if (r == 3)
                AddParticles(1, Colors.Purple, 100f, 300f);
        }
    }

    public void ToggleGrid()
    {
        _gridOn = !_gridOn;
    }

    [Command("bob", "echos bob's position")]
    public static void Bob()
    {
        //GD.Print(_bob.Position);
        DebugConsole.Log(_bob.Position.ToString());
    }

    public void OnDragSliderChanged(float value)
    {
        Drag = value;
        _dragLabel.Text = "Drag: " + Drag;
    }
}

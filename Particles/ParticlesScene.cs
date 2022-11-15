using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public class ParticlesScene : Node2D
{
    Label fpsLabel;
    Font font;
    Vector2 _size;
    bool _wrap = true;
    public List<Force> Forces = new List<Force>();
    //public List<Particle> Particles = new List<Particle>();
    public QuadTree<Particle> QuadTree;

    [Export]
    public float Drag = 0f;
    [Export]
    public Texture ParticleTexture;

    public override void _Ready()
    {
        fpsLabel = GetNode<Label>("UI/FPSLabel");
        _size = GetViewport().Size;
        QuadTree = new QuadTree<Particle>(new Rect2(0, 0, _size));
        font = GD.Load<Font>("res://Assets/Spaceport28.tres");
        var grav = new Gravity(979.0f, GetViewport().Size);
        grav.ForceDistance = 512f;
        Forces.Add(grav);
        AddParticles(64, ForceColors.White);
    }

    private void AddParticles(int count, ForceColors fColor)
    {

        var size = GetViewport().Size;
        for (int i = 0; i < count; i++)
        {
            var color = Randomizer.RandomColor(false);
            var pos = new Vector2(Randomizer.Between(0, size.x), Randomizer.Between(0, size.y));
            var p = new Particle(pos, color, Randomizer.RandomVec2() * Randomizer.Between(10, 20), size, ParticleTexture.GetSize());
            QuadTree.Add(p);

        }
    }

    public override void _Process(float delta)
    {
        //if (Core.DebugRenderEnabled)
            fpsLabel.Text = "FPS: " + Engine.GetFramesPerSecond();
        foreach (var force in Forces)
        {
            if (force.IsUniversal)
            {
                var Particles = QuadTree.ToList();

                for (int f = 0; f < Particles.Count; f++)
                {
                    var from = Particles[f];

                    for (int t = f + 1; t < Particles.Count; t++)
                    {

                        var to = Particles[t];
                        var dist = Mathf.Min(to.Position.DistanceSquaredTo(from.Position), to.Position.ToroidalDistanceSquared(from.Position, _size));
                        if (from == to)
                            continue;
                        //if (dist > force.ForceDistance * force.ForceDistance)
                        //    continue;
                        var force12 = force.CalculateForce(from, to);
                        var dir12 = to.Position.DirectionTo(from.Position);
                        if (_wrap)
                        {
                            if (from.Position.ToroidalDistanceSquared(to.Position, _size) < from.Position.DistanceSquaredTo(to.Position))
                            {
                                dir12 = -dir12;
                            }
                        }

                        to.ApplyForce(dir12, force12);
                        from.ApplyForce(-dir12, force12);
                    }
                    from.Velocity -= from.Velocity * Drag * delta;
                    from.Update(delta);
                    QuadTree.Move(from);
                }

            }
        }

        Update();
    }

    public override void _Draw()
    {
        foreach (var p in QuadTree)
        {
            DrawTexture(ParticleTexture, p.Position,p.ForceColor);//Randomizer.RandomColor(true));
        }
        //DrawLine(new Vector2(_size.x * .5f, 0), new Vector2(_size.x*.5f, _size.y), Colors.Purple);
        //DrawLine(new Vector2(0,_size.y*.5f), new Vector2(_size.x,_size.y *.5f), Colors.Purple);
        ////DrawString()
        //var ul = new Rect2(Vector2.Zero, _size * .5f);
        //var ur = new Rect2(new Vector2(_size.x * .5f, 0), _size * .5f);
        //var br = new Rect2(_size * .5f, _size * .5f);
        //var bl = new Rect2(new Vector2(0, _size.y * .5f), _size * .5f);

        //var ulCount = QuadTree.GetObjects(ul).Count;
        //var urCount = QuadTree.GetObjects(ur).Count;
        //var brCount = QuadTree.GetObjects(br).Count;    
        //var blCount = QuadTree.GetObjects(bl).Count;

        //DrawString(font, ul.GetCenter(), ulCount.ToString(), Colors.Purple);
        //DrawString(font,ur.GetCenter(), urCount.ToString(), Colors.Purple);
        //DrawString(font, br.GetCenter(), brCount.ToString(), Colors.Purple);
        //DrawString(font, bl.GetCenter(), blCount.ToString(), Colors.Purple);

        //if (Core.DebugRenderEnabled)
            //DrawQuadHash();

    }

    private void DrawQuadHash()
    {
        Vector2 cellsize = new Vector2(64, 64);
        for (int x = 0; x < _size.x; x += 64)
        {
            for (int y = 0; y < _size.y; y += 64)
            {
                var rect = new Rect2(x, y, cellsize);
                var count = QuadTree.GetObjects(rect).Count;
                DrawRect(rect, Colors.Purple, false);
                DrawString(font, rect.GetCenter(), count.ToString(), Colors.Purple);
            }
        }
    }

    
}

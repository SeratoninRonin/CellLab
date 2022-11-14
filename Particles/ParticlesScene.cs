using Godot;
using System;
using System.Collections.Generic;

public class ParticlesScene : Node2D
{
    bool _wrap = true;
    bool _drag = true;
    ScenePool particlePool;
    public List<Force> Forces = new List<Force>();
    public List<Particle> Particles = new List<Particle>();

    public override void _Ready()
    {
        var grav = new Gravity(1000);
        Forces.Add(grav);
        particlePool = GetNode<ScenePool>("ParticlePool");
        AddParticles(100,ForceColors.White);
    }

    private void AddParticles(int count, ForceColors fColor)
    {
        for (int i = 0; i < count; i++)
        {
            var p = (Particle)particlePool.Obtain();
            p.ForceColor = fColor;
            p.Visible = true;
            var sz = GetViewport().Size;
            //var x = Mathf.Sqrt(Randomizer.Between(0, sz.x * sz.x));
            //var y = Mathf.Sqrt(Randomizer.Between(0, sz.y * sz.y));
            var x = Randomizer.Between(0, sz.x);
            var y = Randomizer.Between(0, sz.y);
            p.GlobalPosition = new Vector2(x, y);
            Particles.Add(p);
        }
    }

    public override void _Process(float delta)
    {
        foreach (var force in Forces)
        {
            
            if (force.IsUniversal)
            {
                foreach (var from in Particles)
                {
                    foreach(var to in Particles)
                    {
                        if (from == to)
                            continue;
                        else
                        {
                            var fpos = from.GlobalPosition;
                            var tpos = to.GlobalPosition;
                            var f = force.CalculateForce(from, to, _wrap);
                            var dir = fpos.DirectionTo(tpos);
                            if(_wrap)
                            {
                                if (fpos.ToroidalDistanceSquared(tpos) < fpos.DistanceSquaredTo(tpos))
                                    dir = -dir;
                            }
                            to.ApplyForce(dir, f);
                        }
                    }
                }
            }
        }

        if(_drag)
        {
            foreach(Particle p in Particles)
            {
                p.Velocity *= .99f;
            }
        }
    }
}

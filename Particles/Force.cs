using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Godot;

public enum ForceColors
{
    White,
    Red,
    Orange,
    Yellow,
    Green,
    Blue,
    Purple,
    Cyan,
    Magenta,
    Brown,
    Black
}

public abstract class Force
{
    public bool IsUniversal = false;
    public List<ForceColors> ForceColors = new List<ForceColors>();
    public abstract float CalculateForce(Particle from, Particle on, bool toroidialDistance =true);
}

public class Gravity : Force
{
    public float G { get; private set; }

    public Gravity(float g)
    {
        IsUniversal = true;
        G = g;
    }

    public override float CalculateForce(Particle from, Particle on, bool toroidialDistance=true)
    {
        float dist2 = 0;
        if (toroidialDistance)
            dist2 = from.GlobalPosition.ToroidalDistanceSquared(on.GlobalPosition);
        else
            dist2 = from.GlobalPosition.DistanceSquaredTo(on.GlobalPosition);
        if (dist2 < 1)
            return -1;
        return -G * (1 / dist2);
    }
}


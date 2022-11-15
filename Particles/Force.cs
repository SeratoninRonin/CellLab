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
    public float ForceDistance = 256f;
    public bool IsUniversal = false;
    public List<ForceColors> ForceColors = new List<ForceColors>();
    public abstract float CalculateForce(Particle from, Particle on, bool toroidialDistance =true);
}

public class Gravity : Force
{
    public float G { get; private set; }
    Vector2 _size;

    public Gravity(float g, Vector2 screenSize)
    {
        IsUniversal = true;
        G = g;
        _size = screenSize;
    }

    public override float CalculateForce(Particle from, Particle on, bool toroidialDistance=true)
    {
        float dist2 = 0;
        if (toroidialDistance)
        {
            var dist = from.Position.DistanceSquaredTo(on.Position);
            var tDist = from.Position.ToroidalDistanceSquared(on.Position, _size);
            if (tDist < dist)
                dist2 = tDist;
            else
                dist2 = dist;
        }
        else
            dist2 = from.Position.DistanceSquaredTo(on.Position);
        // if (dist2 < 16)
        //  return -1;
        if (dist2 < from.TexSize.x * from.TexSize.y)
            return -(G / Mathf.Abs(G));
            //return 0;
        return G * (1 / dist2);
    }
}


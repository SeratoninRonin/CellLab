using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Godot;

public struct ColorMatch : IEquatable<ColorMatch>
{
    public Color From;
    public Color To;

    public ColorMatch(Color from, Color to)
    {
        From = from;
        To = to;
    }

    public bool Equals(ColorMatch other)
    {
        if (From == other.From && To == other.To)
            return true;
        return false;
    }
}

public struct ForceDetails
{
    public float Force;
    public float Distance;

    public ForceDetails(float force, float maxDistance)
    {
        Force = force;
        Distance = maxDistance;
    }
}

public class MatchDetails : Tuple<ColorMatch, ForceDetails>
{
    public ColorMatch Match => Item1;
    public ForceDetails ForceDetails => Item2;

    public MatchDetails(ColorMatch match, ForceDetails forceDetails) : base(match, forceDetails)
    {
    }
}

public static class ForceCalculator
{
    public static Vector2 WorldSpace;
    public static float SafeDistance = 6f;
    public static float SafeValue = 0f;

    private static Dictionary<ColorMatch, ForceDetails> _rules
        = new Dictionary<ColorMatch, ForceDetails>();

    private static Dictionary<ColorMatch, float> _ranges = new Dictionary<ColorMatch, float>();
    public static Dictionary<Color, float> Ranges = new Dictionary<Color, float>();

    public static Dictionary<ColorMatch, ForceDetails> Rules => _rules;
    public static void Rule(Color from, Color to, float force, float distance)
    {
        var colorMatch = new ColorMatch(from, to);
        ForceDetails forceDetails = new ForceDetails(force, distance);
        if (!_rules.ContainsKey(colorMatch))
            _rules.Add(colorMatch, forceDetails);
        else
            _rules[colorMatch] = forceDetails;
        if(!Ranges.ContainsKey(from))
        {
            Ranges.Add(from, distance);
        }
        else
        {
            if (Ranges[from]<distance)
                Ranges[from] = distance;
        }
    
    }

    public static float CalculateForce(Color from, Color to, float dist)
    {
        var match = new ColorMatch(from, to);

        if (!_rules.ContainsKey(match))
            return 0f;
        else
        {
            var forceDetails = _rules[match];
            if (dist > forceDetails.Distance)
                return 0f;
            var frc = forceDetails.Force;
            //if (dist < SafeDistance)
            //  frc = frc * SafeValue;
            if (dist < SafeDistance)
                return -1;
               // return -frc;
            return frc / dist;
            //if (dist > SafeDistance)
            //    return frc / dist;
            //else
            //    return (frc/dist*dist) * SafeValue;
        }
    }
}

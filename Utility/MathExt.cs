using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Godot;

public static class MathExt
{
    public static float ToroidalDistance(float x1, float y1, float x2, float y2)
    {
        float dx = Mathf.Abs(x2 - x1);
        float dy = Mathf.Abs(y2 - y1);

        if (dx > .5f)
            dx = 1f - dx;

        if(dy>.5f)
            dy= 1f - dy;

        return Mathf.Sqrt(dx * dx + dy * dy);
    }

    
}
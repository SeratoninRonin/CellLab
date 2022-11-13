using Godot;

public static class Vector3Ext
{
    public static float SumXYZ(this Vector3 v)
    {
        return v.x + v.y + v.z;
    }

}

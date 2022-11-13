using System;
using Godot;

public static class StringConvert
{
    public static Vector2 Vec2FromString(string str)
    {
        var v = new Vector2();
        str = str.Trim('(', ')');
        var split = str.Split(',');
        v.x = float.Parse(split[0]);
        v.y = float.Parse(split[1]);
        return v;
    }

    public static Vector3 Vec3FromString(string str)
    {
        var v = new Vector3();
        str = str.Trim('(', ')');
        var split = str.Split(',');
        v.x = float.Parse(split[0]);
        v.y = float.Parse(split[1]);
        v.z = float.Parse(split[2]);
        return v;
    }

    public static DateTime DateTimeFromString(string dateTimeString)
    {
        var text = dateTimeString;
        var dateTimeAMPM = text.Split(' ');
        var monthDayYear = dateTimeAMPM[0].Split('/');
        var hourMinutesSeconds = dateTimeAMPM[1].Split(':');
        var month = int.Parse(monthDayYear[0]);
        var day = int.Parse(monthDayYear[1]);
        var year = int.Parse(monthDayYear[2]);
        var hours = int.Parse(hourMinutesSeconds[0]);
        var minutes = int.Parse(hourMinutesSeconds[1]);
        var seconds = int.Parse(hourMinutesSeconds[2]);
        if (dateTimeAMPM[2] == "PM")
        {
            if (hours != 12)
            {
                hours += 12;
            }
        }
        else if (hours == 12)
        {
            hours = 0;
        }

       return new DateTime(year, month, day, hours, minutes, seconds);
    }

    public static Color ColorFromString(string str)
    {
        var c = new Color();
        var split = str.Split(',');
        c.r = float.Parse(split[0]);
        c.g = float.Parse(split[1]);
        c.b = float.Parse(split[2]);
        c.a = float.Parse(split[3]);
        return c;
    }
}
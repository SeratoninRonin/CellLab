using System;
using Godot;

public static class DateTimeExt
{
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
        GD.Print("Translating date string: " + dateTimeString);
        GD.Print(year + " " + month + " " + day + " " + hours + " " + minutes +" "+ seconds);
        return new DateTime(year, month, day, hours, minutes, seconds);
    }
}

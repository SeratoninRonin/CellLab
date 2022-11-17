using Godot;
using System;
using System.Diagnostics;
using System.IO;

public class ColorTextLog
{
    public bool Active = true;
#if DEBUG
    public bool TraceOn = true;
#endif

    public ColorTextLog(string path, string title = "Game Log File")
    {
        Active = true;
        var textOut = new StreamWriter(new FileStream(path, FileMode.Create, FileAccess.Write));
        textOut.WriteLine(title);
        textOut.WriteLine("<br>");
        textOut.WriteLine("<span style=&quotfont - family: &quot;Kootenay&quot;; color: #000000; &quot>");
        textOut.WriteLine("Log started at " + DateTime.Now.ToLongTimeString() + "</span><hr />");
        textOut.Close();
        Path = path;
    }

    public string Path { get; }

    public void Log(string text)
    {
        Log(text, Colors.Black);
        if (!Active)
        {
            return;
        }
    }

    public void Log(string text, Color textColor)
    {
        if (!Active)
        {
            return;
        }

        var begin = "<span style =\"color: " + textColor.ToHtml() + ";\">";
        var build = begin + DateTime.Now.ToLongTimeString();
        var ampm = build.Substring(build.Length - 3);
        build = build.Remove(build.Length - 3, 3);
        build += "." + DateTime.Now.Millisecond + ampm + " : " + text + "</span><br>";
        text = build;
        Output(text);
#if DEBUG
        if (TraceOn)
        {
            Trace.WriteLine(text);
        }
#endif
    }

    private void Output(string text)
    {
        try
        {
            var textOut = new StreamWriter(new FileStream(Path, FileMode.Append, FileAccess.Write));
            textOut.WriteLine(text);
            textOut.Close();
        }
        catch (Exception e)
        {
            var error = e.Message;
#if DEBUG
            if (TraceOn)
            {
                Trace.WriteLine(error);
            }
#endif
        }
    }
}
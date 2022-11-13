using System;
using System.Diagnostics;
using System.IO;

public class TextLog
{
    public bool Active = true;

#if DEBUG
    public bool TraceOn = true;
#endif

    public TextLog(string path, string title = "!Game Log File")
    {
        Active = true;
        var textOut = new StreamWriter(new FileStream(path, FileMode.Create, FileAccess.Write));
        if (title != null)
        {
            textOut.WriteLine(title);
        }

        textOut.WriteLine("!Log started at " + DateTime.Now.ToLongTimeString());
        textOut.Close();
        Path = path;
    }

    public string Path { get; }

    public void Log(string text)
    {
        if (!Active)
        {
            return;
        }

        var build = DateTime.Now.ToLongTimeString();
        var ampm = build.Substring(build.Length - 3);
        build = build.Remove(build.Length - 3, 3);
        build += "." + DateTime.Now.Millisecond + ampm + " -: " + text;
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

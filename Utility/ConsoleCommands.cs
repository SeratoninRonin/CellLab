using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Godot;

public static class ConsoleCommands
{
    [Command("mem", "Estimates current memory usage")]
    public static void MemoryUse()
    {
        DebugConsole.Log(OS.GetStaticMemoryUsage().ToString() + " B");
        var b = OS.GetStaticMemoryUsage();
        var kb = b / 1024;
        var mb = kb / 1024;
        DebugConsole.Log(mb.ToString() + "MB");
        
    }

    [Command("fps","Estimates the current frames per second")]
    public static void LogFPS()
    {
        DebugConsole.Log(Engine.GetFramesPerSecond() + " fps");
    }
    
}
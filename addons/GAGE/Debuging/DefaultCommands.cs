using Godot;
using System;
using System.Text;


#if DEBUG
[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
public class CommandAttribute : Attribute
{
    public string Name;
    public string Help;


    public CommandAttribute(string name, string help)
    {
        Name = name;
        Help = help;
    }
}

public partial class DebugConsole : Control
{
    [Command("quit", "Quits the Game")]
    private static void Quit()
    {
        Core.Quit();
    }

    [Command("clear", "Clears the terminal")]
    private static void Clear()
    {
        Flush();
    }

    [Command("debug-render", "enables/disables debug rendering")]
    private static void DebugRender()
    {
        Core.DebugRenderEnabled = !Core.DebugRenderEnabled;
        Log(string.Format("Debug rendering {0}",
            Core.DebugRenderEnabled ? "enabled" : "disabled"));
    }

    [Command("help", "Shows usage help for a given command")]
    private static void Help(string command)
    {
        if (_instance._sorted.Contains(command))
        {
            var c = _instance._commands[command];
            StringBuilder str = new StringBuilder();

            //Title
            str.Append(":: ");
            str.Append(command);

            //Usage
            if (!string.IsNullOrEmpty(c.Usage))
            {
                str.Append(" ");
                str.Append(c.Usage);
            }

            Log(str.ToString());

            //Help
            if (string.IsNullOrEmpty(c.Help))
            {
                Log("No help info set");
            }
            else
            {
                Log(c.Help);
            }
        }
        else
        {
            StringBuilder str = new StringBuilder();
            str.Append("Commands list: ");
            str.Append(string.Join(", ", _instance._sorted));
            Log(str.ToString());
            Log("Type 'help command' for more info on that command!");
        }
    }
}
#endif
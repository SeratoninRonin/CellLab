using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
#if DEBUG
public partial class DebugConsole : Control
{
    public static bool CaptureToTextLog = true;
    private static DebugConsole _instance;
    private Label _echo;
    private LineEdit _input;
    private const int MAX_LINES = 2;
    private const int MAX_CMD_BUFFER = 5;
    private List<string> _buffer = new List<string>();
    private List<string> _commandBuffer = new List<string>();
    private int cmdIdx = -1;
    private Dictionary<string, CommandInfo> _commands = new Dictionary<string, CommandInfo>();
    private List<string> _sorted = new List<string>();

    public override void _Ready()
    {
        _instance = this;
        _echo = GetNode<Label>("Panel/Echo");
        _input = GetNode<LineEdit>("Panel/Input");

        BuildCommandsList();
    }

    public override void _Process(float delta)
    {
        if (Input.IsActionJustReleased("debug_console"))
        {
            Toggle();
        }

        if (Visible)
        {
            if (Input.IsActionJustReleased("debug_up") || Input.IsActionJustReleased("debug_down"))
            {
                if (_commandBuffer.Count > 0)
                {
                    if (Input.IsActionJustReleased("debug_up"))
                    {
                        cmdIdx++;
                    }
                    else
                    {
                        cmdIdx--;
                    }

                    cmdIdx = MathHelper.Clamp(cmdIdx, 0, MAX_CMD_BUFFER - 1);
                    _input.Clear();
                    _input.Text = _commandBuffer[_commandBuffer.Count - 1 - cmdIdx];
                }
            }

            string final = string.Empty;
            foreach (var l in _buffer)
            {
                final += l + "\n";
            }
            _echo.Text = final;
        }
    }
    private void Toggle()
    {
        _instance._input.Clear();
        Visible = !Visible;
        if (Visible)
        {
            _input.GrabFocus();
        }
    }

    public static void Flush()
    {
        _instance._input.Clear();
        _instance._echo.Text = string.Empty;
        _instance._buffer.Clear();
    }

    public void OnTextEntered(string new_text)
    {
        Log(new_text);
        _input.Clear();
        var split = new_text.Split(' ');
        var cmd = split[0];
        var arg = split.Skip(1).ToArray();
        ExecuteCommand(cmd, arg);
        _commandBuffer.Add(new_text);
        if (_commandBuffer.Count > MAX_CMD_BUFFER)
        {
            _commandBuffer.RemoveAt(0);
        }
    }

    public static void Log(string message)
    {
        //TODO: break the message into lines
        if (_instance._buffer.Count > MAX_LINES)
        {
            _instance._buffer.RemoveAt(0);
        }

        _instance._buffer.Add(message);
        if (CaptureToTextLog)
        {
            //Core.Instance.TextLog.Log(message);
        }
    }

    #region Execute

    private void ExecuteCommand(string command, string[] args)
    {
        if (_commands.ContainsKey(command))
        {
            _commands[command].Action(args);
        }
        else
        {
            Log("Command '" + command + "' not found! Type 'help' for list of commands");
        }
    }
    #endregion

    #region Process commands

    private void BuildCommandsList()
    {
        ProcessAssembly(Core._instance.GetType().GetTypeInfo().Assembly);

        foreach (var command in _commands)
        {
            _sorted.Add(command.Key);
        }

        _sorted.Sort();
    }

    private void ProcessAssembly(Assembly assembly)
    {
        foreach (var type in assembly.DefinedTypes)
        {
            foreach (var method in type.DeclaredMethods)
            {
                CommandAttribute attr = null;
                var attrs = method.GetCustomAttributes(typeof(CommandAttribute), false)
                    .Where(a => a is CommandAttribute);
                if (IEnumerableExt.Count(attrs) > 0)
                {
                    attr = attrs.First() as CommandAttribute;
                }

                if (attr != null)
                {
                    ProcessMethod(method, attr);
                }
            }
        }
    }

    private void ProcessMethod(MethodInfo method, CommandAttribute attr)
    {
        if (!method.IsStatic)
        {
            throw new Exception(method.DeclaringType.Name + "." + method.Name +
                                " is marked as a command, but is not static");
        }
        else
        {
            var info = new CommandInfo();
            info.Help = attr.Help;

            var parameters = method.GetParameters();
            var defaults = new object[parameters.Length];
            var usage = new string[parameters.Length];

            for (var i = 0; i < parameters.Length; i++)
            {
                var p = parameters[i];
                usage[i] = p.Name + ":";

                if (p.ParameterType == typeof(string))
                {
                    usage[i] += "string";
                }
                else if (p.ParameterType == typeof(int))
                {
                    usage[i] += "int";
                }
                else if (p.ParameterType == typeof(float))
                {
                    usage[i] += "float";
                }
                else if (p.ParameterType == typeof(bool))
                {
                    usage[i] += "bool";
                }
                else
                {
                    throw new Exception(method.DeclaringType.Name + "." + method.Name +
                                            " is marked as a command, but has an invalid parameter type. Allowed types are: string, int, float, and bool");
                }

                // no System.DBNull in PCL so we fake it
                if (p.DefaultValue.GetType().FullName == "System.DBNull")
                {
                    defaults[i] = null;
                }
                else if (p.DefaultValue != null)
                {
                    defaults[i] = p.DefaultValue;
                    if (p.ParameterType == typeof(string))
                    {
                        usage[i] += "=\"" + p.DefaultValue.ToString() + "\"";
                    }
                    else
                    {
                        usage[i] += "=" + p.DefaultValue.ToString();
                    }
                }
                else
                {
                    defaults[i] = null;
                }
            }

            if (usage.Length == 0)
            {
                info.Usage = "";
            }
            else
            {
                info.Usage = "[" + string.Join(" ", usage) + "]";
            }

            info.Action = args =>
                {
                    if (parameters.Length == 0)
                    {
                        method.Invoke(null, null);
                    }
                    else
                    {
                        var param = (object[])defaults.Clone();

                        for (var i = 0; i < param.Length && i < args.Length; i++)
                        {
                            if (parameters[i].ParameterType == typeof(string))
                            {
                                param[i] = ArgString(args[i]);
                            }
                            else if (parameters[i].ParameterType == typeof(int))
                            {
                                param[i] = ArgInt(args[i]);
                            }
                            else if (parameters[i].ParameterType == typeof(float))
                            {
                                param[i] = ArgFloat(args[i]);
                            }
                            else if (parameters[i].ParameterType == typeof(bool))
                            {
                                param[i] = ArgBool(args[i]);
                            }
                        }

                        try
                        {
                            method.Invoke(null, param);
                        }
                        catch (Exception e)
                        {
                            Log(e.Message);
                        }
                    }
                };

            _commands[attr.Name] = info;
        }
    }

    private struct CommandInfo
    {
        public Action<string[]> Action;
        public string Help;
        public string Usage;
    }

    #region Parsing Arguments

    private static string ArgString(string arg)
    {
        if (arg == null)
        {
            return "";
        }
        else
        {
            return arg;
        }
    }

    private static bool ArgBool(string arg)
    {
        if (arg != null)
        {
            return !(arg == "0" || arg.ToLower() == "false" || arg.ToLower() == "f");
        }
        else
        {
            return false;
        }
    }

    private static int ArgInt(string arg)
    {
        try
        {
            return Convert.ToInt32(arg);
        }
        catch
        {
            return 0;
        }
    }

    private static float ArgFloat(string arg)
    {
        try
        {
            return Convert.ToSingle(arg);
        }
        catch
        {
            return 0;
        }
    }

    #endregion


    #endregion
}
#endif
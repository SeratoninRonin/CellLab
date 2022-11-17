using Godot;
using System;
using System.Collections;

public class Core : Node
{
    /// <summary>
    /// provides access to the single Core/Game instance
    /// </summary>
    public static Core Instance => _instance;

    /// <summary>
    /// facilitates easy access to the global Content instance for internal classes
    /// </summary>
    internal static Core _instance;

    private FastList<GlobalManager> _globalManagers = new FastList<GlobalManager>();
    private CoroutineManager _coroutineManager = new CoroutineManager();
    private TimerManager _timerManager = new TimerManager();

#if DEBUG

    //internal TextLog TextLog = new TextLog("core.log");
    private static DebugOverlay _immediate;

    public static DebugOverlay Immediate => _immediate;
    public static bool DebugRenderEnabled = false;
#endif

    public Core()
    {
        _instance = this;
    }

    public override void _Ready()
    {
        // setup systems
        RegisterGlobalManager(_coroutineManager);
        RegisterGlobalManager(_timerManager);

#if DEBUG
        _immediate = (DebugOverlay)GD.Load<PackedScene>("res://addons/GAGE/Debuging/DebugOverlay.tscn").Instance();
        _immediate.Name = "Immediate";
        AddChild(_immediate);

        if (_immediate.Draw == null)
        {
            _immediate.Draw = _immediate.GetNode<DebugDrawControl>("Draw");
        }

        InputMap.AddAction("debug_console");
        var key = new InputEventKey();
        key.Scancode = (int)KeyList.Quoteleft;
        key.Pressed = true;
        InputMap.ActionAddEvent("debug_console", key);

        InputMap.AddAction("debug_up");
        var key2 = new InputEventKey();
        key2.Scancode = (int)KeyList.Up;
        key2.Pressed = true;
        InputMap.ActionAddEvent("debug_up", key2);

        InputMap.AddAction("debug_down");
        var key3 = new InputEventKey();
        key3.Scancode = (int)KeyList.Down;
        key3.Pressed = true;
        InputMap.ActionAddEvent("debug_down", key3);

        DebugConsole.Log("Core ready!");

#endif
    }

    public override void _Process(float delta)
    {
        Time.Update(delta);
        for (var i = _globalManagers.Length - 1; i >= 0; i--)
        {
            if (_globalManagers.Buffer[i].Enabled)
            {
                _globalManagers.Buffer[i].Update(delta);
            }
        }
    }

    #region Game Functions

    public static void Quit()
    {
        _instance.GetTree().Quit();
    }

    #endregion Game Functions

    #region Global Managers

    /// <summary>
    /// adds a global manager object that will have its update method called each frame before Scene.update is called
    /// </summary>
    /// <returns>The global manager.</returns>
    /// <param name="manager">Manager.</param>
    public static void RegisterGlobalManager(GlobalManager manager)
    {
        _instance._globalManagers.Add(manager);
        manager.Enabled = true;
    }

    /// <summary>
    /// removes the global manager object
    /// </summary>
    /// <returns>The global manager.</returns>
    /// <param name="manager">Manager.</param>
    public static void UnregisterGlobalManager(GlobalManager manager)
    {
        _instance._globalManagers.Remove(manager);
        manager.Enabled = false;
    }

    /// <summary>
    /// gets the global manager of type T
    /// </summary>
    /// <returns>The global manager.</returns>
    /// <typeparam name="T">The 1st type parameter.</typeparam>
    public static T GetGlobalManager<T>() where T : GlobalManager
    {
        for (var i = 0; i < _instance._globalManagers.Length; i++)
        {
            if (_instance._globalManagers.Buffer[i] is T)
            {
                return _instance._globalManagers.Buffer[i] as T;
            }
        }

        return null;
    }

    #endregion Global Managers

    #region Systems access

    /// <summary>
    /// starts a coroutine. Coroutines can yeild ints/floats to delay for seconds or yeild to other calls to startCoroutine.
    /// Yielding null will make the coroutine get ticked the next frame.
    /// </summary>
    /// <returns>The coroutine.</returns>
    /// <param name="enumerator">Enumerator.</param>
    public static ICoroutine StartCoroutine(IEnumerator enumerator)
    {
        return _instance._coroutineManager.StartCoroutine(enumerator);
    }

    /// <summary>
    /// schedules a one-time or repeating timer that will call the passed in Action
    /// </summary>
    /// <param name="timeInSeconds">Time in seconds.</param>
    /// <param name="repeats">If set to <c>true</c> repeats.</param>
    /// <param name="context">Context.</param>
    /// <param name="onTime">On time.</param>
    public static ITimer Schedule(float timeInSeconds, bool repeats, object context, Action<ITimer> onTime)
    {
        return _instance._timerManager.Schedule(timeInSeconds, repeats, context, onTime);
    }

    /// <summary>
    /// schedules a one-time timer that will call the passed in Action after timeInSeconds
    /// </summary>
    /// <param name="timeInSeconds">Time in seconds.</param>
    /// <param name="context">Context.</param>
    /// <param name="onTime">On time.</param>
    public static ITimer Schedule(float timeInSeconds, object context, Action<ITimer> onTime)
    {
        return _instance._timerManager.Schedule(timeInSeconds, false, context, onTime);
    }

    /// <summary>
    /// schedules a one-time or repeating timer that will call the passed in Action
    /// </summary>
    /// <param name="timeInSeconds">Time in seconds.</param>
    /// <param name="repeats">If set to <c>true</c> repeats.</param>
    /// <param name="onTime">On time.</param>
    public static ITimer Schedule(float timeInSeconds, bool repeats, Action<ITimer> onTime)
    {
        return _instance._timerManager.Schedule(timeInSeconds, repeats, null, onTime);
    }

    /// <summary>
    /// schedules a one-time timer that will call the passed in Action after timeInSeconds
    /// </summary>
    /// <param name="timeInSeconds">Time in seconds.</param>
    /// <param name="onTime">On time.</param>
    public static ITimer Schedule(float timeInSeconds, Action<ITimer> onTime)
    {
        return _instance._timerManager.Schedule(timeInSeconds, false, null, onTime);
    }

    #endregion Systems access

    #region Screenshots

    public static Image Screenshot()
    {
        var img = _instance.GetViewport().GetTexture().GetData();
        img.FlipY();
        return img;
    }

    public static Error Screenshot(string path)
    {
        var img = Screenshot();
        return img.SavePng(path);
    }

#if DEBUG

    [Command("screenshot", "takes a screenshot to the specified filename after an optional delay")]
    public static void Screenshot(string filename, float delay = 0)
    {
        if (delay > 0)
        {
            Core.Schedule(delay, (t) =>
            {
                Screenshot(filename);
                t.Stop();
            });
        }
        else
            Screenshot(filename);
    }

#endif

    #endregion Screenshots
}
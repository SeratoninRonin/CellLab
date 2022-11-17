using System;
using System.Collections.Generic;

/// <summary>
/// allows delayed and repeated execution of an Action
/// </summary>
public class TimerManager : GlobalManager
{
    private List<QuickTimer> _timers = new List<QuickTimer>();

    public override void Update(float delta)
    {
        for (var i = _timers.Count - 1; i >= 0; i--)
        {
            // tick our timer. if it returns true it is done so we remove it
            if (_timers[i].Tick(delta))
            {
                _timers[i].Unload();
                _timers.RemoveAt(i);
            }
        }
    }

    /// <summary>
    /// schedules a one-time or repeating timer that will call the passed in Action
    /// </summary>
    /// <param name="timeInSeconds">Time in seconds.</param>
    /// <param name="repeats">If set to <c>true</c> repeats.</param>
    /// <param name="context">Context.</param>
    /// <param name="onTime">On time.</param>
    internal ITimer Schedule(float timeInSeconds, bool repeats, object context, Action<ITimer> onTime)
    {
        var timer = new QuickTimer();
        timer.Initialize(timeInSeconds, repeats, context, onTime);
        _timers.Add(timer);

        return timer;
    }
}
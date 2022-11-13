using Godot;
using System;
using System.Collections.Generic;

public class StateMachineT<T>
{ 
    public event Action OnStateChanged;

    public StateT<T> CurrentState => _currentState;


    public StateT<T> PreviousState;
    public float ElapsedTimeInState = 0f;

    protected StateT<T> _currentState;
    protected T _context;
    protected Dictionary<Type, StateT<T>> _states = new Dictionary<Type, StateT<T>>();
    public Dictionary<Type, StateT<T>> States => _states;

    public StateMachineT(T context, StateT<T> initialState)
    {
        _context = context;

        // setup our initial state
        AddState(initialState);
        _currentState = initialState;
        _currentState.Begin();
    }


    /// <summary>
    /// adds the state to the machine
    /// </summary>
    public void AddState(StateT<T> state)
    {
        state.SetMachineAndContext(this, _context);
        _states[state.GetType()] = state;
    }


    /// <summary>
    /// ticks the state machine with the provided delta time
    /// </summary>
    public virtual void Update(float deltaTime)
    {
        ElapsedTimeInState += deltaTime;
        _currentState.Reason();
        _currentState.Update(deltaTime);
    }

    /// <summary>
    /// Gets a specific state from the machine without having to
    /// change to it.
    /// </summary>
    public virtual R GetState<R>() where R : StateT<T>
    {
        var type = typeof(R);
        
        if (_states.ContainsKey(type))
            return (R)_states[type];
        else return null;
    }

    public virtual R GetState<R>(Type type) where R : StateT<T>
    {
        if (_states.ContainsKey(type))
            return (R)_states[type];
        else return null;
    }

    public void ChangeState(Type t)
    {
        var newType = t;
        if (_currentState.GetType() == newType)
        {
            return;
        }

        // only call end if we have a currentState
        if (_currentState != null)
        {
            _currentState.End();
        }

        if (!_states.ContainsKey(newType))
            return;

        // swap states and call begin
        ElapsedTimeInState = 0f;
        PreviousState = _currentState;
        _currentState = _states[newType];
        _currentState.Begin();

        // fire the changed event if we have a listener
        if (OnStateChanged != null)
        {
            OnStateChanged();
        }
    }

    /// <summary>
    /// changes the current state
    /// </summary>
    public R ChangeState<R>() where R : StateT<T>
    {
        // avoid changing to the same state
        var newType = typeof(R);
        if (_currentState.GetType() == newType)
        {
            return _currentState as R;
        }

        // only call end if we have a currentState
        if (_currentState != null)
        {
            _currentState.End();
        }

        if (!_states.ContainsKey(newType))
            return null;

        // swap states and call begin
        ElapsedTimeInState = 0f;
        PreviousState = _currentState;
        _currentState = _states[newType];
        _currentState.Begin();

        // fire the changed event if we have a listener
        if (OnStateChanged != null)
        {
            OnStateChanged();
        }

        return _currentState as R;
    }
}


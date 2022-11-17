using Godot;
using System.Collections.Generic;

/// <summary>
/// Objects implementing this interface will have {@link #reset()} called when passed to {@link #push(Object)}
/// </summary>
public interface IScenePool
{
    /// <summary>
    /// Resets the object for reuse. Object references should be nulled and fields may be set to default values
    /// </summary>
    void Reset();
}

/// <summary>
/// simple static class that can be used to pool any object
/// </summary>
public class ScenePool : Node2D
{
    [Export]
    public PackedScene PooledScene;

    [Export]
    public int CacheSize = 8;

    [Export]
    public bool Trim = false;

    [Export]
    public bool Randomize = false;

    private Queue<Node> _objectQueue = new Queue<Node>(0);

    public override void _Ready()
    {
        WarmCache(CacheSize);
    }

    public override void _Process(float delta)
    {
        if (Trim && _objectQueue.Count > CacheSize)
        {
            TrimCache(CacheSize);
        }
    }

    public override void _ExitTree()
    {
        ClearCache();
    }

    public bool Contains(Node n)
    {
        return _objectQueue.Contains(n);
    }

    /// <summary>
    /// warms up the cache filling it with a max of cacheCount objects
    /// </summary>
    /// <param name="cacheCount">new cache count</param>
    public void WarmCache(int cacheCount)
    {
        cacheCount -= _objectQueue.Count;
        if (cacheCount > 0)
        {
            for (var i = 0; i < cacheCount; i++)
            {
                if (Randomize)
                {
                    Randomizer.Reseed();
                    GD.Randomize();
                }
                var t = (Node)PooledScene.Instance();
                AddChild(t);

                _objectQueue.Enqueue(t);
            }
        }
    }

    /// <summary>
    /// trims the cache down to cacheCount items
    /// </summary>
    /// <param name="cacheCount">Cache count.</param>
    public void TrimCache(int cacheCount)
    {
        if (cacheCount < _objectQueue.Count)
        {
#if DEBUG

            DebugConsole.Log(this.Name + " Trim " + _objectQueue.Count + " to " + cacheCount);
#endif
            while (cacheCount > _objectQueue.Count)
            {
                var o = _objectQueue.Dequeue();
                o.QueueFree();
            }
        }
    }

    /// <summary>
    /// clears out the cache
    /// </summary>
    public void ClearCache()
    {
        foreach (var o in _objectQueue)
        {
            o.QueueFree();
        }
        _objectQueue.Clear();
    }

    /// <summary>
    /// pops an item off the stack if available creating a new item as necessary
    /// </summary>
    public Node Obtain()
    {
        Node n = new Node();

        if (_objectQueue.Count > 0)
        {
            n = _objectQueue.Dequeue();
        }
        else
        {
            if (Randomize)
            {
                //TODO: What are we doing here?
                Randomizer.Reseed();
                GD.Randomize();
            }
            n = (Node)PooledScene.Instance();
            AddChild(n);
        }
        return n;
    }

    /// <summary>
    /// pushes an item back on the stack
    /// </summary>
    /// <param name="obj">Object.</param>
    public void Free(Node obj)
    {
        if (!_objectQueue.Contains(obj))
        {
            _objectQueue.Enqueue(obj);

            if (obj is IScenePool p)
            {
                p.Reset();
            }
        }
    }

    public void Reset()
    {
        var list = GetChildren();
        foreach (var o in list)
        {
            if (o is Node n)
            {
                Free(n);
            }
        }
    }
}
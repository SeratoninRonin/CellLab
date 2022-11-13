using System;
using Godot;
using System.Collections.Generic;
using System.Threading;
using System.Linq;

public class Cell
{
    int _x, _y;
    int _index;
    ICellGraph2D _graph;
    public List<Cell> Neighbors = new List<Cell>();
    public Dictionary<string, float> Traits { get; private set; }
    public int Index => _index;
    public int X => _x;
    public int Y => _y;
    public Vector2 Location { get; private set; }
    public Cell(int x, int y, ICellGraph2D graph, KeyValuePair<string, float>[] traits = null)
    {
        _graph = graph;
        _x = x;
        _y = y;
        var max = graph.Height < graph.Width ? graph.Height : graph.Width;
        _index = x * max + y;
        Location = new Vector2(x, y);
        Traits = new Dictionary<string, float>();
        if (traits != null)
        {
            foreach (var kvp in traits)
                Traits.Add(kvp.Key, kvp.Value);
        }
    }

    public List<Cell> GetNeighborsWithTraitAny(string trait)
    {
        var list = new List<Cell>();

        foreach (var n in Neighbors)
            if (n.Traits.ContainsKey(trait))
                list.Add(n);

        return list;
    }

    public List<Cell> GetNeighborsWithTraitOver(string trait, float minInclusive)
    {
        var list = new List<Cell>();

        foreach (var n in Neighbors)
            if (n.Traits.ContainsKey(trait) && n.Traits[trait] >= minInclusive)
                list.Add(n);

        return list;
    }

    public List<Cell> GetNeighborsWithTraitUnder(string trait, float maxInclusive)
    {
        var list = new List<Cell>();

        foreach (var n in Neighbors)
            if (n.Traits.ContainsKey(trait) && n.Traits[trait] <= maxInclusive)
                list.Add(n);

        return list;
    }
}
using Godot;
using System.Collections.Generic;

public class Cell
{
    private int _x, _y;
    private int _index;
    public Dictionary<string, float> Traits { get; private set; }
    public int Index => _index;
    public int X => _x;
    public int Y => _y;
    public Vector2 Location { get; private set; }
    public List<Cell> Neighbors { get; private set; }

    public Cell(int x, int y, int rankOffset, KeyValuePair<string, float>[] traits = null)
    {
        _x = x;
        _y = y;
        _index = x * rankOffset + y;
        Location = new Vector2(x, y);
        Traits = new Dictionary<string, float>();
        if (traits != null)
        {
            foreach (var kvp in traits)
                Traits.Add(kvp.Key, kvp.Value);
        }
    }

    public int CountNeightborsWithTrait(string trait)
    {
        var count = 0;
        foreach (var n in Neighbors)
            if (n.Traits.ContainsKey(trait))
                count++;
        return count;
    }

    public List<Cell> GetNeighborsWithTrait(string trait, float amt = -1)
    {
        var list = new List<Cell>();

        foreach (var n in Neighbors)
        {
            if (n.Traits.ContainsKey(trait))
            {
                if (amt < 0)
                    list.Add(n);
                else if (n.Traits[trait] == amt)
                    list.Add(n);
            }
        }

        return list;
    }

    public int CountNeighborsWithTraitOver(string trait, float minInclusive)
    {
        var count = 0;
        foreach (var n in Neighbors)
            if (n.Traits.ContainsKey(trait) && n.Traits[trait] < minInclusive)
                count++;
        return count;
    }

    public List<Cell> GetNeighborsWithTraitOver(string trait, float minInclusive)
    {
        var list = new List<Cell>();

        foreach (var n in Neighbors)
            if (n.Traits.ContainsKey(trait) && n.Traits[trait] >= minInclusive)
                list.Add(n);

        return list;
    }

    public int CountNeighborsWithTraitUnder(string trait, float maxExclusive)
    {
        var count = 0;
        foreach (var n in Neighbors)
            if (n.Traits.ContainsKey(trait) && n.Traits[trait] < maxExclusive)
                count++;
        return count;
    }

    public List<Cell> GetNeighborsWithTraitUnder(string trait, float maxExclusive)
    {
        var list = new List<Cell>();

        foreach (var n in Neighbors)
            if (n.Traits.ContainsKey(trait) && n.Traits[trait] < maxExclusive)
                list.Add(n);

        return list;
    }

    public void RefreshNeighbors(CellGrid grid)
    {
        Neighbors = new List<Cell>();
        bool edge = false;
        var end = grid.GridWidth - 1;
        var bot = grid.GridHeight - 1;
        if (X == 0 || Y == 0 || X == end || Y == bot)
            edge = true;

        for (int x = -1; x < 2; x++)
        {
            for (int y = -1; y < 2; y++)
            {
                if (x == 0 && y == 0)
                    continue;

                var w = X + x;
                var z = Y + y;
                if (edge && grid.Wrap)
                {
                    if (X == 0 && x == -1)
                    {
                        w = end;
                    }
                    else if (X == end && x == 1)
                    {
                        w = 0;
                    }

                    if (Y == 0 && y == -1)
                    {
                        z = bot;
                    }
                    else if (Y == bot && y == 1)
                    {
                        z = 0;
                    }
                }

                if (grid.IsInBounds(w, z))
                {
                    Neighbors.Add(grid[w, z]);
                    //catch (Exception e)
                    //{
                    //    GD.Print("0-------EXCEPTION-------------*");
                    //   // GD.Print(e.Message.ToString());
                    //    GD.Print(new Vector2(w, z));
                    //    GD.Print(grid.Cells.Count);
                    //}
                }
            }
        }
    }
}
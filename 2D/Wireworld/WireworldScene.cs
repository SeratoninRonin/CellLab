using Godot;
using System;

public class WireworldScene : Node2D
{
    TileMap _grid;
    MoveableCamera _camera;
    Sprite _selector;

    public override void _Ready()
    {
        _grid = GetNode<TileMap>("Grid");
        _camera = GetNode<MoveableCamera>("MoveableCamera");
        _selector = GetNode<Sprite>("SelectorSprite");
    }
}

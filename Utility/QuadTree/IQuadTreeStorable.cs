﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Godot;

public interface IQuadTreeStorable
{
    Rect2 Bounds { get; }
}

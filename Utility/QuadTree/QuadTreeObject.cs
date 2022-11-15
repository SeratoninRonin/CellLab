using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class QuadTreeObject<T> where T : IQuadTreeStorable
{
    public T Data;

    internal QuadTreeNode<T> Owner;

    public QuadTreeObject(T data)
    {
        Data = data;
    }
}

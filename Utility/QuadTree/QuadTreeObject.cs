public class QuadTreeObject<T> where T : IQuadTreeStorable
{
    public T Data;

    internal QuadTreeNode<T> Owner;

    public QuadTreeObject(T data)
    {
        Data = data;
    }
}
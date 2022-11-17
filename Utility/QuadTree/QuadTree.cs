using Godot;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// A QuadTree Object that provides fast and efficient storage of objects in a world space.
/// </summary>
/// <typeparam name="T">Any object implementing IQuadStorable.</typeparam>
public class QuadTree<T> : ICollection<T> where T : IQuadTreeStorable
{
    private readonly Dictionary<T, QuadTreeObject<T>> _wrappedDictionary = new Dictionary<T, QuadTreeObject<T>>();

    // Alternate method, use Parallel arrays
    // The root of this quad tree
    private readonly QuadTreeNode<T> _quadTreeRoot;

    /// <summary>
    /// Creates a QuadTree for the specified area.
    /// </summary>
    /// <param name="rect">The area this QuadTree object will encompass.</param>
    public QuadTree(Rect2 rect)
    {
        _quadTreeRoot = new QuadTreeNode<T>(rect);
    }

    /// <summary>
    /// Creates a QuadTree for the specified area.
    /// </summary>
    /// <param name="x">The top-left position of the area rectangle.</param>
    /// <param name="y">The top-right position of the area rectangle.</param>
    /// <param name="width">The width of the area rectangle.</param>
    /// <param name="height">The height of the area rectangle.</param>
    public QuadTree(int x, int y, int width, int height)
    {
        _quadTreeRoot = new QuadTreeNode<T>(new Rect2(x, y, width, height));
    }

    /// <summary>
    /// Gets the rectangle that bounds this QuadTree
    /// </summary>
    public Rect2 QuadRect
    {
        get { return _quadTreeRoot.QuadRect; }
    }

    /// <summary>
    /// Get the objects in this tree that intersect with the specified rectangle.
    /// </summary>
    /// <param name="rect">The rectangle to find objects in.</param>
    public List<T> GetObjects(Rect2 rect)
    {
        return _quadTreeRoot.GetObjects(rect);
    }

    public List<T> GetObjectsToroidal(Rect2 sourceRect)
    {
        return null;
    }

    /// <summary>
    /// Get the objects in this tree that intersect with the specified rectangle.
    /// </summary>
    /// <param name="rect">The rectangle to find objects in.</param>
    /// <param name="results">A reference to a list that will be populated with the results.</param>
    public void GetObjects(Rect2 rect, ref List<T> results)
    {
        _quadTreeRoot.GetObjects(rect, ref results);
    }

    /// <summary>
    /// Get all objects in this Quad, and it's children.
    /// </summary>
    public List<T> GetAllObjects()
    {
        return new List<T>(_wrappedDictionary.Keys);
    }

    /// <summary>
    /// Moves the object in the tree
    /// </summary>
    /// <param name="item">The item that has moved</param>
    public bool Move(T item)
    {
        if (Contains(item))
        {
            _quadTreeRoot.Move(_wrappedDictionary[item]);
            return true;
        }
        return false;
    }

    //public void DebugRender(SpriteBatch batch)
    //{
    //    DebugRenderNode(batch, _quadTreeRoot);

    //    foreach (var ele in this)
    //    {
    //        //batch.drawHollowRect(ele.bounds, Debug.Colors.colliderBounds, Debug.Size.lineSizeMultiplier);
    //        batch.DrawRectangle(ele.Bounds, Color.MonoGameOrange);
    //    }
    //}

    //public void DebugRenderNode(SpriteBatch batch, QuadTreeNode<T> node)
    //{
    //    if (node.isEmptyLeaf)
    //        //batch.drawHollowRect(node.quadRect, Color.Red * 0.5f, Debug.Size.lineSizeMultiplier);
    //        batch.DrawRectangle(node.QuadRect, Color.MonoGameOrange);

    //    if (node.TopLeftChild != null)
    //        DebugRenderNode(batch, node.TopLeftChild);

    //    if (node.TopRightChild != null)
    //        DebugRenderNode(batch, node.TopRightChild);

    //    if (node.BottomLeftChild != null)
    //        DebugRenderNode(batch, node.BottomLeftChild);

    //    if (node.BottomRightChild != null)
    //        DebugRenderNode(batch, node.BottomRightChild);
    //}

    /// <summary>
    /// The top left child for this QuadTree, only usable in debug mode
    /// </summary>
    public QuadTreeNode<T> rootQuad
    {
        get { return _quadTreeRoot; }
    }

    #region ICollection<T> Members

    ///<summary>
    /// Adds an item to the QuadTree
    ///</summary>
    ///
    ///<param name="item">The object to add to the <see cref="T:System.Collections.Generic.ICollection`1" />.</param>
    ///<exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.ICollection`1" /> is read-only.</exception>
    public void Add(T item)
    {
        var wrappedObject = new QuadTreeObject<T>(item);
        _wrappedDictionary.Add(item, wrappedObject);
        _quadTreeRoot.Insert(wrappedObject);
    }

    ///<summary>
    ///Removes all items from the <see cref="T:System.Collections.Generic.ICollection`1" />.
    ///</summary>
    ///
    ///<exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.ICollection`1" /> is read-only. </exception>
    public void Clear()
    {
        _wrappedDictionary.Clear();
        _quadTreeRoot.Clear();
    }

    ///<summary>
    ///Determines whether the QuadTree contains a specific value.
    ///</summary>
    ///
    ///<returns>
    ///true if <paramref name="item" /> is found in the <see cref="T:System.Collections.Generic.ICollection`1" />; otherwise, false.
    ///</returns>
    ///
    ///<param name="item">The object to locate in the <see cref="T:System.Collections.Generic.ICollection`1" />.</param>
    public bool Contains(T item)
    {
        return _wrappedDictionary.ContainsKey(item);
    }

    ///<summary>
    /// Copies the elements of the <see cref="T:System.Collections.Generic.ICollection`1" /> to an <see cref="T:System.Array" />, starting at a particular <see cref="T:System.Array" /> index.
    ///</summary>
    ///<param name="array">The one-dimensional <see cref="T:System.Array" /> that is the destination of the elements copied from <see cref="T:System.Collections.Generic.ICollection`1" />. The <see cref="T:System.Array" /> must have zero-based indexing.</param>
    ///<param name="arrayIndex">The zero-based index in <paramref name="array" /> at which copying begins.</param>
    ///<exception cref="T:System.ArgumentNullException"><paramref name="array" /> is null.</exception>
    ///<exception cref="T:System.ArgumentOutOfRangeException"><paramref name="arrayIndex" /> is less than 0.</exception>
    public void CopyTo(T[] array, int arrayIndex)
    {
        _wrappedDictionary.Keys.CopyTo(array, arrayIndex);
    }

    ///<summary>
    /// Gets the number of elements contained in the <see cref="T:System.Collections.Generic.ICollection`1" />.
    ///</summary>
    ///<returns>
    ///The number of elements contained in the <see cref="T:System.Collections.Generic.ICollection`1" />.
    ///</returns>
    public int Count
    {
        get { return _wrappedDictionary.Count; }
    }

    ///<summary>
    /// Gets a value indicating whether the <see cref="T:System.Collections.Generic.ICollection`1" /> is read-only.
    ///</summary>
    ///
    ///<returns>
    ///true if the <see cref="T:System.Collections.Generic.ICollection`1" /> is read-only; otherwise, false.
    ///</returns>
    ///
    public bool IsReadOnly
    {
        get { return false; }
    }

    ///<summary>
    /// Removes the first occurrence of a specific object from the QuadTree
    ///</summary>
    ///<returns>
    /// true if <paramref name="item" /> was successfully removed from the <see cref="T:System.Collections.Generic.ICollection`1" />; otherwise, false. This method also returns false if <paramref name="item" /> is not found in the original <see cref="T:System.Collections.Generic.ICollection`1" />.
    ///</returns>
    ///<param name="item">The object to remove from the <see cref="T:System.Collections.Generic.ICollection`1" />.</param>
    ///<exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.ICollection`1" /> is read-only.</exception>
    public bool Remove(T item)
    {
        if (Contains(item))
        {
            _quadTreeRoot.Delete(_wrappedDictionary[item], true);
            _wrappedDictionary.Remove(item);
            return true;
        }
        else
        {
            return false;
        }
    }

    #endregion ICollection<T> Members

    #region IEnumerable<T> and IEnumerable Members

    ///<summary>
    /// Returns an enumerator that iterates through the collection.
    ///</summary>
    ///
    ///<returns>
    ///A <see cref="T:System.Collections.Generic.IEnumerator`1" /> that can be used to iterate through the collection.
    ///</returns>
    ///<filterpriority>1</filterpriority>
    public IEnumerator<T> GetEnumerator()
    {
        return _wrappedDictionary.Keys.GetEnumerator();
    }

    ///<summary>
    /// Returns an enumerator that iterates through a collection.
    ///</summary>
    ///
    ///<returns>
    ///An <see cref="T:System.Collections.IEnumerator" /> object that can be used to iterate through the collection.
    ///</returns>
    ///<filterpriority>2</filterpriority>
    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    #endregion IEnumerable<T> and IEnumerable Members
}
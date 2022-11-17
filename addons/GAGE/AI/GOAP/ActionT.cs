﻿namespace GOAP
{
    /// <summary>
    /// convenince Action subclass with a typed context. This is useful when an Action requires validation so that it has some way to get
    /// the data it needs to do the validation.
    /// </summary>
    public class Action<T> : ActionGoap
    {
        protected T _context;

        public Action(T context, string name) : base(name)
        {
            _context = context;
            Name = name;
        }

        public Action(T context, string name, int cost) : this(context, name)
        {
            Cost = cost;
        }

        public virtual void Execute()
        {
        }
    }
}
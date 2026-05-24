using System;
using System.Collections.Generic;

namespace OCCPort
{
    public class deque<T> : List<T>
    {
        public  void Append(T aPnt2d)
        {
            Add(aPnt2d);
        }

        public bool IsEmpty()
        {
            return Count == 0;
        }
        public void clear()
        {
            Clear();
        }
        public T begin()
        {
            return this[0];
        }
        internal void erase(int v1, int v2)
        {
            RemoveRange(v1, v2);
        }

        internal int size()
        {
            return Count;

        }
    }

}
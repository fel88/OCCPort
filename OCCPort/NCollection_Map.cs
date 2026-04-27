using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace OCCPort
{
    internal class NCollection_Map<T>
    {
        internal bool Add(T e)
        {
            if (shapes.Contains(e))
            {
                return false;
            }
            shapes.Add(e);
            return true;
        }

        //! Added: add a new key if not yet in the map, and return 
        //! reference to either newly added or previously existing object
        public T Added(T K)
        {
            //probably should clone here
            if (!shapes.Contains(K))
            {
                shapes.Add(K);
            }
            return K;
        }

        public bool IsEmpty()
        {
            return shapes.Count == 0;

        }
        List<T> shapes = new List<T>();

        public NCollection_Map() { }
        public NCollection_Map(int v)
        {
        }

        internal void Remove(T e)
        {
            shapes.Remove(e);
        }

        internal bool Contains(T theCell)
        {
            return shapes.Contains(theCell);
        }
    }
}
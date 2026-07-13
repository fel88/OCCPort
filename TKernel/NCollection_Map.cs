using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace TKernel
{
    public class NCollection_Map<T> : NCollection_Map<T, NCollection_DefaultHasher<T>>
    {
        public NCollection_Map() : base() { }
        public NCollection_Map(int v) : base(v)
        {

        }

    }
    public class NCollection_Map<T, Hasher> : List<T> where Hasher : IEqualityComparer<T>, new()
    {
        Hasher hasher;
        public new bool Add(T e)
        {

            foreach (var item in this)
            {
                if (hasher.Equals(item, e))
                    return false;
            }
            //if (Contains(e))
            //{
            //    return false;
            //}
            base.Add(e);
            return true;
        }

        public new void Remove(T e)
        {
            for (int i = 0; i < this.Count; i++)
            {
                T? item = this[i];
                if (hasher.Equals(item, e))
                {
                    RemoveAt(i);
                    return;
                }
            }
        }

        public int Extent()
        {
            return Count;
        }
        //! Added: add a new key if not yet in the map, and return 
        //! reference to either newly added or previously existing object
        public T Added(T K)
        {

            foreach (var item in this)
            {
                if (hasher.Equals(item, K))
                {
                    return item;
                }
            }
            //probably should clone here
            //if (!Contains(K))
            //{
            base.Add(K);
            //}

            return K;
        }

        public bool IsEmpty()
        {
            return Count == 0;

        }


        public NCollection_Map()
        {

            hasher = new Hasher();
        }
        public NCollection_Map(int v)
        {
            hasher = new Hasher();
        }




    }
}
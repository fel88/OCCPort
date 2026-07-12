using System.Data;
using System.Reflection.Emit;

namespace TKernel
{
    /**
   * Purpose:     The DataMap is a Map to store keys with associated
   *              Items. See Map  from NCollection for  a discussion
   *              about the number of buckets.
   *
   *              The DataMap can be seen as an extended array where
   *              the Keys  are the   indices.  For this reason  the
   *              operator () is defined on DataMap to fetch an Item
   *              from a Key. So the following syntax can be used :
   *
   *              anItem = aMap(aKey);
   *              aMap(aKey) = anItem;
   *
   *              This analogy has its  limit.   aMap(aKey) = anItem
   *              can  be done only  if aKey was previously bound to
   *              an item in the map.
*/

    public class NCollection_DataMap<T1, T2> : NCollection_DataMap<T1, T2, NCollection_DefaultHasher<T1>>
    {
        public NCollection_DataMap()
        {
        }
        public NCollection_DataMap(int v, NCollection_IncAllocator myAllocator)
        {
        }
    }

    public class NCollection_DataMap<T1, T2, T3> : Dictionary<T1, T2> where T3 : IEqualityComparer<T1>, new()
    {
        public   T2 ChangeSeek(T1 theIObj)
        {
            throw new NotImplementedException();
        }

        public class Iterator
        {
            KeyValuePair<T1, T2>[] collection;
            public Iterator(NCollection_DataMap<T1, T2, T3> _collection)
            {
                collection = _collection.ToArray();
            }
            int index = 0;
            public T1 Key()
            {
                return collection[index].Key;
            }

            public bool More()
            {
                return index < collection.Length;
            }

            public void Next()
            {
                index++;
            }

            public T2 Value()
            {
                return collection[index].Value;

            }
        }
        public bool IsEmpty()
        {
            return Count == 0;
        }

        public void UnBind(T1 theIObj)
        {
            Remove(theIObj);
        }

        public T2 Find(T1 theObject)
        {
            if (ContainsKey(theObject))
            {
                return this[theObject];

            }
            return default;
        }
     
        public bool Find(T1 theObject, out T2 aResult)
        {
            if (ContainsKey(theObject))
            {
                aResult = this[theObject];
                return true;
            }
            aResult = default;
            return false;
        }

        public NCollection_DataMap() : base(new T3())
        {

        }

        public bool IsBound(T1 key)
        {
            return ContainsKey(key);
        }

        public int Extent()
        {
            return Count;
        }
        public int Size()
        {
            return Count;
        }

        public bool Bind(T1 aFreeEdgeId, T2 v)
        {
            bool ret = false;
            if (!ContainsKey(aFreeEdgeId))
            {
                ret = true;
                Add(aFreeEdgeId, v);
            }
            else
            {
                this[aFreeEdgeId] = v;
            }
            return ret;
        }
    }
}
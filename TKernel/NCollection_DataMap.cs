using TKernel;

namespace OCCPort
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

    }       

    public class NCollection_DataMap<T1, T2, T3> : Dictionary<T1, T2> where T3 : IEqualityComparer<T1>, new()        
    {

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
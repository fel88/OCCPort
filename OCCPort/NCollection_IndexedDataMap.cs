using System.Collections.Generic;

namespace OCCPort
{
    internal class NCollection_IndexedDataMap<T1, T2>
    {
        List<object> data = new List<object>();
        public int Extent()
        {
            return data.Count;

        }
    }
}
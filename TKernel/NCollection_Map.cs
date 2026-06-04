using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace TKernel
{
    public class NCollection_Map<T>:List<T>
    {
        public new  bool Add(T e)
        {
            if (Contains(e))
            {
                return false;
            }
            base.Add(e);
            return true;
        }

        //! Added: add a new key if not yet in the map, and return 
        //! reference to either newly added or previously existing object
        public T Added(T K)
        {
            foreach (var item in this)
            {
                if (item.Equals(K))
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
        

        public NCollection_Map() { }
        public NCollection_Map(int v)
        {
        }

        

        
    }
}
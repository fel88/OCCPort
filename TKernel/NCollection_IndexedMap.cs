using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace TKernel
{
    public class NCollection_IndexedMap<T, Hasher> : List<T> where Hasher : IEqualityComparer<T>, new()
    {        
        public NCollection_IndexedMap()
        {
            
        }

        Hasher hasher = new Hasher();

        public  void RemoveKey(T v1)
        {
            for (int i = 0; i < Count; i++)
            {
                if (hasher.Equals(this[i], v1))
                {
                    RemoveAt(i);
                    return;
                }
            }            
        }

        public new int Add(T theStruct)
        {
            if (this.Contains(theStruct, hasher))
            {
                for (int i = 0; i < Count; i++)
                {
                    if (hasher.Equals(this[i], theStruct))
                        return i;
                }                
            }
            
            base.Add(theStruct);
            return Count;
        }

        public int Size()
        {
            return Count;
        }        
    }
}

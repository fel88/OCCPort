using OCCPort.Common;
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

        public bool IsEmpty()
        {
            return Count == 0;
        }

        //! FindIndex
        public int FindIndex(T theKey1)
        {
            if (IsEmpty()) 
                return 0;

            for (int i = 0; i < Count; i++)
            {
                if (hasher.Equals(this[i], theKey1))
                {
                    RemoveAt(i);
                    return i;
                }
            }
            return 0;
            /*
            IndexedMapNode* pNode1 = (IndexedMapNode*)myData1[Hasher::HashCode(theKey1, NbBuckets())];
            while (pNode1)
            {
                if (Hasher::IsEqual(pNode1->Key1(), theKey1))
                {
                    return pNode1->Index();
                }
                pNode1 = (IndexedMapNode*)pNode1->Next();
            }
            return 0;*/
        }

        Hasher hasher = new Hasher();

        public void RemoveKey(T v1)
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

using OCCPort;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace OCCPort
{
    public class NCollection_IndexedDataMap<TheKeyType, T2, Hasher> : NCollection_BaseMap where Hasher : IEqualityComparer<TheKeyType>, new()
    {
        public class Iterator
        {
            public Iterator(NCollection_IndexedDataMap<TheKeyType, T2, Hasher> aMapOfPCurves)
            {
                myMap = aMapOfPCurves;
                myIndex = (1);
            }
            NCollection_IndexedDataMap<TheKeyType, T2, Hasher> myMap;   //!< Pointer to current node
            int myIndex; //!< Current index

            //! Query if the end of collection is reached by iterator
            internal bool More()
            {
                return (myMap != null) && (myIndex <= myMap.Extent());
            }

            public TheKeyType Key()
            {
                Exceptions.Standard_NoSuchObject_Raise_if(!More(), "NCollection_IndexedDataMap::Iterator::Key");
                return myMap.FindKey(myIndex);
            }

            //! Make a step along the collection
            internal void Next()
            {
                ++myIndex;
            }
            //! Value access
            public T2 Value()
            {
                Exceptions.Standard_NoSuchObject_Raise_if(!More(), "NCollection_IndexedDataMap::Iterator::Value");
                return myMap.FindFromIndex(myIndex);
            }

        }
        public NCollection_IndexedDataMap()
        {
            dic = new List<KeyValuePair<TheKeyType, T2>>();
        }
        internal T2 ChangeFromKey(TheKeyType theKey1)
        {
            if (IsEmpty())
                return default(T2);
            foreach (var item in dic)
            {
                if (hasher.Equals(item.Key, theKey1))
                    return item.Value;
            }
            return default(T2);
        }
        //! Contains
        public bool Contains(TheKeyType theKey1)
        {
            if (IsEmpty())
                return false;
            foreach (var item in dic)
            {
                if (hasher.Equals(item.Key, theKey1))
                    return true;
            }
            return false;

            //Standard_Integer iK1 = Hasher::HashCode(theKey1, NbBuckets());
            //IndexedDataMapNode* pNode1;
            //pNode1 = (IndexedDataMapNode*)myData1[iK1];
            //while (pNode1)
            //{
            //    if (Hasher::IsEqual(pNode1->Key1(), theKey1))
            //        return Standard_True;
            //    pNode1 = (IndexedDataMapNode*)pNode1->Next();
            //}
            //return Standard_False;
        }
        public List<KeyValuePair<TheKeyType, T2>> dic = null;

        Hasher hasher = new Hasher();
        public T2 this[int key]
        {
            get => FindFromIndex(key);
            //set => dic[key ]=new KeyValuePair<T1, T2> () = value;
        }
        public virtual bool IsEmpty()
        { return dic.Count == 0; }

        //! FindFromIndex
        public T2 FindFromIndex(int theIndex)
        {
            Exceptions.Standard_OutOfRange_Raise_if(theIndex < 1 || theIndex > Extent(), "NCollection_IndexedDataMap::FindFromIndex");
            return dic[theIndex - 1].Value;
            //IndexedDataMapNode* aNode = (IndexedDataMapNode*)myData2[theIndex - 1];
            //return aNode->Value();
        }

        public TheKeyType FindKey(int theIndex)
        {
            Exceptions.Standard_OutOfRange_Raise_if(theIndex < 1 || theIndex > Extent(), "NCollection_IndexedDataMap::FindKey");
            var aNode = dic[theIndex - 1];
            return aNode.Key;
            //return dic[theIndex].Key;
        }
        //! Returns the Index of already bound Key or appends new Key with specified Item value.
        //! @param theKey1 Key to search (and to bind, if it was not bound already)
        //! @param theItem Item value to set for newly bound Key; ignored if Key was already bound
        //! @return index of Key
        public int Add(TheKeyType theKey1, T2 theItem)
        {
            for (int i = 0; i < dic.Count; i++)
            {
                if (hasher.Equals(dic[i].Key, theKey1))
                    //if (dic[i].Key.Equals(theKey1))
                    return i;
            }
            dic.Add(new KeyValuePair<TheKeyType, T2>(theKey1, theItem));
            return dic.Count;
            //if (Resizable())
            //{
            //    ReSize(Extent());
            //}

            //const Standard_Integer iK1 = Hasher::HashCode(theKey1, NbBuckets());
            //IndexedDataMapNode* pNode = (IndexedDataMapNode*)myData1[iK1];
            //while (pNode)
            //{
            //    if (Hasher::IsEqual(pNode->Key1(), theKey1))
            //    {
            //        return pNode->Index();
            //    }
            //    pNode = (IndexedDataMapNode*)pNode->Next();
            //}

            //const Standard_Integer aNewIndex = Increment();
            //pNode = new(this->myAllocator) IndexedDataMapNode(theKey1, aNewIndex, theItem, myData1[iK1]);
            //myData1[iK1] = pNode;
            //myData2[aNewIndex - 1] = pNode;
            //return aNewIndex;
        }
        //! Substitute
        public void Substitute(int theIndex,
                     TheKeyType theKey1,
                     T2 theItem)
        {
            Exceptions.Standard_OutOfRange_Raise_if(theIndex < 1 || theIndex > Extent(),
                                          "NCollection_IndexedDataMap::Substitute : " +
                                          "Index is out of range");

            dic[theIndex] = new KeyValuePair<TheKeyType, T2>(theKey1, theItem);
            // check if theKey1 is not already in the map
            //const Standard_Integer iK1 = Hasher::HashCode(theKey1, NbBuckets());
            //IndexedDataMapNode* p = (IndexedDataMapNode*)myData1[iK1];
            //while (p)
            //{
            //    if (Hasher::IsEqual(p->Key1(), theKey1))
            //    {
            //        if (p->Index() != theIndex)
            //        {
            //            throw new Standard_DomainError("NCollection_IndexedDataMap::Substitute : "+
            //                                        "Attempt to substitute existing key");
            //        }
            //        p->Key1() = theKey1;
            //        p->ChangeValue() = theItem;
            //        return;
            //    }
            //    p = (IndexedDataMapNode*)p->Next();
            //}

            //// Find the node for the index I
            //p = (IndexedDataMapNode*)myData2[theIndex - 1];

            //// remove the old key
            //const Standard_Integer iK = Hasher::HashCode(p->Key1(), NbBuckets());
            //IndexedDataMapNode* q = (IndexedDataMapNode*)myData1[iK];
            //if (q == p)
            //    myData1[iK] = (IndexedDataMapNode*)p->Next();
            //else
            //{
            //    while (q->Next() != p)
            //        q = (IndexedDataMapNode*)q->Next();
            //    q->Next() = p->Next();
            //}

            //// update the node
            //p->Key1() = theKey1;
            //p->ChangeValue() = theItem;
            //p->Next() = myData1[iK1];
            //myData1[iK1] = p;
        }

        //! FindIndex
        public int FindIndex(TheKeyType theKey1)
        {
            if (IsEmpty())
                return 0;

            for (int i = 0; i < dic.Count; i++)
            {
                /*  if (hasher != null)
                  {
                      if (hasher.Equals(dic[i].Key, theKey1))
                          return i + 1;
                  }
                  else*/
                {

                    if (dic[i].Key.Equals(theKey1))
                        return i + 1;
                }
            }
            //IndexedDataMapNode* pNode1 = (IndexedDataMapNode*)myData1[Hasher::HashCode(theKey1, NbBuckets())];
            //while (pNode1)
            //{
            //    if (Hasher::IsEqual(pNode1->Key1(), theKey1))
            //    {
            //        return pNode1->Index();
            //    }
            //    pNode1 = (IndexedDataMapNode*)pNode1->Next();
            //}
            return 0;
        }

        public int Extent()
        {
            return dic.Count;

        }
    }
}
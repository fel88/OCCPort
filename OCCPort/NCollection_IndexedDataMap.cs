using OCCPort;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace OCCPort
{
    public class NCollection_IndexedDataMap<T1, T2> : NCollection_BaseMap
    {
        List<KeyValuePair<T1, T2>> dic = new List<KeyValuePair<T1, T2>>();

        public T2 this[int key]
        {
            get => dic[key].Value;
            //set => dic[key ]=new KeyValuePair<T1, T2> () = value;
        }

        public T1 FindKey(int theIndex)
        {
            return dic[theIndex].Key;
        }
        //! Returns the Index of already bound Key or appends new Key with specified Item value.
        //! @param theKey1 Key to search (and to bind, if it was not bound already)
        //! @param theItem Item value to set for newly bound Key; ignored if Key was already bound
        //! @return index of Key
        public int Add(T1 theKey1, T2 theItem)
        {
            for (int i = 0; i < dic.Count; i++)
            {
                if (dic[i].Key.Equals(theKey1))
                    return i;
            }
            dic.Add(new KeyValuePair<T1, T2>(theKey1, theItem));
            return dic.Count - 1;
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
                     T1 theKey1,
                     T2 theItem)
        {
            Exceptions.Standard_OutOfRange_Raise_if(theIndex < 1 || theIndex > Extent(),
                                          "NCollection_IndexedDataMap::Substitute : " +
                                          "Index is out of range");

            dic[theIndex] = new KeyValuePair<T1, T2>(theKey1, theItem);
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
        public int FindIndex(T1 theKey1)
        {
            if (IsEmpty())
                return 0;

            for (int i = 0; i < dic.Count; i++)
            {
                if (dic[i].Key.Equals(theKey1))
                    return i;
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
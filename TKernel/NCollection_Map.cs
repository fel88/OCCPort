using OCCPort.Common;
using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace TKernel
{
    /**
     * Purpose:     Single hashed Map. This  Map is used  to store and
     *              retrieve keys in linear time.
     *              
     *              The ::Iterator class can be  used to explore  the
     *              content of the map. It is not  wise to iterate and
     *              modify a map in parallel.
     *               
     *              To compute  the hashcode of  the key the  function
     *              ::HashCode must be defined in the global namespace
     *               
     *              To compare two keys the function ::IsEqual must be
     *              defined in the global namespace.
     *               
     *              The performance of  a Map is conditioned  by  its
     *              number of buckets that  should be kept greater  to
     *              the number   of keys.  This  map has  an automatic
     *              management of the number of buckets. It is resized
     *              when  the number of Keys  becomes greater than the
     *              number of buckets.
     *              
     *              If you have a fair  idea of the number of  objects
     *              you  can save on automatic   resizing by giving  a
     *              number of buckets  at creation or using the ReSize
     *              method. This should be  consider only for  crucial
     *              optimisation issues.
     */
    public class NCollection_Map<T> : NCollection_Map<T, NCollection_DefaultHasher<T>>
    {
        public NCollection_Map() : base() { }
        public NCollection_Map(int v) : base()
        {

        }

    }
    public class NCollection_Map<T, Hasher> : NCollection_BaseMap where Hasher : IHasher<T>, new()
    {
        public NCollection_Map() : base(1, true)
        {

        }
        //! Clear data. If doReleaseMemory is false then the table of
        //! buckets is not released and will be reused.
        public void Clear(bool doReleaseMemory = true)
        {

            //Destroy(MapNode::delNode, doReleaseMemory);
            Destroy(null, doReleaseMemory);
        }


        //!   Implementation of the Iterator interface.
        public new class Iterator : NCollection_BaseMap.Iterator
        {
            //! Constructor
            public Iterator(NCollection_Map<T, Hasher> theMap) :
       base(theMap)
            {
            }
            //! Query if the end of collection is reached by iterator
            public bool More()
            {
                return PMore();
            }

            //! Make a step along the collection
            public void Next()
            { PNext(); }


            //! Key
            public T Key()
            {
                Exceptions.Standard_NoSuchObject_Raise_if(!More(), "NCollection_Map::Iterator::Key");
                return ((MapNode)myNode).Value();
            }

            //! Value inquiry
            public T Value()
            {
                Exceptions.Standard_NoSuchObject_Raise_if(!More(), "NCollection_Map::Iterator::Value");
                return ((MapNode)myNode).Value();
            }
        }


        Hasher hasher = new Hasher();
        public bool Add(T K)
        {
            if (Resizable())
                ReSize(Extent());
            var data = myData1;
            int k = hasher.HashCode(K, NbBuckets());
            MapNode p = (MapNode)data[k];
            while (p != null)
            {
                if (hasher.IsEqual(p.Key(), K))
                    return false;
                p = (MapNode)p.Next();
            }
            data[k] = new MapNode(K, data[k]);
            Increment();
            return true;
        }

        public bool Remove(T K)
        {
            if (IsEmpty())
                return false;
            var data = myData1;
            int k = hasher.HashCode(K, NbBuckets());
            MapNode p = (MapNode)data[k];
            MapNode q = null;
            while (p != null)
            {
                if (hasher.IsEqual(p.Key(), K))
                {
                    Decrement();
                    if (q != null)
                        q.Next(p.Next());
                    else
                        data[k] = (MapNode)p.Next();
                    //p->~MapNode();
                    // this->myAllocator->Free(p);
                    return true;
                }
                q = p;
                p = (MapNode)p.Next();
            }
            return false;
        }


        //! ReSize
        void ReSize(int N)
        {
            NCollection_ListNode[] newdata = null;
            NCollection_ListNode[] dummy = null;
            int newBuck = 0;
            if (BeginResize(N, ref newBuck, ref newdata, ref dummy))
            {
                if (myData1 != null)
                {
                    var olddata = myData1;
                    MapNode p = null, q = null;
                    int i, k;
                    for (i = 0; i <= NbBuckets(); i++)
                    {
                        if (olddata[i] != null)
                        {
                            p = (MapNode)olddata[i];
                            while (p != null)
                            {
                                k = hasher.HashCode(p.Key(), newBuck);
                                q = (MapNode)p.Next();
                                p.Next(newdata[k]);
                                newdata[k] = p;
                                p = q;
                            }
                        }
                    }
                }
                EndResize(N, newBuck, newdata, dummy);
            }
        }

        //! Added: add a new key if not yet in the map, and return 
        //! reference to either newly added or previously existing object
        public T Added(T K)
        {
            if (Resizable())
                ReSize(Extent());
            var data = myData1;
            int k = hasher.HashCode(K, NbBuckets());
            MapNode p = (MapNode)data[k];
            while (p != null)
            {
                if (hasher.IsEqual(p.Key(), K))
                    return p.Key();
                p = (MapNode)p.Next();
            }
            data[k] = new MapNode(K, data[k]);
            Increment();
            return ((MapNode)data[k]).Key();
        }

        //!   Adaptation of the TListNode to the map notations
        public class MapNode : NCollection_TListNode<T>
        {
            //! Constructor with 'Next'
            public MapNode(T theKey,
               NCollection_ListNode theNext)
              :
        base(theKey, theNext)
            { }

            //! Key
            public T Key()
            { return this.Value(); }
        }


        //! Contains
        public bool Contains(T K)
        {
            if (IsEmpty())
                return false;

            MapNode p = (MapNode)myData1[hasher.HashCode(K, NbBuckets())];
            while (p != null)
            {
                if (hasher.IsEqual(p.Key(), K))
                    return true;
                p = (MapNode)p.Next();
            }
            return false;
        }



    }

    /**
 * Purpose:     Abstract list node class. Used by BaseList
 * Remark:      Internal class
 */
    public class NCollection_TListNode<TheItemType>
      : NCollection_ListNode
    {

        public NCollection_TListNode(TheItemType theItem,
                            NCollection_ListNode theNext = null)
           : base(theNext)
        {
            myValue = (theItem);
        }
        //! Constant value access
        public TheItemType Value()
        {
            return myValue;
        }

        //! Variable value access
        public TheItemType ChangeValue() { return myValue; }
        public void ChangeValue(TheItemType v) { myValue = v; }


        TheItemType myValue; //!< The item stored in the node

    }


}
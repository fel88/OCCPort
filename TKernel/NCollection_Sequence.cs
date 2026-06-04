using OCCPort.Common;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Xml.Linq;

namespace TKernel
{
    public class NCollection_Sequence<T> : List<T>
    {
        
        public int Length()
        {
            return Count;
        }
        public bool IsEmpty()
        {
            return Count == 0;
        }
        public T ChangeValue(int i)
        {
            return this[i - 1];
        }

        //! Constant item access by theIndex
        public T Value(int theIndex)
        {
            Exceptions.Standard_OutOfRange_Raise_if(theIndex <= 0 || theIndex > Count, "NCollection_Sequence::Value");
            return this[theIndex - 1];
            //NCollection_Sequence * const aLocalTHIS = (NCollection_Sequence*)this;
            //aLocalTHIS->myCurrentItem = Find(theIndex);
            //aLocalTHIS->myCurrentIndex = theIndex;
            //return ((const Node*) myCurrentItem) -> Value();
        }

        public void ChangeValue(int i, T thePnt)
        {
            this[i - 1] = thePnt;
        }

        public int Size()
        {
            return Count;
        }

        //! InsertBefore theIndex theItem
        public void InsertBefore(int theIndex,
                     T theItem)
        {
            //InsertAfter(theIndex - 1, theItem); 
            Insert(theIndex - 1, theItem);
        }

        //! InsertBefore theIndex another sequence (making it empty)
        public void InsertBefore(int theIndex,
                           NCollection_Sequence<T> theSeq)
        {

            //    InsertAfter(theIndex - 1, theSeq);
            InsertRange(theIndex - 1, theSeq);
        }



        public void Append(T t)
        {
            Add(t);
        }
        public new T this[int key]
        {
            get => base[key - Lower()];
            set => base[key - Lower()] = value;
        }
        //! Method for consistency with other collections.
        //! @return Lower bound (inclusive) for iteration.
        public int Lower()
        {
            return 1;
        }
        public int Upper()
        {
            return Count - 1;
        }
        //! Method for consisten
        internal T First()
        {
            return this[Lower()];
        }

        public void Remove(int v)
        {
            RemoveAt(v - 1);
        }
    }
}
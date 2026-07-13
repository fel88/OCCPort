



namespace TKernel
{//! Generic matrix of 4 x 4 elements.
    public class NCollection_List<T> : List<T>
    {
        public NCollection_List() { }

        //! Copy constructor
        public NCollection_List(NCollection_List<T> copy)
        {
            Assign(copy);
        }

        //! Replace this list by the items of another list (theOther parameter).
        //! This method does not change the internal allocator.
        public NCollection_List<T> Assign(NCollection_List<T> theOther)
        {
            if (this != theOther)
            {
                Clear();
                //appendList(theOther.PFirst());
                AddRange(theOther);
            }
            return this;
        }
        public int Extent()
        {
            return Count;
        }
        public void Prepend(T a)
        {
            Insert(0, a);
        }


        public class Iterator
        {
            public void Initialize(NCollection_List<T> myShapes)
            {
                list = myShapes;
                index = 0;
            }

            public NCollection_List<T> list;
            public Iterator() { }
            public Iterator(NCollection_List<T> aDisplayedObjects)
            {
                list = aDisplayedObjects;
            }

            int index = 0;
            public bool More()
            {
                return index < list.Count;
            }

            public void Next()
            {
                index++;
            }

            public T Value()
            {
                return list[index];
            }

            public T ChangeValue()
            {
                return Value();
            }
        }
        public void Append(T aPoints)
        {
            Add(aPoints);
        }

        public T First()
        {
            return this[0];
        }
        public T Last()
        {
            return this[^1];
        }

        public bool IsEmpty()
        {
            return Count == 0;
        }
        public int Size()
        {
            return Count;
        }

        public void InsertBefore(T aNewLayer, T aLayerIter)
        {
            var index = IndexOf(aLayerIter);
            Insert(index, aNewLayer);
        }

        public void InsertAfter(T aNewLayer, T aLayerIter)
        {
            var index = IndexOf(aLayerIter);
            Insert(index + 1, aNewLayer);
        }

        public void RemoveFirst()
        {
            RemoveAt(0);
        }
    }
}

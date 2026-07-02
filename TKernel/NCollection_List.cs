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


        public class Iterator
        {

            NCollection_List<T> list;
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

       
    }
}

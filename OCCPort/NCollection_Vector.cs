using System.Collections.Generic;
using System.Threading;

namespace OCCPort
{
    public class NCollection_Vector<T> : NCollection_BaseVector<T>
    {

        //! Operator[] - query the const value
        public new T this[int key]
        {
            get => Value(key);

        }

        public bool IsEmpty()
        {
            return Size() == 0;
        }

        public T Value(int theIndex)
        {
            return findV(theIndex);
        }

     

        public void Append(T t)
        {
            Add(t);
        }  //! Total number of items in the vector
        public int Size()
        {
            return Count;
        }  //! Method for consistency with other collections.
           //! @return Lower bound (inclusive) for iteration.
        public int Lower()
        {
            return 0;
        }  //! Method for consistency with other collections.
           //! @return Upper bound (inclusive) for iteration.
        public int Upper()
        {
            return Count - 1;
        }


    }
}
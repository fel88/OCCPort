using System.Collections.Generic;

namespace OCCPort
{
    public class NCollection_Vector<T> : List<T>
    {
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
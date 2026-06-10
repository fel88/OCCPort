
namespace TKernel
{
    //! Class NCollection_Vector (dynamic array of objects)
    //!
    //! This class is similar to NCollection_Array1  though the indices always start
    //! at 0 (in Array1 the first index must be specified)
    //!
    //! The Vector is always created with 0 length. It can be enlarged by two means:
    //!  1. Calling the method Append (val) - then "val" is added to the end of the
    //!     vector (the vector length is incremented)
    //!  2. Calling the method SetValue (i, val)  - if "i" is greater than or equal
    //!     to the current length of the vector,  the vector is enlarged to accomo-
    //!     date this index
    //!
    //! The methods Append and SetValue return  a non-const reference  to the copied
    //! object  inside  the vector.  This reference  is guaranteed to be valid until
    //! the vector is destroyed. It can be used to access the vector member directly
    //! or to pass its address to other data structures.
    //!
    //! The vector iterator remembers the length of the vector  at the moment of the
    //! creation or initialisation of the iterator.   Therefore the iteration begins
    //! at index 0  and stops at the index equal to (remembered_length-1).  It is OK
    //! to enlarge the vector during the iteration.
    public class NCollection_Vector<T> : NCollection_BaseVector<T>
    {

        //! Operator[] - query the const value
        public new T this[int key]
        {
            get => Value(key);
            set => base[key] = value;

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

        public class Iterator
        {
            public Iterator(NCollection_Vector<T> selectMgr_SensitiveEntities)
            {

            }

            public bool More()
            {
                throw new NotImplementedException();
            }

            public object Next()
            {
                throw new NotImplementedException();
            }

            public T Value()
            {
                throw new NotImplementedException();
            }
        }
    }
}
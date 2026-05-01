using System.Collections.Generic;

namespace OCCPort
{
    public class NCollection_BaseVector<T> : List<T>
    {   //! Locate the memory holding the desired value
        public T findV(int theIndex)
        {
            Exceptions.Standard_OutOfRange_Raise_if(theIndex < 0 || theIndex >= Count,
                                          "NCollection_BaseVector::findV");
            ;
            return this[theIndex];
        }

    }
}
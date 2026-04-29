namespace OCCPort
{
    /**
     * Purpose:     This is a base class for all Maps:
     *                Map
     *                DataMap
     *                DoubleMap
     *                IndexedMap
     *                IndexedDataMap
     *              Provides utilitites for managing the buckets.
     */

    public class NCollection_BaseMap
    {
        int mySize;

        //! IsEmpty
        public virtual bool IsEmpty()
        { return mySize == 0; }

    }
}
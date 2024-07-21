namespace OCCPort
{
    public class NCollection_Buffer
    {
        protected byte[] myData;      //!< data pointer
        protected int mySize;      //!< buffer length in bytes
                                   //Handle(NCollection_BaseAllocator) myAllocator; //!< buffer allocator
        public NCollection_Buffer(NCollection_BaseAllocator theAlloc,
                        int theSize = 0,
                       byte[] theData = null)
        {
            myData = (null);
            mySize = (0);
            //myAllocator = (theAlloc);

            if (theData != null)
            {
                myData = theData;
                mySize = theSize;
            }
            else
            {
                //Allocate(theSize);
            }
        }
    }
}
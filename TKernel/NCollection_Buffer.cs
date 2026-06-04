namespace TKernel
{//! Generic matrix of 4 x 4 elements.
    public class NCollection_Buffer
    {
        protected byte[] myData;      //!< data pointer
        protected int mySize;      //!< buffer length in bytes
                                   //Handle(NCollection_BaseAllocator) myAllocator; //!< buffer allocator


        //! @return true if buffer is not allocated
        public bool IsEmpty()
        {
            return myData == null;
        }

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
        }//! @return buffer data
        public byte[] Data()
        {
            return myData;
        }
        //! De-allocate buffer.
        protected void Free()
        {
            //if (!myAllocator.IsNull())
            {
                //   myAllocator->Free(myData);
            }
            myData = null;
            mySize = 0;
        }

    }
}

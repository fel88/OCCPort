using System;

namespace OCCPort
{
    //! Buffer of vertex attributes.
    //! This class is intended for advanced usage allowing invalidation of entire buffer content or its sub-part.
    class Graphic3d_AttribBuffer : Graphic3d_Buffer
    {
        // =======================================================================
        public Graphic3d_AttribBuffer(NCollection_BaseAllocator theAlloc) : base(theAlloc)
        {
            myIsInterleaved = (true);
            myIsMutable = false;
        }

        //! Return number of initially allocated elements which can fit into this buffer,
        //! while NbElements can be overwritten to smaller value.
        public int NbMaxElements() { return Stride != 0 ? (int)(mySize / (int)(Stride)) : 0; }

        public void SetInterleaved(bool theIsInterleaved)
        {
            if (NbMaxElements() != 0)
            {
                throw new Standard_ProgramError("Graphic3d_AttribBuffer::SetInterleaved() should not be called for allocated buffer");
            }
            myIsInterleaved = theIsInterleaved;
        }
        public void SetMutable(bool theMutable)
        {
            if (mySize > Standard_Integer.IntegerLast()
             && theMutable)
            {
                throw new Standard_OutOfRange("Graphic3d_AttribBuffer::SetMutable(), Mutable flag cannot be used for buffer exceeding 32-bit address space");
            }
            myIsMutable = theMutable;
        }

        Graphic3d_BufferRange myInvalidatedRange; //!< invalidated buffer data range (as byte offsets)
        bool myIsInterleaved;    //!< flag indicating the vertex attributes being interleaved
        bool myIsMutable;        //!< flag indicating that data can be invalidated
    }
}
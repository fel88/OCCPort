namespace OCCPort
{
    public class Graphic3d_IndexBuffer : Graphic3d_Buffer
    {  //! Empty constructor.
     public   Graphic3d_IndexBuffer(NCollection_BaseAllocator theAlloc)
  : base(theAlloc) { }

        //! Access index at specified position
        public int Index(int theIndex)
        {
            return Stride == sizeof(ushort)
                 ? (int)((ushort)(theIndex))
                 : (int)((uint)(theIndex));
        }

        //! Change index at specified position
        public void SetIndex(int theIndex, int theValue)
        {
            if (Stride == sizeof(ushort))
            {
                //ChangeValue < unsigned short> (theIndex) = (unsigned short )theValue;
            }
            else
            {
                //ChangeValue < unsigned int> (theIndex) = (unsigned int   )theValue;
            }
        }

    }
}